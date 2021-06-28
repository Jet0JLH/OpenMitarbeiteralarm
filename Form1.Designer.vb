<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.MQTTStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BtnStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SerialTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MQTTStatusLabel, Me.BtnStatusLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 428)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.StatusStrip1.Size = New System.Drawing.Size(800, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'MQTTStatusLabel
        '
        Me.MQTTStatusLabel.Name = "MQTTStatusLabel"
        Me.MQTTStatusLabel.Size = New System.Drawing.Size(152, 17)
        Me.MQTTStatusLabel.Text = "Nicht mit Server verbunden"
        '
        'BtnStatusLabel
        '
        Me.BtnStatusLabel.Name = "BtnStatusLabel"
        Me.BtnStatusLabel.Size = New System.Drawing.Size(70, 17)
        Me.BtnStatusLabel.Text = "Knopfstatus"
        '
        'Button1
        '
        Me.Button1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(359, 190)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Test"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'SerialTimer
        '
        Me.SerialTimer.Interval = 10000
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Name = "Form1"
        Me.Text = "Open Mitarbeiteralarm"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents MQTTStatusLabel As ToolStripStatusLabel
    Friend WithEvents Button1 As Button
    Friend WithEvents SerialTimer As Timer
    Friend WithEvents SerialPort1 As IO.Ports.SerialPort
    Friend WithEvents BtnStatusLabel As ToolStripStatusLabel
End Class
