Public Class SettingsForm
    Public Sub loadConfig()
        TextBox1.Text = Form1.xml.<conf>.<server>.<address>.Value
        TextBox2.Text = Form1.xml.<conf>.<server>.<port>.Value
        TextBox4.Text = Form1.xml.<conf>.<server>.<user>.Value
        TextBox3.Text = Form1.xml.<conf>.<server>.<pw>.Value

        TextBox5.Text = Form1.xml.<conf>.<client>.<firstname>.Value
        TextBox6.Text = Form1.xml.<conf>.<client>.<lastname>.Value
        TextBox7.Text = Form1.xml.<conf>.<client>.<room>.Value
        TextBox8.Text = Form1.xml.<conf>.<client>.<id>.Value

        TextBox9.Text = Form1.xml.<conf>.<button>.<serialport>.Value

        RichTextBox1.Clear()
        For Each item In Form1.xml.<conf>.<alerts>.Elements("group")
            RichTextBox1.AppendText(item.Value & vbCrLf)
        Next
        If Form1.alertSound <= 1 Then
            ComboBox1.SelectedIndex = Form1.alertSound
        Else
            ComboBox1.SelectedIndex = 0
        End If
    End Sub
    Public Function generateConfig() As XDocument
        Dim xml As New XDocument(<conf><server></server><client></client><alerts></alerts><button></button></conf>)
        If removeSpaces(TextBox1.Text) <> "" Then
            xml.Element("conf").Element("server").Add(<address></address>)
            xml.Element("conf").Element("server").Element("address").Value = removeSpaces(TextBox1.Text)
        End If
        If removeSpaces(TextBox2.Text) <> "" Then
            xml.Element("conf").Element("server").Add(<port></port>)
            xml.Element("conf").Element("server").Element("port").Value = removeSpaces(TextBox2.Text)
        End If
        If removeSpaces(TextBox4.Text) <> "" Then
            xml.Element("conf").Element("server").Add(<user></user>)
            xml.Element("conf").Element("server").Element("user").Value = removeSpaces(TextBox4.Text)
        End If
        If removeSpaces(TextBox3.Text) <> "" Then
            xml.Element("conf").Element("server").Add(<pw></pw>)
            xml.Element("conf").Element("server").Element("pw").Value = removeSpaces(TextBox3.Text)
        End If
        If removeSpaces(TextBox5.Text) <> "" Then
            xml.Element("conf").Element("client").Add(<firstname></firstname>)
            xml.Element("conf").Element("client").Element("firstname").Value = removeSpaces(TextBox5.Text)
        End If
        If removeSpaces(TextBox6.Text) <> "" Then
            xml.Element("conf").Element("client").Add(<lastname></lastname>)
            xml.Element("conf").Element("client").Element("lastname").Value = removeSpaces(TextBox6.Text)
        End If
        If removeSpaces(TextBox7.Text) <> "" Then
            xml.Element("conf").Element("client").Add(<room></room>)
            xml.Element("conf").Element("client").Element("room").Value = removeSpaces(TextBox7.Text)
        End If
        If removeSpaces(TextBox8.Text) <> "" Then
            xml.Element("conf").Element("client").Add(<id></id>)
            xml.Element("conf").Element("client").Element("id").Value = removeSpaces(TextBox8.Text)
        End If
        If removeSpaces(TextBox9.Text) <> "" Then
            xml.Element("conf").Element("button").Add(<serialport></serialport>)
            xml.Element("conf").Element("button").Element("serialport").Value = removeSpaces(TextBox9.Text)
        End If
        xml.Element("conf").Element("alerts").Add(<sound></sound>)
        xml.Element("conf").Element("alerts").Element("sound").Value = ComboBox1.SelectedIndex
        For Each line In RichTextBox1.Lines
            If removeSpaces(line) <> "" Then
                Dim element As New XElement(<group></group>)
                element.Value = removeSpaces(line)
                xml.Element("conf").Element("alerts").Add(element)
            End If
        Next
        Return xml
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            generateConfig().Save(Form1.globalConfigPath)
            If MsgBox("Konfiguration wurde gespeichert. Damit diese ültig ist, muss der Mitarbeiteralarm neugestartet werden. Soll dies nun automatisch passieren?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                Application.Restart()
            Else
                Me.Close()
            End If
        Catch ex As Exception
            MsgBox("Fehler beim Speichern der Konfiguration ist aufgetreten", MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        loadConfig()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        loadConfig()
    End Sub
    Private Function removeSpaces(text As String) As String
        While text.StartsWith(" ")
            text = text.Substring(1)
        End While
        While text.EndsWith(" ")
            text = text.Substring(0, text.Length - 1)
        End While
        Return text
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Select Case ComboBox1.SelectedIndex
            Case 0
                My.Computer.Audio.Play(My.Resources.alert, AudioPlayMode.Background)
            Case 1
                My.Computer.Audio.Play(My.Resources.alert2, AudioPlayMode.Background)
        End Select
    End Sub
End Class