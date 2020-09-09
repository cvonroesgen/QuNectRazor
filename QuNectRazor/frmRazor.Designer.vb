<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRazor
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRazor))
        Me.pb = New System.Windows.Forms.ProgressBar()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ckbDetectProxy = New System.Windows.Forms.CheckBox()
        Me.RetrieveTheTableReportsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.lblAppToken = New System.Windows.Forms.Label()
        Me.txtAppToken = New System.Windows.Forms.TextBox()
        Me.lblServer = New System.Windows.Forms.Label()
        Me.txtServer = New System.Windows.Forms.TextBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.txtUsername = New System.Windows.Forms.TextBox()
        Me.btnAnalyze = New System.Windows.Forms.Button()
        Me.cmbTests = New System.Windows.Forms.ComboBox()
        Me.tvFields = New System.Windows.Forms.TreeView()
        Me.cmbPassword = New System.Windows.Forms.ComboBox()
        Me.btnAppToken = New System.Windows.Forms.Button()
        Me.btnUserToken = New System.Windows.Forms.Button()
        Me.txtResult = New System.Windows.Forms.TextBox()
        Me.cmbTables = New System.Windows.Forms.ComboBox()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pb
        '
        Me.pb.Location = New System.Drawing.Point(284, 81)
        Me.pb.Maximum = 1000
        Me.pb.Name = "pb"
        Me.pb.Size = New System.Drawing.Size(511, 23)
        Me.pb.TabIndex = 47
        Me.pb.Visible = False
        '
        'ckbDetectProxy
        '
        Me.ckbDetectProxy.AutoSize = True
        Me.ckbDetectProxy.Location = New System.Drawing.Point(633, 31)
        Me.ckbDetectProxy.Name = "ckbDetectProxy"
        Me.ckbDetectProxy.Size = New System.Drawing.Size(188, 17)
        Me.ckbDetectProxy.TabIndex = 48
        Me.ckbDetectProxy.Text = "Automatically detect proxy settings"
        Me.ckbDetectProxy.UseVisualStyleBackColor = True
        '
        'RetrieveTheTableReportsToolStripMenuItem
        '
        Me.RetrieveTheTableReportsToolStripMenuItem.Name = "RetrieveTheTableReportsToolStripMenuItem"
        Me.RetrieveTheTableReportsToolStripMenuItem.Size = New System.Drawing.Size(354, 22)
        Me.RetrieveTheTableReportsToolStripMenuItem.Text = "Retrieve the table reports for the selected application."
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RetrieveTheTableReportsToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(355, 26)
        '
        'lblAppToken
        '
        Me.lblAppToken.AutoSize = True
        Me.lblAppToken.Location = New System.Drawing.Point(13, 65)
        Me.lblAppToken.Name = "lblAppToken"
        Me.lblAppToken.Size = New System.Drawing.Size(148, 13)
        Me.lblAppToken.TabIndex = 32
        Me.lblAppToken.Text = "QuickBase Application Token"
        '
        'txtAppToken
        '
        Me.txtAppToken.Location = New System.Drawing.Point(10, 84)
        Me.txtAppToken.Name = "txtAppToken"
        Me.txtAppToken.Size = New System.Drawing.Size(258, 20)
        Me.txtAppToken.TabIndex = 31
        '
        'lblServer
        '
        Me.lblServer.AutoSize = True
        Me.lblServer.Location = New System.Drawing.Point(404, 12)
        Me.lblServer.Name = "lblServer"
        Me.lblServer.Size = New System.Drawing.Size(93, 13)
        Me.lblServer.TabIndex = 30
        Me.lblServer.Text = "QuickBase Server"
        '
        'txtServer
        '
        Me.txtServer.Location = New System.Drawing.Point(407, 31)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(217, 20)
        Me.txtServer.TabIndex = 29
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(193, 31)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(187, 20)
        Me.txtPassword.TabIndex = 27
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Location = New System.Drawing.Point(13, 12)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(110, 13)
        Me.lblUsername.TabIndex = 26
        Me.lblUsername.Text = "QuickBase Username"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(10, 31)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(177, 20)
        Me.txtUsername.TabIndex = 25
        '
        'btnAnalyze
        '
        Me.btnAnalyze.Location = New System.Drawing.Point(756, 114)
        Me.btnAnalyze.Name = "btnAnalyze"
        Me.btnAnalyze.Size = New System.Drawing.Size(65, 22)
        Me.btnAnalyze.TabIndex = 50
        Me.btnAnalyze.Text = "Analyze"
        Me.btnAnalyze.UseVisualStyleBackColor = True
        '
        'cmbTests
        '
        Me.cmbTests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTests.FormattingEnabled = True
        Me.cmbTests.Location = New System.Drawing.Point(494, 115)
        Me.cmbTests.Name = "cmbTests"
        Me.cmbTests.Size = New System.Drawing.Size(256, 21)
        Me.cmbTests.TabIndex = 51
        '
        'tvFields
        '
        Me.tvFields.Location = New System.Drawing.Point(10, 142)
        Me.tvFields.Name = "tvFields"
        Me.tvFields.Size = New System.Drawing.Size(809, 647)
        Me.tvFields.TabIndex = 53
        '
        'cmbPassword
        '
        Me.cmbPassword.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPassword.FormattingEnabled = True
        Me.cmbPassword.Items.AddRange(New Object() {"Please choose...", "QuickBase Password", "QuickBase User Token"})
        Me.cmbPassword.Location = New System.Drawing.Point(193, 4)
        Me.cmbPassword.Name = "cmbPassword"
        Me.cmbPassword.Size = New System.Drawing.Size(187, 21)
        Me.cmbPassword.TabIndex = 77
        '
        'btnAppToken
        '
        Me.btnAppToken.Location = New System.Drawing.Point(163, 59)
        Me.btnAppToken.Name = "btnAppToken"
        Me.btnAppToken.Size = New System.Drawing.Size(19, 20)
        Me.btnAppToken.TabIndex = 78
        Me.btnAppToken.Text = "?"
        Me.btnAppToken.UseVisualStyleBackColor = True
        '
        'btnUserToken
        '
        Me.btnUserToken.Location = New System.Drawing.Point(381, 5)
        Me.btnUserToken.Name = "btnUserToken"
        Me.btnUserToken.Size = New System.Drawing.Size(19, 20)
        Me.btnUserToken.TabIndex = 79
        Me.btnUserToken.Text = "?"
        Me.btnUserToken.UseVisualStyleBackColor = True
        '
        'txtResult
        '
        Me.txtResult.Location = New System.Drawing.Point(10, 142)
        Me.txtResult.MaxLength = 0
        Me.txtResult.Multiline = True
        Me.txtResult.Name = "txtResult"
        Me.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtResult.Size = New System.Drawing.Size(811, 644)
        Me.txtResult.TabIndex = 80
        '
        'cmbTables
        '
        Me.cmbTables.FormattingEnabled = True
        Me.cmbTables.Location = New System.Drawing.Point(10, 114)
        Me.cmbTables.Name = "cmbTables"
        Me.cmbTables.Size = New System.Drawing.Size(478, 21)
        Me.cmbTables.TabIndex = 82
        '
        'frmRazor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(831, 801)
        Me.Controls.Add(Me.tvFields)
        Me.Controls.Add(Me.cmbTables)
        Me.Controls.Add(Me.txtResult)
        Me.Controls.Add(Me.btnUserToken)
        Me.Controls.Add(Me.btnAppToken)
        Me.Controls.Add(Me.cmbPassword)
        Me.Controls.Add(Me.cmbTests)
        Me.Controls.Add(Me.btnAnalyze)
        Me.Controls.Add(Me.pb)
        Me.Controls.Add(Me.ckbDetectProxy)
        Me.Controls.Add(Me.lblAppToken)
        Me.Controls.Add(Me.txtAppToken)
        Me.Controls.Add(Me.lblServer)
        Me.Controls.Add(Me.txtServer)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.lblUsername)
        Me.Controls.Add(Me.txtUsername)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmRazor"
        Me.Text = "QuNect Razor"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout

End Sub
    Friend WithEvents pb As System.Windows.Forms.ProgressBar
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ckbDetectProxy As System.Windows.Forms.CheckBox
    Friend WithEvents RetrieveTheTableReportsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents lblAppToken As System.Windows.Forms.Label
    Friend WithEvents txtAppToken As System.Windows.Forms.TextBox
    Friend WithEvents lblServer As System.Windows.Forms.Label
    Friend WithEvents txtServer As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents txtUsername As System.Windows.Forms.TextBox
    Friend WithEvents btnAnalyze As System.Windows.Forms.Button
    Friend WithEvents cmbTests As System.Windows.Forms.ComboBox
    Friend WithEvents tvFields As TreeView
    Friend WithEvents cmbPassword As ComboBox
    Friend WithEvents btnAppToken As Button
    Friend WithEvents btnUserToken As Button
    Friend WithEvents txtResult As TextBox
    Friend WithEvents cmbTables As ComboBox
End Class
