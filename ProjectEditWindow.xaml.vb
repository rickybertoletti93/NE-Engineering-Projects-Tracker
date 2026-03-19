Class ProjectEditWindow

    Public Property ProjectCodeValue As String
    Public Property ProjectNameValue As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(dialogTitle As String, projectCode As String, projectName As String)
        InitializeComponent()

        txtTitle.Text = dialogTitle
        txtProjectCode.Text = projectCode
        txtProjectName.Text = projectName
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrWhiteSpace(txtProjectCode.Text) Then
            MessageBox.Show("Project Code is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Information)
            txtProjectCode.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtProjectName.Text) Then
            MessageBox.Show("Project Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Information)
            txtProjectName.Focus()
            Return
        End If

        ProjectCodeValue = txtProjectCode.Text.Trim()
        ProjectNameValue = txtProjectName.Text.Trim()

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