Public Class DocumentHistoryRow
    Public Property RevisionCode As String
    Public Property Description As String
    Public Property AssignedTo As String

    Public Property Round1Received As Date?
    Public Property Round1Comments As Date?
    Public Property Round1Attachment As String

    Public Property Round2Received As Date?
    Public Property Round2Comments As Date?
    Public Property Round2Attachment As String

    Public Property Round3Received As Date?
    Public Property Round3Comments As Date?
    Public Property Round3Attachment As String

    Public Property DateExpected As Date?
    Public Property OfficialIssueDate As Date?
End Class