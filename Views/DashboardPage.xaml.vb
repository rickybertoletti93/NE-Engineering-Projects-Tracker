Class DashboardPage

    Private ReadOnly _viewModel As New DashboardViewModel()

    Private Sub DashboardPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        DataContext = _viewModel

        AddHandler AppState.SelectionChanged, AddressOf RefreshDashboard
        AddHandler AppState.DocumentsChanged, AddressOf RefreshDashboard
        AddHandler AppState.ProjectsChanged, AddressOf RefreshDashboard
        AddHandler AppState.EnvironmentsChanged, AddressOf RefreshDashboard

        RefreshDashboard()
    End Sub

    Private Sub RefreshDashboard()
        _viewModel.LoadData()
    End Sub

End Class