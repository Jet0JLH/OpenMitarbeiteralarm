Imports System.IO.Ports
Imports MQTTnet.Client

Public Class Form1
    Public Const globalConfigPath As String = "config.xml"
    Dim serverAddress As String
    Dim serverPort As Integer
    Dim clientID As String
    Dim mqttUser As String
    Dim mqttPw As String
    Dim userFirstname As String
    Dim userLastname As String
    Dim userRoom As String
    Dim alertGroups As List(Of String)

    Dim factory = New MQTTnet.MqttFactory
    Dim mqttClient As MQTTnet.Client.MqttClient = factory.CreateMqttClient
    Public xml As XDocument

    Public Function loadGlobalConfig() As Boolean
        If My.Computer.FileSystem.FileExists(globalConfigPath) Then
            Try
                Dim de As System.DirectoryServices.DirectoryEntry = System.DirectoryServices.AccountManagement.UserPrincipal.Current.GetUnderlyingObject
                xml = XDocument.Load(globalConfigPath)
                serverAddress = xml.<conf>.<server>.<address>.Value
                serverPort = xml.<conf>.<server>.<port>.Value
                mqttUser = xml.<conf>.<server>.<user>.Value
                mqttPw = xml.<conf>.<server>.<pw>.Value
                If xml.<conf>.<client>.<firstname>.Value <> "" Then
                    userFirstname = xml.<conf>.<client>.<firstname>.Value
                Else
                    userFirstname = System.DirectoryServices.AccountManagement.UserPrincipal.Current.GivenName
                End If
                If xml.<conf>.<client>.<lastname>.Value <> "" Then
                    userLastname = xml.<conf>.<client>.<lastname>.Value
                Else
                    userLastname = System.DirectoryServices.AccountManagement.UserPrincipal.Current.Surname
                End If
                If xml.<conf>.<client>.<room>.Value <> "" Then
                    userRoom = xml.<conf>.<client>.<room>.Value
                Else
                    userRoom = de.Properties.Item("physicalDeliveryOfficeName").Value
                End If
                If xml.<conf>.<client>.<id>.Value <> "" Then
                    clientID = xml.<conf>.<client>.<id>.Value
                Else
                    clientID = My.Computer.Name & "-" & My.User.CurrentPrincipal.Identity.Name
                End If
                If xml.<conf>.<button>.<serialport>.Value <> "" Then
                    SerialPort1.PortName = xml.<conf>.<button>.<serialport>.Value
                End If
                alertGroups = New List(Of String)
                For Each item In xml.<conf>.<alerts>.Elements("group")
                    alertGroups.Add(item.Value)
                Next
                Return True
            Catch ex As Exception
                MsgBox("Fehler beim Laden der Konfigurationsdatei", MsgBoxStyle.Critical)
                Me.Close()
            End Try
        End If
        Return False
    End Function
    Private Sub testMQTT()
        If mqttClient.IsConnected Then
            'Dim msg As MQTTnet.MqttApplicationMessage = New MQTTnet.MqttApplicationMessageBuilder().WithTopic("gebäude1/warnkreis1").WithPayload("Testalarm").WithExactlyOnceQoS().Build()
            'mqttClient.PublishAsync(msg, Threading.CancellationToken.None)
            sendAlert()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        Console.WriteLine("Lade Konfiuration")
        If loadGlobalConfig() Then
            Console.WriteLine("Versuche mit Server zu verbinden")


            AddHandler SerialPort1.PinChanged, AddressOf SerialPort1_PinChanged

            ConnectionTimer.Start()
        End If
    End Sub
    Private Sub SerialPort1_PinChanged(sender As Object, e As IO.Ports.SerialPinChangedEventArgs)
        If e.EventType = IO.Ports.SerialPinChange.CtsChanged Then
            Threading.Thread.Sleep(50)
            If SerialPort1.IsOpen Then
                If Not SerialPort1.CtsHolding Then
                    sendAlert()
                End If
            End If
        End If
    End Sub

    Private Sub ConnectionTimer_Tick(sender As Object, e As EventArgs) Handles ConnectionTimer.Tick
        If SerialPort1.IsOpen Then

        Else
            Try
                If SerialPort1.PortName <> "none" Then
                    SerialPort1.Open()
                    BtnStatusLabel.Text = "Knopf vorhanden"
                    BtnStatusLabel.Tag = 0
                End If
            Catch ex As Exception
                If BtnStatusLabel.Tag <> 1 Then
                    BtnStatusLabel.Text = "Keine Verbindung zu Knopf"
                    BtnStatusLabel.Tag = 1
                End If
            End Try
        End If
        If mqttClient.IsConnected Then
            If MQTTStatusLabel.Tag <> 0 Then
                MQTTStatusLabel.Text = "Mit Server verbunden"
                MQTTStatusLabel.Tag = 0
            End If
        Else
            If MQTTStatusLabel.Tag <> 1 Then
                MQTTStatusLabel.Text = "Keine Server Verbindung"
                MQTTStatusLabel.Tag = 1
                Dim options = New MQTTnet.Client.Options.MqttClientOptionsBuilder().WithClientId(clientID).WithTcpServer(serverAddress, serverPort).WithCredentials(mqttUser, mqttPw).WithCleanSession().Build()

                Dim mqttTask As Task(Of MQTTnet.Client.Connecting.MqttClientAuthenticateResult) = mqttClient.ConnectAsync(options, Threading.CancellationToken.None)

                While mqttTask.IsCompleted = False
                    Console.WriteLine(mqttTask.IsCompleted)
                End While
                Console.WriteLine(mqttTask.IsCompleted)
                Console.WriteLine(mqttClient.IsConnected)
                For Each item In alertGroups
                    Dim unused = subscribeMQTT(item)
                Next
                mqttClient.UseApplicationMessageReceivedHandler(AddressOf reciveAlert)
            End If
        End If

    End Sub
    Private Sub sendAlert()
        Dim text As String = My.Computer.Name & ";alert;" & userFirstname & ";" & userLastname & ";" & userRoom
        If mqttClient.IsConnected Then
            For Each item In alertGroups
                Dim msg As MQTTnet.MqttApplicationMessage = New MQTTnet.MqttApplicationMessageBuilder().WithTopic(item).WithPayload(Text).WithExactlyOnceQoS().WithRetainFlag(False).Build()
                mqttClient.PublishAsync(msg, Threading.CancellationToken.None)
            Next
        End If
    End Sub

    Private Async Function subscribeMQTT(topic As String) As Task(Of MQTTnet.Client.Subscribing.MqttClientSubscribeResult)
        Dim filter As MQTTnet.MqttTopicFilter = New MQTTnet.MqttTopicFilterBuilder().WithTopic(topic).WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce).Build()
        Dim options = New MQTTnet.Client.Subscribing.MqttClientSubscribeOptions()
        options.TopicFilters.Add(filter)
        Return Await mqttClient.SubscribeAsync(options, Threading.CancellationToken.None)
    End Function

    Private Sub reciveAlert(e As MQTTnet.MqttApplicationMessageReceivedEventArgs)
        Dim parameter As List(Of String) = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload).Split(";").ToList
        If parameter.Count > 1 Then
            If parameter(0) <> My.Computer.Name And parameter(1) = "alert" Then
                If parameter.Count <= 3 Then
                    setAlertText(parameter(2), "", "?")
                ElseIf parameter.Count <= 4 Then
                    setAlertText(parameter(2), parameter(3), "?")
                Else
                    setAlertText(parameter(2), parameter(3), parameter(4))
                End If
                My.Computer.Audio.Play(My.Resources.alert, AudioPlayMode.Background)
                Me.Show()
                Me.TopMost = True
                Me.BringToFront()
                Me.Focus()
            End If
        End If
    End Sub
    Public Sub setAlertText(vorname As String, nachname As String, raum As String)
        Label2.Text = "In Raum " & raum & " bei Mitarbeiter " & vorname & " " & nachname & " wurde Alarm ausgelöst!" & vbCrLf & "Bitte einmal nach dem Kollegen sehen"
        Label3.Text = "Ausgelöst: " & My.Computer.Clock.LocalTime
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
    End Sub

    Private Sub BeendenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeendenToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Hide()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles BackgroundColorTimer.Tick
        If Me.BackColor = Color.Red Then
            Me.BackColor = Color.LightGray
        Else
            Me.BackColor = Color.Red
        End If
    End Sub

    Private Sub EinstellungenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EinstellungenToolStripMenuItem.Click
        If Application.OpenForms().OfType(Of SettingsForm).Any = False Then
            SettingsForm.ShowDialog()
        End If
    End Sub
End Class
