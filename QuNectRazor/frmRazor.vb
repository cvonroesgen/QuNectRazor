Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data.Odbc
Imports System.Text.RegularExpressions
Imports System.Threading
Public Class frmRazor
    Private Const AppName = "QuNectRazor"
    Private cmdLineArgs() As String
    Private automode As Boolean = False
    Private connectionString As String = ""
    Private appdbid As String = ""
    Private qdbAppName As String = ""
    Private Class qdbVersion
        Public year As Integer
        Public major As Integer
        Public minor As Integer
    End Class
    Private qdbVer As qdbVersion = New qdbVersion
    Private fieldCharacterSettings = New ArrayList

    Private Class tests
        Public Const parseUTF8 = "Check for UTF-8 parsing errors"
        Public Const dupes = "Check for Duplicate Column Names"
        Public Const findParsingProblems = "Check for SQL Parsing Errors due to Column Names"
        Public Const empty = "Find Empty Columns"
        Public Const fields = "List non Report Link/Formula URL fields"
    End Class
    Private Sub razor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tvFields.Visible = False

        txtUsername.Text = GetSetting(AppName, "Credentials", "username")
        cmbPassword.SelectedIndex = CInt(GetSetting(AppName, "Credentials", "passwordOrToken", "0"))
        txtPassword.Text = GetSetting(AppName, "Credentials", "password")
        txtServer.Text = GetSetting(AppName, "Credentials", "server", "")
        txtAppToken.Text = GetSetting(AppName, "Credentials", "apptoken", "")
        Dim detectProxySetting As String = GetSetting(AppName, "Credentials", "detectproxysettings", "0")
        If detectProxySetting = "1" Then
            ckbDetectProxy.Checked = True
        Else
            ckbDetectProxy.Checked = False
        End If



        Dim myBuildInfo As FileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath)
        Me.Text = "QuNect Razor " & myBuildInfo.ProductVersion

        fieldCharacterSettings.Add("all")
        fieldCharacterSettings.Add("all characters")
        fieldCharacterSettings.Add("msaccess")
        fieldCharacterSettings.Add("ms access allowed characters")
        fieldCharacterSettings.Add("lnuhs")
        fieldCharacterSettings.Add("letters numbers underscores hyphens spaces")
        fieldCharacterSettings.Add("lnus")
        fieldCharacterSettings.Add("letters numbers underscores spaces")
        fieldCharacterSettings.Add("lnu")
        fieldCharacterSettings.Add("letters numbers underscores")
        fieldCharacterSettings.Add("lnunc")
        fieldCharacterSettings.Add("letters numbers underscores no colons")
        cmbTests.Items.Add("Choose an Analysis")
        cmbTests.Items.Add(tests.parseUTF8)
        cmbTests.Items.Add(tests.dupes)
        cmbTests.Items.Add(tests.findParsingProblems)
        cmbTests.Items.Add(tests.empty)
        cmbTests.Items.Add(tests.fields)
        cmbTests.SelectedIndex = 0
        cmbTables.Items.Add("Please choose a table.")
        If txtUsername.Text = "" Then
            Exit Sub
        End If

        listTables()
    End Sub

    Private Sub txtUsername_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtUsername.TextChanged
        SaveSetting(AppName, "Credentials", "username", txtUsername.Text)
        showHideControls()
    End Sub

    Private Sub txtPassword_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPassword.TextChanged
        SaveSetting(AppName, "Credentials", "password", txtPassword.Text)
        showHideControls()
    End Sub


    Private Sub listTables()
        Me.Cursor = Cursors.WaitCursor
        qdbAppName = ""
        appdbid = ""

        Dim quNectConn As OdbcConnection = getquNectConn("all")
        If Not quNectConn Is Nothing Then
            cmbTables.Visible = True
            Dim tables As DataTable = quNectConn.GetSchema("Tables")
            listTablesFromGetSchema(tables)
            quNectConn.Dispose()
        End If
        Me.Cursor = Cursors.Default
    End Sub
    Private Function getquNectConn(fieldCharacterSetting As String) As OdbcConnection
        Dim connectionString As String = ""
        Try
            connectionString = buildConnectionString(fieldCharacterSetting)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, AppName)
            Return Nothing
        End Try
        Dim quNectConn As OdbcConnection = New OdbcConnection(connectionString)
        Try
            quNectConn.Open()
        Catch excpt As Exception
            Me.Cursor = Cursors.Default
            If excpt.Message.StartsWith("ERROR [IM003]") Or excpt.Message.Contains("Data source name not found") Then
                MsgBox("Please install QuNect ODBC for QuickBase from http://qunect.com/download/QuNect.exe and try again.", MsgBoxStyle.OkOnly, AppName)
            Else
                MsgBox(excpt.Message.Substring(13), MsgBoxStyle.OkOnly, AppName)
            End If
            Return Nothing
            Exit Function
        End Try

        Dim ver As String = quNectConn.ServerVersion
        Dim m As Match = Regex.Match(ver, "\d+\.(\d+)\.(\d+)\.(\d+)")
        qdbVer.year = CInt(m.Groups(1).Value)
        qdbVer.major = CInt(m.Groups(2).Value)
        qdbVer.minor = CInt(m.Groups(3).Value)
        If (qdbVer.year < 15) Or ((qdbVer.year = 15) And ((qdbVer.major <= 5) And (qdbVer.minor < 78))) Then
            MsgBox("You are running the " & ver & " version of QuNect ODBC for QuickBase. Please install the latest version from http://qunect.com/download/QuNect.exe", MsgBoxStyle.OkOnly, AppName)
            quNectConn.Dispose()
            Me.Cursor = Cursors.Default
            Return Nothing
            Exit Function
        End If
        Return quNectConn
    End Function
    Sub timeoutCallback(ByVal result As System.IAsyncResult)
        If Not automode Then
            Me.Cursor = Cursors.Default
            MsgBox("Operation timed out. Please try again.", MsgBoxStyle.OkOnly, AppName)
        End If
    End Sub
    Sub listTablesFromGetSchema(tables As DataTable)

        Dim dbids As String = GetSetting(AppName, "razor", "dbids")
        Dim dbidArray As New ArrayList
        dbidArray.AddRange(dbids.Split(";"c))
        Dim i As Integer
        Dim dbidCollection As New Collection
        For i = 0 To dbidArray.Count - 1
            Try
                dbidCollection.Add(dbidArray(i), dbidArray(i))
            Catch excpt As Exception
                'ignore dupes
            End Try
        Next

        cmbTables.BeginUpdate()
        cmbTables.Items.Clear()
        cmbTables.Items.Add("Please choose a table.")
        Dim dbName As String
        Dim applicationName As String = ""
        Dim prevAppName As String = ""
        Dim dbid As String
        pb.Value = 0
        pb.Visible = True
        pb.Maximum = tables.Rows.Count
        Dim getDBIDfromdbName As New Regex("([a-z0-9~]+)$")

        For i = 0 To tables.Rows.Count - 1
            pb.Value = i
            Application.DoEvents()
            dbName = tables.Rows(i)(2)
            applicationName = tables.Rows(i)(0)
            Dim dbidMatch As Match = getDBIDfromdbName.Match(dbName)
            dbid = dbidMatch.Value

            Dim tableName As String = dbName
            If appdbid.Length = 0 And dbName.Length > applicationName.Length Then
                tableName = dbName.Substring(applicationName.Length + 2)
            End If
            cmbTables.Items.Add(tableName)
        Next
        pb.Visible = False
        cmbTables.EndUpdate()
        pb.Value = 0
        cmbTables.SelectedIndex = 0
        Me.Cursor = Cursors.Default
    End Sub
    Sub showHideControls()
        cmbPassword.Visible = txtUsername.Text.Length > 0
        txtPassword.Visible = cmbPassword.Visible And cmbPassword.SelectedIndex <> 0
        txtServer.Visible = txtPassword.Visible And txtPassword.Text.Length > 0
        lblServer.Visible = txtServer.Visible
        lblAppToken.Visible = cmbPassword.Visible And cmbPassword.SelectedIndex = 1
        btnAppToken.Visible = lblAppToken.Visible
        txtAppToken.Visible = lblAppToken.Visible
        btnUserToken.Visible = cmbPassword.Visible And cmbPassword.SelectedIndex = 2
    End Sub

    Private Sub txtServer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtServer.TextChanged
        SaveSetting(AppName, "Credentials", "server", txtServer.Text)
        showHideControls()
    End Sub


    Private Sub btnRazor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        razor()
    End Sub
    Private Function buildConnectionString(fieldNameCharacters As String) As String
        buildConnectionString = "FIELDNAMECHARACTERS=" & fieldNameCharacters & ";uid=" & txtUsername.Text
        buildConnectionString &= ";pwd=" & txtPassword.Text
        buildConnectionString &= ";driver={QuNect ODBC for QuickBase};"
        buildConnectionString &= ";quickbaseserver=" & txtServer.Text
        buildConnectionString &= ";APPTOKEN=" & txtAppToken.Text
        If ckbDetectProxy.Checked Then
            buildConnectionString &= ";DETECTPROXY=1"
        End If

        If appdbid.Length Then
            buildConnectionString &= ";APPID=" & appdbid & ";APPNAME=" & qdbAppName
        End If
        If txtPassword.Text <> "" And cmbPassword.SelectedIndex = 0 Then
            cmbPassword.Focus()
            Throw New System.Exception("Please indicate whether you are using a password or a user token.")
            Return ""
        ElseIf cmbPassword.SelectedIndex = 1 Then
            buildConnectionString &= ";PWDISPASSWORD=1"
        Else
            buildConnectionString &= ";PWDISPASSWORD=0"
        End If
    End Function
    Private Sub razor()
        'here we need to go through the list and razor
        Dim connectionString As String = ""
        Try
            connectionString = buildConnectionString("all")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, AppName)
            Exit Sub
        End Try
        Me.Cursor = Cursors.WaitCursor
        Dim quNectConnFIDs As OdbcConnection = New OdbcConnection(connectionString & ";usefids=1")

        Try
            quNectConnFIDs.Open()
        Catch excpt As Exception
            If Not automode Then
                MsgBox(excpt.Message(), MsgBoxStyle.OkOnly, AppName)
            End If
            quNectConnFIDs.Dispose()
            Me.Cursor = Cursors.Default
            Exit Sub
        End Try

        Dim quNectConn As OdbcConnection = New OdbcConnection(connectionString)
        Try
            quNectConn.Open()
        Catch excpt As Exception
            If Not automode Then
                MsgBox(excpt.Message(), MsgBoxStyle.OkOnly, AppName)
            End If
            quNectConn.Dispose()
            Me.Cursor = Cursors.Default
            Exit Sub
        End Try


        quNectConn.Close()
        quNectConn.Dispose()
        quNectConnFIDs.Close()
        quNectConnFIDs.Dispose()
        Me.Cursor = Cursors.Default
    End Sub
    Private Function razorTable(ByVal dbName As String, ByVal dbid As String, ByVal quNectConn As OdbcConnection, ByVal quNectConnFIDs As OdbcConnection) As DialogResult
        'we need to get the schema of the table
        Dim restrictions(2) As String
        restrictions(2) = dbid
        Dim columns As DataTable = quNectConn.GetSchema("Columns", restrictions)
        'now we can look for formula fileURL fields
        razorTable = DialogResult.OK
        Dim quickBaseSQL As String = "select * from """ & dbid & """"

        Dim quNectCmd As OdbcCommand = New OdbcCommand(quickBaseSQL, quNectConnFIDs)
        Dim dr As OdbcDataReader
        Try
            dr = quNectCmd.ExecuteReader()
        Catch excpt As Exception
            If Not automode Then
                razorTable = MsgBox("Could not get field identifiers for table " & dbid & " because " & excpt.Message() & vbCrLf & "Would you like to continue?", MsgBoxStyle.OkCancel, AppName)
            End If
            quNectCmd.Dispose()
            Exit Function
        End Try
        If Not dr.HasRows Then
            Exit Function
        End If
        Dim i
        Dim clist As String = ""
        Dim period As String = ""
        For i = 0 To dr.FieldCount - 1
            clist &= period & dr.GetName(i).Replace("fid", "")
            period = "."
        Next
        dr.Close()
        quNectCmd.Dispose()
        quNectCmd = New OdbcCommand(quickBaseSQL, quNectConn)
        Try
            dr = quNectCmd.ExecuteReader()
        Catch excpt As Exception
            quNectCmd.Dispose()
            Exit Function
        End Try
        If Not dr.HasRows Then
            Exit Function
        End If

        dr.Close()
        quNectCmd.Dispose()
    End Function
    Private Function ChangeCharacter(s As String, replaceWith As Char, idx As Integer) As String
        Dim sb As New StringBuilder(s)
        sb(idx) = replaceWith
        Return sb.ToString()
    End Function

    Private Function UrlDecode(text As String) As String
        text = text.Replace("+", " ")
        Return System.Uri.UnescapeDataString(text)
    End Function
    Private Sub txtAppToken_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAppToken.TextChanged
        SaveSetting(AppName, "Credentials", "apptoken", txtAppToken.Text)
        showHideControls()
    End Sub
    Private Sub ckbDetectProxy_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ckbDetectProxy.CheckStateChanged
        If ckbDetectProxy.Checked Then
            SaveSetting(AppName, "Credentials", "detectproxysettings", "1")
        Else
            SaveSetting(AppName, "Credentials", "detectproxysettings", "0")
        End If
        showHideControls()
    End Sub
    Private Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        If cmbTables.Items.Count = 0 Then
            listTables()
            MsgBox("Please Choose a Table", MsgBoxStyle.OkOnly, AppName)
            Exit Sub
        End If
        txtResult.Visible = False
        txtResult.Text = ""
        tvFields.Visible = False
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Try
            If cmbTables.SelectedIndex = 0 Then
                MsgBox("Please Choose a Table", MsgBoxStyle.OkOnly, AppName)
            ElseIf cmbTests.Text = tests.parseUTF8 Then
                txtResult.Visible = True
                findUTF8ParseErrors()
            ElseIf cmbTests.Text = tests.dupes Then
                txtResult.Visible = True
                findDupeColumnNames()
            ElseIf cmbTests.Text = tests.findParsingProblems Then
                txtResult.Visible = True
                findParsingProblems()
            ElseIf cmbTests.Text = tests.empty Then
                txtResult.Visible = True
                findEmptyFields()
            ElseIf cmbTests.Text = tests.fields Then
                tvFields.Visible = True
                listFields()
            Else
                MsgBox("Please Choose an Analysis", MsgBoxStyle.OkOnly, AppName)
            End If
        Catch ex As Exception
            MsgBox("Sorry we could not perform your analysis because: " & ex.Message, MsgBoxStyle.OkOnly, AppName)
        End Try
        Me.Cursor = Cursors.Default
        cmbTables.Focus()
    End Sub
    Private Sub findEmptyFields()
        Dim colArray As New ArrayList
        Dim quNectConn As OdbcConnection = getquNectConn("all;IGNOREDUPEFIELDNAMES=1;TEXTNULL=1")
        Dim rowcount = 0
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)
        If Not quNectConn Is Nothing Then
            Dim restrictions(3) As String
            restrictions(2) = dbid 'catalog, owner, table, column
            Dim columns = quNectConn.GetSchema("Columns", restrictions)
            Dim findBooleans As Regex = New Regex("Checkbox", RegexOptions.IgnoreCase)
            Dim quickBaseSQL As String = "SELECT count(1) from [" & dbid & "]"
            Dim quNectCmd = New OdbcCommand(quickBaseSQL, quNectConn)
            Try
                Dim dr = quNectCmd.ExecuteReader()
                dr.Read()
                rowcount = dr.GetValue(0)
                If rowcount = 0 Then
                    txtResult.Text = "There are no rows in " & cmbTables.Items(cmbTables.SelectedIndex)
                    Exit Sub
                End If
            Catch excpt As Exception
                quNectCmd.Dispose()
                Exit Sub
            End Try
            For i = 0 To columns.Rows.Count - 1
                Application.DoEvents()
                Dim ColumnName As String = columns.Rows(i)("COLUMN_NAME") 'index 3 for column name, index 11 for remarks has field type and then the fid
                Dim remarks As String = columns.Rows(i)("REMARKS")
                Dim whereClause = " IS NOT NULL"
                Dim isCheckBox = findBooleans.Match(remarks).Success
                If isCheckBox Then
                    whereClause = " = 1"
                End If
                quNectCmd = New OdbcCommand(quickBaseSQL & " WHERE [" & ColumnName & "]" & whereClause, quNectConn)
                Try
                    Dim dr = quNectCmd.ExecuteReader()
                    dr.Read()
                    If dr.GetValue(0) = 0 Then
                        colArray.Add(ColumnName & " is empty. " & columns.Rows(i)("REMARKS"))
                    End If
                Catch excpt As Exception
                    quNectCmd.Dispose()
                    Exit Sub
                End Try
            Next
            quNectConn.Dispose()
        End If
        Dim message As String = ""
        For Each col In colArray
            message &= vbCrLf & col
        Next
        txtResult.Text = "Out of the " & rowcount & " rows you have access to, the following columns have no data." & vbCrLf & message
    End Sub
    Private Sub findUTF8ParseErrors()
        txtResult.Text = ""
        Dim colDictionary As New Dictionary(Of String, String)
        Dim fidArray As New ArrayList
        Dim quNectConn As OdbcConnection = getquNectConn("all;IGNOREDUPEFIELDNAMES=1;PARSEUTF8=1")
        Dim tableName As String = cmbTables.Items(cmbTables.SelectedIndex)
        Dim dbid = tableName.Split(" ")(tableName.Split(" ").Length - 1)
        If Not quNectConn Is Nothing Then
            Dim restrictions(3) As String
            restrictions(2) = dbid 'catalog, owner, table, column
            Dim columns = quNectConn.GetSchema("Columns", restrictions)
            Dim findTextFields As Regex = New Regex("Text (fid\d+)", RegexOptions.IgnoreCase)
            For i = 0 To columns.Rows.Count - 1
                Application.DoEvents()
                Dim remarks As String = columns.Rows(i)("REMARKS")
                Dim isTextField = findTextFields.Match(remarks)
                If isTextField.Success Then
                    Dim strFID = isTextField.Groups(1).Value
                    colDictionary.Add(strFID, columns.Rows(i)("COLUMN_NAME"))
                    fidArray.Add(strFID)
                End If
            Next
        End If
        If fidArray.Count = 0 Then
            txtResult.Text = "There are no text fields in this table."
            quNectConn.Dispose()
            Exit Sub
        End If
        Dim selectFieldList As String = ""
        Dim comma As String = ""
        For Each fid In fidArray
            selectFieldList &= comma & fid
            comma = ","
        Next
        Dim quickBaseSQL As String = "SELECT " & selectFieldList & " from [" & dbid & "]"
        Dim quNectCmd = New OdbcCommand(quickBaseSQL, quNectConn)
        Try
            Dim dr = quNectCmd.ExecuteReader()
            While dr.Read()
                dr.GetString(0)
            End While
        Catch excpt As Exception

            Dim findDBIDridFID = New Regex("dbid:(\w+)\s+rid:(\d+)\s+fid:(\d+)", RegexOptions.IgnoreCase)
            Dim DBIDridFID = findDBIDridFID.Match(excpt.Message)
            If DBIDridFID.Success Then
                Dim rid = DBIDridFID.Groups(2).Value
                Dim fid = DBIDridFID.Groups(3).Value
                txtResult.Text = "In table '" & tableName & "', within record " & rid & ", within field '" & colDictionary("fid" & fid) & "' whose field identifier (fid) is " & fid & ", one or more characters are not UTF-8 compliant."
                Process.Start("https://" & txtServer.Text & "/db/" & DBIDridFID.Groups(1).Value & "?a=q&query={'3'.EX.'" & rid & "'}&clist=" & fid)
            Else
                txtResult.Text = excpt.Message
            End If

            quNectCmd.Dispose()
            quNectConn.Dispose()
            Exit Sub
        End Try
        quNectCmd.Dispose()
        quNectConn.Dispose()
        txtResult.Text = "No UTF-8 parsing errors"
    End Sub

    Private Sub listFields()
        Me.Cursor = Cursors.WaitCursor
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)

        Dim columnNames As New ArrayList
        getAllTypesOfColumnNames(columnNames)
        Dim quNectConn As OdbcConnection = getquNectConn("all;IGNOREDUPEFIELDNAMES=1;TEXTNULL=1")
        Dim rowcount = 0
        tvFields.BeginUpdate()
        tvFields.Nodes.Clear()
        If Not quNectConn Is Nothing Then
            Dim restrictions(3) As String
            restrictions(2) = dbid 'catalog, owner, table, column
            Dim columns = quNectConn.GetSchema("Columns", restrictions)
            For i = 0 To columns.Rows.Count - 1
                Application.DoEvents()
                Dim ColumnName As String = columns.Rows(i)("COLUMN_NAME") 'index 3 for column name, index 11 for remarks has field type and then the fid
                Dim fieldNode As TreeNode = tvFields.Nodes.Add(ColumnName)
                fieldNode.Nodes.Add(columns.Rows(i)("REMARKS"))
                For j As Integer = 0 To fieldCharacterSettings.Count - 1 Step 2
                    fieldNode.Nodes.Add(columnNames.Item(j / 2).Rows(i)("COLUMN_NAME") & " (" & fieldCharacterSettings(j + 1) & ")")
                Next
            Next
            quNectConn.Dispose()
        End If
        tvFields.EndUpdate()
        Me.Cursor = Cursors.Default
    End Sub
    Private Function findSQLKeyWordsForFieldSetting(fieldCharaterSetting As String) As ArrayList
        Dim colArray As New ArrayList
        Dim quNectConn As OdbcConnection = getquNectConn(fieldCharaterSetting & ";IGNOREDUPEFIELDNAMES=1")
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)

        If Not quNectConn Is Nothing Then
            Dim restrictions(3) As String
            restrictions(2) = dbid 'catalog, owner, table, column
            Dim columns As DataTable = quNectConn.GetSchema("Columns", restrictions)
            If columns.Rows.Count = 0 Then
                If cmbPassword.SelectedIndex = 1 Then
                    Throw New Exception("Could not find any columns! Please check your application token.")
                Else
                    Throw New Exception("Could not find any columns! Please check your user token.")
                End If
            End If
            Dim ColumnsCausingParsingProblems = sendFieldNamesThroughParser(columns, quNectConn)

            For i = 0 To ColumnsCausingParsingProblems.Count - 1
                colArray.Add(ColumnsCausingParsingProblems(i) & " caused parsing problems.")
            Next
            quNectConn.Dispose()
        End If
        Return colArray
    End Function
    Private Function sendFieldNamesThroughParser(columns As DataTable, connection As OdbcConnection) As ArrayList
        Dim colArray As New ArrayList

        For i = 0 To columns.Rows.Count - 1
            Application.DoEvents()
            Dim ColumnName As String = columns.Rows(i)("COLUMN_NAME") 'index 3 for column name, index 11 for remarks has field type and then the fid
            colArray.Add(ColumnName)
        Next i
        'need to build a SQL SELECT from the column names
        Return sendArrayOfFieldNamesToParser(colArray, connection)
    End Function
    Private Function sendArrayOfFieldNamesToParser(columns As ArrayList, connection As OdbcConnection) As ArrayList
        Dim Sql As String = "SELECT "
        Dim comma As String = ""
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)

        For i = 0 To columns.Count - 1
            Sql &= comma & columns(i)
            comma = ","
        Next
        Sql &= " FROM """ & dbid & """"
        If checkSQL(Sql, connection) Then
            columns.Clear()
            Return columns
        ElseIf columns.Count = 1 Then
            Return columns
        Else

            Dim firstHalfColumns As New ArrayList
            Dim secondHalfColumns As New ArrayList
            For i = 0 To columns.Count - 1
                If i < columns.Count / 2 Then
                    firstHalfColumns.Add(columns(i))
                Else
                    secondHalfColumns.Add(columns(i))
                End If
            Next
            Dim firstBadColumns As ArrayList = sendArrayOfFieldNamesToParser(firstHalfColumns, connection)
            Dim secondtBadColumns As ArrayList = sendArrayOfFieldNamesToParser(secondHalfColumns, connection)
            For i = 0 To secondtBadColumns.Count - 1
                firstBadColumns.Add(secondtBadColumns(i))
            Next i
            Return firstBadColumns
        End If
    End Function
    Private Sub findParsingProblems()
        Dim colArray As ArrayList = findSQLKeyWordsForFieldSetting("lnu")
        Dim message As String = "Field names causing parsing errors when spaces are not allowed." & vbCrLf
        If colArray.Count = 0 Then
            message = "No field names cause parsing errors when spaces are not allowed." & vbCrLf
        Else
            For Each col In colArray
                message &= col & vbCrLf
            Next
        End If
        colArray = findSQLKeyWordsForFieldSetting("all")
        If colArray.Count = 0 Then
            message &= "No field names cause parsing errors when spaces are allowed." & vbCrLf
        Else
            message &= "Field names causing parsing problems when spaces are allowed." & vbCrLf
            For Each col In colArray
                message &= col & vbCrLf
            Next
        End If
        txtResult.Text = message
    End Sub
    Private Sub findDupeColumnNames()
        Dim message As String = ""
        Dim comma As String = vbCrLf & vbTab
        Dim columns As DataTable = Nothing
        Dim allCharacterColumns As DataTable = Nothing
        For i As Integer = 0 To fieldCharacterSettings.Count - 1 Step 2
            Dim dupeColumns As ArrayList = findDupeColumnNamesForFieldSetting(fieldCharacterSettings(i), fieldCharacterSettings(i + 1), columns, allCharacterColumns)
            Dim dupeColumnString As String = ""
            For Each dupeColumn In dupeColumns
                dupeColumnString &= comma & dupeColumn
            Next
            If dupeColumnString <> "" Then
                message &= fieldCharacterSettings(i + 1) & ": " & dupeColumnString & vbCrLf & vbCrLf
            Else
                message &= fieldCharacterSettings(i + 1) & ":" & vbCrLf & vbTab & "No Duplicates" & vbCrLf & vbCrLf
            End If
        Next
        txtResult.Text = "Duplicate field names are listed below for each field character replacement setting." & vbCrLf & vbCrLf & message
    End Sub
    Private Function findDupeColumnNamesForFieldSetting(fieldCharacterSetting As String, fieldCharacterSettingDescription As String, ByRef columns As DataTable, ByRef allCharacterColumns As DataTable) As ArrayList
        Dim quNectConn As OdbcConnection = getquNectConn(fieldCharacterSetting & ";IGNOREDUPEFIELDNAMES=1")
        Dim colDictionary = New Dictionary(Of String, Boolean)
        Dim colArray As New ArrayList
        Dim checkingUnalteredNames = False
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)

        If Not quNectConn Is Nothing Then
            Dim restrictions(3) As String
            restrictions(2) = dbid 'catalog, owner, table, column
            columns = quNectConn.GetSchema("Columns", restrictions)
            If allCharacterColumns Is Nothing Then
                allCharacterColumns = columns
                checkingUnalteredNames = True
            End If
            Dim colUnique As New Dictionary(Of String, Boolean)
            For i = 0 To columns.Rows.Count - 1
                Application.DoEvents()
                Dim ColumnName As String = columns.Rows(i)("COLUMN_NAME") 'index 3 for column name, index 11 for remarks has field type and then the fid              
                If colUnique.ContainsKey(ColumnName) Then
                    If Not colDictionary.ContainsKey(ColumnName) Then
                        colDictionary.Add(ColumnName, True)
                    End If
                Else
                    colUnique.Add(ColumnName, True)
                End If
            Next
            If colDictionary.Count Then
                For i = 0 To columns.Rows.Count - 1
                    Application.DoEvents()
                    Dim ColumnName As String = columns.Rows(i)("COLUMN_NAME") 'index 3 for column name, index 11 for remarks has field type and then the fid
                    If colDictionary.ContainsKey(ColumnName) Then
                        colArray.Add(allCharacterColumns.Rows(i)("COLUMN_NAME") & " becomes " & ColumnName & " " & columns.Rows(i)("REMARKS"))
                    End If
                Next
            End If
            quNectConn.Dispose()
        End If
        Return colArray
    End Function
    Private Sub getAllTypesOfColumnNames(ColumnTables As ArrayList)

        For i As Integer = 0 To fieldCharacterSettings.Count - 1 Step 2
            ColumnTables.Add(getColumnNamesForFieldSetting(fieldCharacterSettings(i)))
        Next

    End Sub
    Private Function getColumnNamesForFieldSetting(fieldCharacterSetting As String) As DataTable
        Dim quNectConn As OdbcConnection = getquNectConn(fieldCharacterSetting & ";IGNOREDUPEFIELDNAMES=1")
        Dim dbid As String = cmbTables.Items(cmbTables.SelectedIndex)
        dbid = dbid.Split(" ")(dbid.Split(" ").Length - 1)

        Dim restrictions(3) As String
        restrictions(2) = dbid 'catalog, owner, table, column
        getColumnNamesForFieldSetting = quNectConn.GetSchema("Columns", restrictions)
        quNectConn.Dispose()

    End Function
    Private Sub cmbPassword_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPassword.SelectedIndexChanged
        SaveSetting(AppName, "Credentials", "passwordOrToken", cmbPassword.SelectedIndex)
        showHideControls()
    End Sub


    Function checkSQL(Sql As String, Connection As OdbcConnection) As Boolean
        Try
            Using command As OdbcCommand = New OdbcCommand(Sql, Connection)
                Try
                    command.Prepare()
                Catch e As Exception
                    Throw New Exception("SQL parse error: " & Sql)
                Finally
                    command.Dispose()
                End Try
            End Using
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub btnAppToken_Click(sender As Object, e As EventArgs) Handles btnAppToken.Click
        Process.Start("https://help.quickbase.com/user-assistance/app_tokens.html")
    End Sub
    Private Sub btnUserToken_Click(sender As Object, e As EventArgs) Handles btnUserToken.Click
        Process.Start("https://qunect.com/flash/UserToken.html")
    End Sub

    Private Sub frmRazor_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        txtResult.Height = Me.ClientSize.Height - txtResult.Top - txtResult.Left
        txtResult.Width = Me.ClientSize.Width - txtResult.Left - txtResult.Left
        tvFields.Height = Me.ClientSize.Height - tvFields.Top - tvFields.Left
        tvFields.Width = Me.ClientSize.Width - tvFields.Left - tvFields.Left
    End Sub

    Private Sub cmbTables_Click(sender As Object, e As EventArgs) Handles cmbTables.Click
        If cmbTables.Items.Count <= 1 Then
            listTables()
        End If
    End Sub
End Class

