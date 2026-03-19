Public Class DocumentRevision

    Public Property Id As Integer
    Public Property ProjectDocumentId As Integer

    Public Property RevisionCode As String
    Public Property Description As String
    Public Property AssignedTo As String

    Public Property InternalReceiveDate As Date?
    Public Property DateExpected As Date?
    Public Property OfficialIssueDate As Date?

    Public Overridable Property Document As ProjectDocument
    Public Overridable Property CommentRounds As List(Of RevisionCommentRound)

End Class