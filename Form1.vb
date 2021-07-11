﻿Imports System.IO.Ports
Imports MQTTnet.Client

Public Class Form1
    Const globalConfigPath As String = "config.xml"
    Dim serverAddress As String
    Dim serverPort As Integer
    Dim clientID As String
    Dim mqttUser As String
    Dim mqttPw As String
    Dim alertGroups As List(Of String)

    Dim factory = New MQTTnet.MqttFactory
    Dim mqttClient As MQTTnet.Client.MqttClient = factory.CreateMqttClient

    Public Function loadGlobalConfig() As Boolean
        If My.Computer.FileSystem.FileExists(globalConfigPath) Then
            Try
                Dim xml As XDocument = XDocument.Load(globalConfigPath)
                serverAddress = xml.<conf>.<server>.<address>.Value
                serverPort = xml.<conf>.<server>.<port>.Value
                mqttUser = xml.<conf>.<server>.<user>.Value
                mqttPw = xml.<conf>.<server>.<pw>.Value
                clientID = xml.<conf>.<client>.<id>.Value
                SerialPort1.PortName = xml.<conf>.<button>.<serialport>.Value
                alertGroups = New List(Of String)
                For Each item In xml.<conf>.<alerts>.Elements("group")
                    alertGroups.Add(item.Value)
                Next
                Return True
            Catch ex As Exception
                Console.Error.WriteLine("Fehler beim Laden der Konfigurationsdatei")
            End Try
        End If
        Return False
    End Function
    Private Sub testMQTT() Handles Button1.Click
        If mqttClient.IsConnected Then
            'Dim msg As MQTTnet.MqttApplicationMessage = New MQTTnet.MqttApplicationMessageBuilder().WithTopic("gebäude1/warnkreis1").WithPayload("Testalarm").WithExactlyOnceQoS().Build()
            'mqttClient.PublishAsync(msg, Threading.CancellationToken.None)
            alert("test")
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
                    alert("Alarm")
                End If
            End If
        End If
    End Sub

    Private Sub ConnectionTimer_Tick(sender As Object, e As EventArgs) Handles ConnectionTimer.Tick
        If SerialPort1.IsOpen Then

        Else
            Try
                SerialPort1.Open()
                BtnStatusLabel.Text = "Knopf vorhanden"
                BtnStatusLabel.Tag = 0
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
    Private Sub alert(text As String)
        If mqttClient.IsConnected Then
            For Each item In alertGroups
                Dim msg As MQTTnet.MqttApplicationMessage = New MQTTnet.MqttApplicationMessageBuilder().WithTopic(item).WithPayload(text).WithExactlyOnceQoS().WithRetainFlag(False).Build()
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
        MsgBox(e.ApplicationMessage.Topic & " - " & System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload))
    End Sub
End Class
