Public Class ProjectEnvironment

    Public Property Id As Integer
    Public Property ProjectId As Integer
    Public Property EnvironmentName As String

    Public Overridable Property Project As Project
    Public Overridable Property Documents As List(Of ProjectDocument)

    Public Overrides Function ToString() As String
        Return EnvironmentName
    End Function

End Class