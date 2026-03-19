Imports System.Linq
Imports Microsoft.EntityFrameworkCore

Class MainWindow

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AddHandler AppState.ProjectsChanged, AddressOf OnProjectsChanged
        AddHandler AppState.EnvironmentsChanged, AddressOf OnEnvironmentsChanged

        SetActiveMenu(btnDashboard)
        MainFrame.Navigate(New DashboardPage())
        UpdateWindowAppearance()
        LoadProjects()
    End Sub

    Private Sub OnProjectsChanged()
        Dim selectedProject = TryCast(cmbProjects.SelectedItem, Project)
        Dim selectedProjectId As Integer? = Nothing

        If selectedProject IsNot Nothing Then
            selectedProjectId = selectedProject.Id
        End If

        LoadProjects(selectedProjectId)
    End Sub

    Private Sub OnEnvironmentsChanged()
        Dim selectedProject = TryCast(cmbProjects.SelectedItem, Project)
        If selectedProject Is Nothing Then
            cmbEnvironments.ItemsSource = Nothing
            Return
        End If

        LoadEnvironments(selectedProject.Id)
    End Sub

    Private Sub btnDashboard_Click(sender As Object, e As RoutedEventArgs)
        SetActiveMenu(btnDashboard)
        MainFrame.Navigate(New DashboardPage())
    End Sub

    Private Sub btnDocuments_Click(sender As Object, e As RoutedEventArgs)
        SetActiveMenu(btnDocuments)
        MainFrame.Navigate(New DocumentsPage())
    End Sub

    Private Sub btnProjects_Click(sender As Object, e As RoutedEventArgs)
        SetActiveMenu(btnProjects)
        MainFrame.Navigate(New ProjectsPage())
    End Sub

    Private Sub SetActiveMenu(activeButton As Button)
        btnDashboard.Tag = Nothing
        btnDocuments.Tag = Nothing
        btnProjects.Tag = Nothing
        activeButton.Tag = "Active"
    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As RoutedEventArgs)
        WindowState = WindowState.Minimized
    End Sub

    Private Sub btnMaximize_Click(sender As Object, e As RoutedEventArgs)
        If WindowState = WindowState.Maximized Then
            WindowState = WindowState.Normal
        Else
            WindowState = WindowState.Maximized
        End If
        UpdateWindowAppearance()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub TopBar_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs)
        If e.ClickCount = 2 Then
            btnMaximize_Click(Nothing, Nothing)
        Else
            DragMove()
        End If
    End Sub

    Private Sub Window_StateChanged(sender As Object, e As EventArgs)
        UpdateWindowAppearance()
    End Sub

    Private Sub UpdateWindowAppearance()
        If WindowState = WindowState.Maximized Then
            MainShell.CornerRadius = New CornerRadius(0)
            SidebarBorder.CornerRadius = New CornerRadius(0)
            btnMaximize.Content = "❐"
        Else
            MainShell.CornerRadius = New CornerRadius(14)
            SidebarBorder.CornerRadius = New CornerRadius(14, 0, 0, 14)
            btnMaximize.Content = "□"
        End If
    End Sub

    Protected Overrides Sub OnSourceInitialized(e As EventArgs)
        MyBase.OnSourceInitialized(e)
        Me.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight
    End Sub

    Private Sub LoadProjects(Optional projectIdToSelect As Integer? = Nothing)
        Using db As New AppDbContext()
            Dim projects = db.Projects.
                OrderBy(Function(p) p.ProjectCode).
                ToList()

            cmbProjects.ItemsSource = Nothing
            cmbProjects.ItemsSource = projects

            If Not projects.Any() Then
                cmbProjects.SelectedItem = Nothing
                cmbEnvironments.ItemsSource = Nothing
                Return
            End If

            Dim projectToSelect As Project = Nothing

            If projectIdToSelect.HasValue Then
                projectToSelect = projects.FirstOrDefault(Function(p) p.Id = projectIdToSelect.Value)
            End If

            If projectToSelect Is Nothing Then
                projectToSelect = projects.First()
            End If

            cmbProjects.SelectedItem = projectToSelect
        End Using
    End Sub

    Private Sub LoadEnvironments(projectId As Integer, Optional environmentIdToSelect As Integer? = Nothing)
        Using db As New AppDbContext()
            Dim environments = db.Environments.
                Where(Function(env) env.ProjectId = projectId).
                OrderBy(Function(env) env.EnvironmentName).
                ToList()

            cmbEnvironments.ItemsSource = Nothing
            cmbEnvironments.ItemsSource = environments

            If Not environments.Any() Then
                cmbEnvironments.SelectedItem = Nothing
                Return
            End If

            Dim environmentToSelect As ProjectEnvironment = Nothing

            If environmentIdToSelect.HasValue Then
                environmentToSelect = environments.FirstOrDefault(Function(env) env.Id = environmentIdToSelect.Value)
            End If

            If environmentToSelect Is Nothing Then
                environmentToSelect = environments.First()
            End If

            cmbEnvironments.SelectedItem = environmentToSelect
        End Using
    End Sub

    Private Sub cmbProjects_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedProject = TryCast(cmbProjects.SelectedItem, Project)

        If selectedProject Is Nothing Then
            AppState.SelectedProjectId = Nothing
            AppState.SelectedEnvironmentId = Nothing
            cmbEnvironments.ItemsSource = Nothing
            AppState.RaiseSelectionChanged()
            Return
        End If

        AppState.SelectedProjectId = selectedProject.Id
        LoadEnvironments(selectedProject.Id)
    End Sub

    Private Sub cmbEnvironments_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedEnvironment = TryCast(cmbEnvironments.SelectedItem, ProjectEnvironment)

        If selectedEnvironment Is Nothing Then
            AppState.SelectedEnvironmentId = Nothing
        Else
            AppState.SelectedEnvironmentId = selectedEnvironment.Id
        End If

        AppState.RaiseSelectionChanged()
    End Sub

End Class