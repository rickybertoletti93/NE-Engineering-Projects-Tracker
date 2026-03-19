Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Project

    <Key>
    Public Property Id As Integer

    Public Property ProjectCode As String
    Public Property ProjectName As String

    Public Overridable Property Environments As List(Of ProjectEnvironment)
    Public Overridable Property Documents As List(Of ProjectDocument)

    <NotMapped>
    Public ReadOnly Property DisplayName As String
        Get
            Return ProjectCode & " - " & ProjectName
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return DisplayName
    End Function

End Class