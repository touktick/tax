
Imports System
Imports System.Data
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Configuration
Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Web.UI.WebControls.DataControlFieldCollection
Imports System.Web.UI.WebControls.GridView
Imports System.Web.UI.WebControls.GridViewRowCollection
Imports System.Web.UI.ClientScriptManager
Imports System.Web.UI.Control
Imports System.Web.UI.WebControls.ImageButton
Imports System.Web.UI.Page
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls


Partial Class Frm_User
    Inherits System.Web.UI.Page
    Dim cnstr As String = ""
    Dim cn As New SqlConnection
    Dim Acc_Code, ac_A, ac_B, AcTypeLao, AcTypeEng, Get_Code As String
    Dim i As Integer = 0
    Dim strSelectedVL As String
    Dim strSelectedVL1 As String
    Dim strSelectedVL2 As String
    Dim strSelectedVL3 As String
    Dim sqlDate As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            loadDate()
            LoadCMB1()
            Load_Grid()
            Total()
            Total_Bill()
            Total_Bill_Cancel()
        End If
    End Sub
    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        If CheckBox1.Checked = True Then
            Session("CheckBox1") = "1"
        Else
            Session("CheckBox1") = "0"
        End If
        Session("Catgory") = "0"
        Session("EditActive") = "0"
        Session("ID") = "0"
        Response.Redirect("frm_invoice.aspx")
    End Sub

    Private Sub loadDate()
        If Session("MWorkSetting") = "" Then
            txtFdate.Text = Format(Class1.Cl_time, "01/MM/yyyy")
            txtTdate.Text = Format(Class1.Cl_time, "dd/MM/yyyy")
        Else
            txtFdate.Text = Session("MWorkSetting")
            txtTdate.Text = Session("MWorkSetting")
        End If

    End Sub
    Private Sub LoadCMB1()
        Try
            Dim aa As String = ""
            Dim bb As String = ""
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            cmbTax.Items.Clear()
            cmbTax.Items.Add("ທັງໝົດ")
            cmbTax.Text = "ທັງໝົດ"
            aa = " Select * from AP_Tax_Rate order by taxid  "
            da = New SqlDataAdapter(aa, cn)
            da.Fill(ds, "CMBAA")
            If ds.Tables("CMBAA").Rows.Count > 0 Then
                With cmbTax
                    .DataSource = ds.Tables("CMBAA")
                    .DataBind()
                End With
            Else
                cmbTax.Items.Add("ທັງໝົດ")
                cmbTax.Text = "ທັງໝົດ"
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Load_GridView_Stock()
        Try
            Dim sql As String
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            sql = " select * from AP_PI_order order by Bill_no "
            da = New SqlDataAdapter(sql, cn)
            da.Fill(ds, "Stock_in_Order")
            With GridView1
                .DataSource = ds.Tables("Stock_in_Order")
                .DataBind()
            End With

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Protected Sub Button7_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button7.Click
        Load_Grid()
        Total()
        Total_Bill()
        Total_Bill_Cancel()
    End Sub
    Protected Sub Total_Bill_Cancel()

        Dim sql As String
        Dim IDS As String
        Dim aa As String
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With
        Dim Lock As String
        If ComboBox1.SelectedIndex = 0 Then
            Lock = ""
        ElseIf ComboBox1.SelectedIndex = 1 Then
            Lock = " AND IV.Lock=0 "
        Else
            Lock = " AND IV.Lock=1 "
        End If
        If TextBox2.Text <> "" Then
            IDS = " AND  IV.InvoiceNo LIKE N'%" & TextBox2.Text & "%'    "
        Else
            IDS = ""
        End If

        If cmbTax.SelectedIndex = 0 Then
            sql = ""
        Else
            sql = " AND Tax_Desc =N'" & Trim(cmbTax.Text) & "' "
        End If
        sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "



        TxtAccNo_Cancel.Text = "0"

        sql = ""
        sql = "SELECT    COUNT(*) as aa1  " & _
                    " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                    " where  1=1  and Lock=1  " & IDS & " " & sqlDate & "   " & sql & "  "
        da = New SqlDataAdapter(sql, cn)


        da.Fill(ds, "ABC")
        If ds.Tables("ABC").Rows.Count > 0 Then
            TxtAccNo_Cancel.Text = Format(ds.Tables("ABC").Rows(0).Item("aa1"), "#,##0")
        Else
            TxtAccNo_Cancel.Text = "0"
        End If

    End Sub
    Protected Sub Total_Bill()

        Dim sql As String
        Dim IDS As String
        Dim aa As String
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With
        Dim Lock As String
        If ComboBox1.SelectedIndex = 0 Then
            Lock = ""
        ElseIf ComboBox1.SelectedIndex = 1 Then
            Lock = " AND IV.Lock=0 "
        Else
            Lock = " AND IV.Lock=1 "
        End If
        If TextBox2.Text <> "" Then
            IDS = " AND  IV.InvoiceNo LIKE N'%" & TextBox2.Text & "%'    "
        Else
            IDS = ""
        End If

        If cmbTax.SelectedIndex = 0 Then
            sql = ""
        Else
            sql = " AND Tax_Desc =N'" & Trim(cmbTax.Text) & "' "
        End If
        sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "


        TxtAccNo.Text = "0"


        sql = ""
        sql = "SELECT    COUNT(*) as aa1  " & _
                    " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                    " where  1=1 " & IDS & " " & sqlDate & " " & Lock & " " & sql & "  "
        da = New SqlDataAdapter(sql, cn)


        da.Fill(ds, "ABC")
        If ds.Tables("ABC").Rows.Count > 0 Then
            TxtAccNo.Text = Format(ds.Tables("ABC").Rows(0).Item("aa1"), "#,##0")
        Else
            TxtAccNo.Text = "0"
        End If

    End Sub
    Protected Sub Total()

        Dim sql As String
        Dim IDS As String
        Dim aa As String
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With
        Dim Lock As String
        If ComboBox1.SelectedIndex = 0 Then
            Lock = ""
        ElseIf ComboBox1.SelectedIndex = 1 Then
            Lock = " AND IV.Lock=0 "
        Else
            Lock = " AND IV.Lock=1 "
        End If
        If TextBox2.Text <> "" Then
            IDS = " AND  IV.InvoiceNo LIKE N'%" & TextBox2.Text & "%'    "
        Else
            IDS = ""
        End If

        If cmbTax.SelectedIndex = 0 Then
            sql = ""
        Else
            sql = " AND Tax_Desc =N'" & Trim(cmbTax.Text) & "' "
        End If
        sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "


        TxtAccNo.Text = "0.00"
        TxtAccNo_Cancel.Text = "0.00"

        TxtAmt.Text = "0.00"
        TxtTax.Text = "0.00"
        'aa = "  SELECT  isnull(Sum(Acc_Bill),0) as Acc_Bill , isnull(Sum(Acc_Bill_Cancel),0) as Acc_Bill_Cancel , isnull(Sum(MATBILL),0) as MATBILL , isnull(Sum(TAXBILL),0) as TAXBILL FROM TEM_SERVER  where 1=1 " & MDACC & "  " & MDSecNM & "   " & MDDepNM & "  "
        'da = New SqlDataAdapter(aa, cn)
        sql = ""
        sql = "SELECT  isnull(Sum(AmountNet),0) as AmountNet, isnull(Sum(AmountTax),0) as AmountTax   " & _
                    " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                    " where  1=1 " & IDS & " " & sqlDate & " " & Lock & " " & sql & "  "
        da = New SqlDataAdapter(sql, cn)


        da.Fill(ds, "ABC")
        If ds.Tables("ABC").Rows.Count > 0 Then

            TxtAmt.Text = Format(ds.Tables("ABC").Rows(0).Item("AmountNet"), "#,##0.00")
            TxtTax.Text = Format(ds.Tables("ABC").Rows(0).Item("AmountTax"), "#,##0.00")

        Else

            TxtAmt.Text = "0.00"
            TxtTax.Text = "0.00"
        End If

    End Sub
    Private Sub Load_Grid()
        Try
            Dim sql As String
            Dim IDS As String
            Dim aa As String
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            Dim Lock As String
            If ComboBox1.SelectedIndex = 0 Then
                Lock = ""
            ElseIf ComboBox1.SelectedIndex = 1 Then
                Lock = " AND IV.Lock=0 "
            Else
                Lock = " AND IV.Lock=1 "
            End If
            If TextBox2.Text <> "" Then
                IDS = " AND  IV.InvoiceNo LIKE N'%" & TextBox2.Text & "%'    "
            Else
                IDS = ""
            End If

            'If cmd_Sang1.Text <> "" Then
            '    IDS3 = " AND  dbo.AP_Office.off_id = N'" & TextBox14.Text & "'   "
            'Else
            '    IDS3 = ""
            'End If
            If cmbTax.SelectedIndex = 0 Then
                sql = ""
            Else
                sql = " AND Tax_Desc =N'" & Trim(cmbTax.Text) & "' "
            End If
            sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "

            sql = ""
            sql = "SELECT IV.Lock, IV.InvoiceNo, IV.InvoiceDate, IV.DueDate, IV.JobNo, IV.PayerAccountID, IV.ItemCount, IV.AmountInvoice, IV.AmountTax, IV.AmountNet, IV.AmountDiscount, IV.PercentDiscount, IV.AmountTotal, IV.AmountPaid, IV.Currency,  " & _
                        " IV.InvoiceDescription, IV.IsFinished, IV.THB_LAK, IV.USD_LAK, IV.LastUpdate, IV.LastUser,  IV.AcRevenue, IV.AcTax, IV.Paid_Date, IV.AcAdv, IV.credit_term, IV.AmountLAK, IV.AcDebit, CM.PayerAccountNm, IV.TIN, IV.Tax_Rate " & _
                        " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                        " where  1=1 " & IDS & " " & sqlDate & " " & Lock & " " & sql & " order by InvoiceDate, InvoiceNo "
            da = New SqlDataAdapter(sql, cn)
            da.Fill(ds, "Opening")
            With GridView1
                .DataSource = ds.Tables("Opening")
                .DataBind()
            End With
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Protected Sub cmd_Sang1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbTax.SelectedIndexChanged
        Try
            Dim aa As String
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            aa = " Select * from AP_Tax_Rate where tax_Desc=N'" & Trim(cmbTax.Text) & "'  order by taxid"
            da = New SqlDataAdapter(aa, cn)
            da.Fill(ds, "IOtaxid")
            If ds.Tables("IOtaxid").Rows.Count > 0 Then
                TextBox14.Text = Trim(ds.Tables("IOtaxid").Rows(0).Item("taxid").ToString)
            End If
            '=============
            '===============STCNM===========
            Load_Grid()
        Catch ex As Exception
            Response.Write(ex)
        End Try
    End Sub

    Protected Sub GridView1_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit

    End Sub

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView1.RowCommand
        If e.CommandName = "NNOY" Then
            Dim aa As String = Session("Off_Id")
            Dim mySQLconnection As New SqlConnection(Session("cnstr"))
            Dim cm As New SqlCommand
            Dim sql As String
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            If mySQLconnection.State = ConnectionState.Open Then mySQLconnection.Close()
            mySQLconnection.Open()
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With

            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            Session("Date") = " ແຕ່ວັນທີ " & txtFdate.Text & " ເຖິງ " & txtTdate.Text

            sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "

            sql = ""
            sql = "  SELECT   * from AP_TAX_Items  where Bill_no='" & Trim(Session("ReportID")) & "'   "
            da = New SqlDataAdapter(sql, cn)
            da.Fill(ds, "ABC")
            If ds.Tables("ABC").Rows.Count > 0 Then
                sql = ""
                sql = " SELECT   * from AP_TAX_Items  where Bill_no='" & Session("Biil_No") & "' order by  Bill_no ASC "
            Else
                sql = ""
                sql = " SELECT  InvoiceDescription as ItemNm ,  1 as Qty, AmountInvoice as Price, AmountInvoice as Amonth  FROM AP_TAX_InvList where InvoiceNo='" & Session("Biil_No") & "' "
            End If
            Session("Report") = sql
            Response.Write("<script>")
            Response.Write("window.open('ALL_Report/FrmRTP_CryInvoiceTAX_NNOY.aspx','_blank')")
            Response.Write("</script>")

        End If
    End Sub

    Protected Sub GridView1_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridView1.RowEditing
        Session("ReportID") = GridView1.DataKeys(e.NewEditIndex).Values(0).ToString()
        Session("Bill_no") = GridView1.DataKeys(e.NewEditIndex).Values(0).ToString()
        Dim aa As String = Session("Off_Id")
        Dim mySQLconnection As New SqlConnection(Session("cnstr"))
        Dim cm As New SqlCommand
        Dim sql As String
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        If mySQLconnection.State = ConnectionState.Open Then mySQLconnection.Close()
        mySQLconnection.Open()
        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With

        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With
        Session("Date") = " ແຕ່ວັນທີ " & txtFdate.Text & " ເຖິງ " & txtTdate.Text

        sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "

        sql = ""
        sql = "  SELECT   * from AP_TAX_Items  where Bill_no='" & Trim(Session("ReportID")) & "'   "
        ' sql = "  SELECT   * from AP_TAX_InvList_Item  where tax_no='" & Trim(Session("ReportID")) & "'   "
        da = New SqlDataAdapter(sql, cn)
        da.Fill(ds, "ABC")
        If ds.Tables("ABC").Rows.Count > 0 Then
            sql = ""
            sql = " delete AP_TAX_InvList_Item where tax_id is null  " & _
            " insert into AP_TAX_InvList_Item (tax_nm,unit,qty,price,re_price)  " & _
            " SELECT   ItemNm ,  Unit, Qty, Price , Amonth  FROM AP_TAX_Items where Bill_no='" & Session("Bill_no") & "' order by  Bill_no ASC "
            With cm
                .CommandType = CommandType.Text
                .CommandText = sql
                .Connection = mySQLconnection
                .ExecuteNonQuery()
            End With


        Else
            sql = ""
            'sql = " SELECT  InvoiceDescription as ItemNm ,  1 as Qty, Unit, AmountInvoice as Price, AmountInvoice as Amonth  FROM AP_TAX_InvList where InvoiceNo='" & Session("Bill_no") & "' "
            sql = " delete AP_TAX_InvList_Item where tax_id is null  " & _
           " insert into AP_TAX_InvList_Item (tax_nm,qty,unit,price,re_price)  " & _
          " SELECT  InvoiceDescription as ItemNm ,  1 as Qty, Unit, AmountInvoice as Price, AmountInvoice as Amonth  FROM AP_TAX_InvList where InvoiceNo='" & Session("Bill_no") & "' "
            With cm
                .CommandType = CommandType.Text
                .CommandText = sql
                .Connection = mySQLconnection
                .ExecuteNonQuery()
            End With
        End If


        sql = " select count(tax_nm) as AA from AP_TAX_InvList_Item "
        da = New SqlDataAdapter(sql, cn)
        da.Fill(ds, "CCC")
        If ds.Tables("CCC").Rows(0).Item("AA").ToString > 32 Then
            sql = " select top 32 * from AP_TAX_InvList_Item order by tax_id "
        Else
            sql = " select top 16 * from AP_TAX_InvList_Item order by tax_id "

        End If


        Session("Report") = sql
        'Session("REP") = "Report"
        Response.Write("<script>")
        '  Response.Write("window.open('ALL_ACC/FrmRTP_CryInvoiceTAX.aspx','_blank')")
        Response.Write("window.open('ALL_Report/FrmRTP_CryInvoiceTAX.aspx','_blank')")
        '  Response.Write("window.open('ALL_ACC/CrystalReport1_SN_LAK.rpt','_blank')")
        Response.Write("</script>")

    End Sub


    Protected Sub Button10_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button10.Click
        Try
            Dim mySQLconnection As New SqlConnection(Session("cnstr"))
            Dim cm As New SqlCommand
            Dim sql As String
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            If mySQLconnection.State = ConnectionState.Open Then mySQLconnection.Close()
            mySQLconnection.Open()
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            Dim aa As String
            With cn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = Session("cnstr")
                .Open()
            End With
            Session("Date") = " ແຕ່ວັນທີ " & txtFdate.Text & " ເຖິງ " & txtTdate.Text

            Dim Lock As String
            If ComboBox1.SelectedIndex = 0 Then
                Lock = ""
            ElseIf ComboBox1.SelectedIndex = 1 Then
                Lock = " AND IV.Lock=0 "
            Else
                Lock = " AND IV.Lock=1 "
            End If

            sqlDate = " AND IV.InvoiceDate >= '" & Right(Trim(txtFdate.Text), 4) & "/" & Mid(Trim(txtFdate.Text), 4, 2) & "/" & Left(Trim(txtFdate.Text), 2) & " 00:00:00' AND IV.InvoiceDate<='" & Right(Trim(txtTdate.Text), 4) & "/" & Mid(Trim(txtTdate.Text), 4, 2) & "/" & Left(Trim(txtTdate.Text), 2) & " 23:59:59'  "
            sql = " delete RPT_TAX_InvList_Item "
            sql = sql & " insert into RPT_TAX_InvList_Item ( Lock, InvoiceNo, InvoiceDate, DueDate, JobNo, PayerAccountID, ItemCount,AmountInvoice," & _
" AmountTax, AmountNet_0,AmountNet_10 , AmountDiscount, PercentDiscount, AmountTotal, AmountPaid, Currency, InvoiceDescription, IsFinished, THB_LAK, " & _
 " USD_LAK, AcRevenue, AcTax, Paid_Date, AcAdv, credit_term, AmountLAK, AcDebit,PayerAccountNm, TIN, TAX_Rate ) " & _
                        "SELECT IV.Lock, IV.InvoiceNo, IV.InvoiceDate, IV.DueDate, IV.JobNo, IV.PayerAccountID, IV.ItemCount, IV.AmountInvoice, IV.AmountTax, IV.AmountNet, 0, IV.AmountDiscount, IV.PercentDiscount, IV.AmountTotal, IV.AmountPaid, IV.Currency,  " & _
                        " IV.InvoiceDescription, IV.IsFinished, IV.THB_LAK, IV.USD_LAK, IV.AcRevenue, IV.AcTax, IV.Paid_Date, IV.AcAdv, IV.credit_term, IV.AmountLAK, IV.AcDebit, CM.PayerAccountNm, IV.TIN, IV.Tax_Rate " & _
                        " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                        " where  1=1  " & sqlDate & " " & Lock & " and IV.Tax_Rate = '0' " & _
                        "  union all  " & _
                        "SELECT IV.Lock, IV.InvoiceNo, IV.InvoiceDate, IV.DueDate, IV.JobNo, IV.PayerAccountID, IV.ItemCount, IV.AmountInvoice, IV.AmountTax, 0, IV.AmountNet, IV.AmountDiscount, IV.PercentDiscount, IV.AmountTotal, IV.AmountPaid, IV.Currency,  " & _
                        " IV.InvoiceDescription, IV.IsFinished, IV.THB_LAK, IV.USD_LAK, IV.AcRevenue, IV.AcTax, IV.Paid_Date, IV.AcAdv, IV.credit_term, IV.AmountLAK, IV.AcDebit, CM.PayerAccountNm, IV.TIN, IV.Tax_Rate " & _
                        " from AP_TAX_InvList IV Inner join AP_Customers CM ON IV.PayerAccountID=CM.customerID " & _
                        " where  1=1  " & sqlDate & " " & Lock & " and (IV.Tax_Rate = '7' or IV.Tax_Rate = '10') "
            With cm
                .CommandType = CommandType.Text
                .CommandText = sql
                .Connection = mySQLconnection
                .ExecuteNonQuery()
            End With


            sql = ""
            sql = "select * from RPT_TAX_InvList_Item order by InvoiceDate"
            Session("Report") = sql
            'Session("REP") = "Report"
            Response.Write("<script>")
            Response.Write("window.open('ALL_ACC/XML_CrystalReport_InvoiceTAX.aspx','_blank')")
            Response.Write("</script>")
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub GridView1_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim ccn As String = GridView1.DataKeys(e.RowIndex).Values(0).ToString()
        Session("fsit") = GridView1.DataKeys(e.RowIndex).Values(0).ToString()
        Session("Biil_No") = GridView1.DataKeys(e.RowIndex).Values(0).ToString()
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        With cn
            If .State = ConnectionState.Open Then .Close()
            .ConnectionString = Session("cnstr")
            .Open()
        End With
        Session("Catgory") = "0"
        Session("EditActive") = "1"
        If CheckBox1.Checked = True Then
            Session("CheckBox1") = "1"
        Else
            Session("CheckBox1") = "0"
        End If
        Response.Redirect("frm_invoice.aspx")

    End Sub
    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
       
    End Sub

End Class

