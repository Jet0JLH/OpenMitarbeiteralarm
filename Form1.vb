Public Class Form1
    Const globalConfigPath As String = "config.xml"
    Dim serverAddress As String
    Dim serverPort As Integer
    Dim clientID As String
    Dim mqttUser As String
    Dim mqttPw As String

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
                Return True
            Catch ex As Exception
                Console.Error.WriteLine("Fehler beim Laden der Konfigurationsdatei")
            End Try
        End If
        Return False
    End Function
    Private Sub testMQTT() Handles Button1.Click
        If mqttClient.IsConnected Then
            Dim msg As MQTTnet.MqttApplicationMessage = New MQTTnet.MqttApplicationMessageBuilder().WithTopic("gebäude1/warnkreis1").WithPayload("Testalarm").WithExactlyOnceQoS().Build()
            mqttClient.PublishAsync(msg, Threading.CancellationToken.None)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("Lade Konfiuration")
        If loadGlobalConfig() Then
            Console.WriteLine("Versuche mit Server zu verbinden")
            Dim options = New MQTTnet.Client.Options.MqttClientOptionsBuilder().WithClientId(clientID).WithTcpServer(serverAddress, serverPort).WithCredentials(mqttUser, mqttPw).WithCleanSession().Build()

            Dim mqttTask As Task(Of MQTTnet.Client.Connecting.MqttClientAuthenticateResult) = mqttClient.ConnectAsync(options, Threading.CancellationToken.None)

            While mqttTask.IsCompleted = False
                Console.WriteLine(mqttTask.IsCompleted)
            End While
            Console.WriteLine(mqttTask.IsCompleted)
            Console.WriteLine(mqttClient.IsConnected)

        End If
    End Sub
End Class
