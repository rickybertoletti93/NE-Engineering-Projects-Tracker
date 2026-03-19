Public Class AppState

    Public Shared Event ProjectsChanged()
    Public Shared Event EnvironmentsChanged()
    Public Shared Event SelectionChanged()
    Public Shared Event DocumentsChanged()

    Public Shared Property SelectedProjectId As Integer?
    Public Shared Property SelectedEnvironmentId As Integer?

    Public Shared Sub RaiseProjectsChanged()
        RaiseEvent ProjectsChanged()
    End Sub

    Public Shared Sub RaiseEnvironmentsChanged()
        RaiseEvent EnvironmentsChanged()
    End Sub

    Public Shared Sub RaiseSelectionChanged()
        RaiseEvent SelectionChanged()
    End Sub

    Public Shared Sub RaiseDocumentsChanged()
        RaiseEvent DocumentsChanged()
    End Sub

End Class