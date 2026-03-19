Public Class DocumentCommentRoundViewItem
    Public Property RoundNumber As Integer
    Public Property DateReceived As Date?
    Public Property CommentsDate As Date?
    Public Property AttachmentPath As String

    Public ReadOnly Property AttachmentFileName As String
        Get
            If String.IsNullOrWhiteSpace(AttachmentPath) Then Return "-"
            Return System.IO.Path.GetFileName(AttachmentPath)
        End Get
    End Property
End Class