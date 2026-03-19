Public Class RevisionCommentRound

    Public Property Id As Integer
    Public Property DocumentRevisionId As Integer

    Public Property RoundNumber As Integer   ' 1, 2, 3

    Public Property DateReceived As Date?
    Public Property CommentsDate As Date?
    Public Property AttachmentPath As String

    Public Overridable Property Revision As DocumentRevision

End Class