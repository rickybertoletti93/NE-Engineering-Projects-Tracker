Class ConfirmDialog

    Public Sub New(dialogTitle As String, dialogMessage As String)
        InitializeComponent()

        txtTitle.Text = dialogTitle
        txtMessage.Text = dialogMessage
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = True
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
        Close()
    End Sub

End Class