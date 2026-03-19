Public Class ProjectDocument

    Public Property Id As Integer

    Public Property ProjectId As Integer
    Public Property EnvironmentId As Integer

    Public Property DocumentNumber As String
    Public Property Title As String
    Public Property CurrentRevision As String
    Public Property IssueDate As Date?
    Public Property DateToManufacturer As Date?
    Public Property Status As String

    Public Overridable Property Project As Project
    Public Overridable Property Environment As ProjectEnvironment
    Public Overridable Property Revisions As List(Of DocumentRevision)

End Class