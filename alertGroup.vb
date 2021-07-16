Public Class alertGroup
    Public path As String
    Public direction As directions
    Public Enum directions
        sendAndRecive = 0
        sendOnly = 1
        reciveOnly = 2
    End Enum
    Public Sub New(path As String, direction As directions)
        Me.path = path
        Me.direction = direction
    End Sub
    Public Sub New()
        Me.New("", directions.sendAndRecive)
    End Sub
    Public Sub New(path As String)
        Me.New(path, directions.sendAndRecive)
    End Sub
    Public Function canSend() As Boolean
        If Me.direction = directions.sendAndRecive Or Me.direction = directions.sendOnly Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function canRecive() As Boolean
        If Me.direction = directions.sendAndRecive Or Me.direction = directions.reciveOnly Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
