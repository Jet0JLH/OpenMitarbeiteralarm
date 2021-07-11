Imports System.ComponentModel

Public Class AlertForm
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Me.BackColor = Color.Red Then
            Me.BackColor = Color.LightGray
        Else
            Me.BackColor = Color.Red
        End If
    End Sub
    Public Sub setText(vorname As String, nachname As String, raum As String)
        Label2.Text = "In Raum " & raum & " bei Mitarbeiter " & vorname & " " & nachname & " wurde Alarm ausgelöst!" & vbCrLf & "Bitte einmal nach dem Kollegen sehen"
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub AlertForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Timer1.Stop()
    End Sub
    Private Sub AlertForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        My.Computer.Audio.Play(My.Resources.alert, AudioPlayMode.Background)
        Timer1.Start()
        Me.TopMost = True
        Me.BringToFront()
        Me.Focus()
    End Sub
End Class