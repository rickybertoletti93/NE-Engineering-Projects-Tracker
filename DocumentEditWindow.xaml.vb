Class DocumentEditWindow

    Public Property DocumentNumberValue As String
    Public Property DocumentTitleValue As String
    Public Property StatusValue As String
    Public Property NEDateValue As Date?
    Public Property ManufacturerDateValue As Date?

    Public Sub New()
        InitializeComponent()
        LoadStatuses()
        UpdateDateDisplays()
    End Sub

    Public Sub New(dialogTitle As String,
                   documentNumber As String,
                   documentTitle As String,
                   status As String,
                   neDate As Date?,
                   manufacturerDate As Date?)
        InitializeComponent()
        LoadStatuses()

        txtTitle.Text = dialogTitle
        txtDocumentNumber.Text = documentNumber
        txtDocumentTitle.Text = documentTitle
        cmbStatus.Text = status
        dpNEDate.SelectedDate = neDate
        dpManufacturerDate.SelectedDate = manufacturerDate

        UpdateDateDisplays()
    End Sub

    Private Sub LoadStatuses()
        cmbStatus.ItemsSource = New List(Of String) From {
            "NOT STARTED",
            "WORKING",
            "UNDER INTERNAL REVIEW",
            "COMMENTS TO IMPLEMENT",
            "READY FOR ISSUE",
            "ISSUED"
        }

        cmbStatus.SelectedIndex = 0
    End Sub

    Private Sub btnNEDate_Click(sender As Object, e As RoutedEventArgs)
        dpNEDate.IsDropDownOpen = True
    End Sub

    Private Sub btnManufacturerDate_Click(sender As Object, e As RoutedEventArgs)
        dpManufacturerDate.IsDropDownOpen = True
    End Sub

    Private Sub dpNEDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        UpdateDateDisplays()
    End Sub

    Private Sub dpManufacturerDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        UpdateDateDisplays()
    End Sub

    Private Sub UpdateDateDisplays()
        If dpNEDate IsNot Nothing AndAlso btnNEDate IsNot Nothing Then
            btnNEDate.Tag = If(dpNEDate.SelectedDate.HasValue,
                               dpNEDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                               "Select a date")
        End If

        If dpManufacturerDate IsNot Nothing AndAlso btnManufacturerDate IsNot Nothing Then
            btnManufacturerDate.Tag = If(dpManufacturerDate.SelectedDate.HasValue,
                                         dpManufacturerDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                                         "Select a date")
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrWhiteSpace(txtDocumentNumber.Text) Then
            txtDocumentNumber.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtDocumentTitle.Text) Then
            txtDocumentTitle.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(cmbStatus.Text) Then
            cmbStatus.Focus()
            Return
        End If

        DocumentNumberValue = txtDocumentNumber.Text.Trim()
        DocumentTitleValue = txtDocumentTitle.Text.Trim()
        StatusValue = cmbStatus.Text.Trim()
        NEDateValue = dpNEDate.SelectedDate
        ManufacturerDateValue = dpManufacturerDate.SelectedDate

        DialogResult = True
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
        Close()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
        Close()
    End Sub

    Private Sub Header_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs)
        DragMove()
    End Sub

End Class