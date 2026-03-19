Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows.Controls.Primitives
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.VisualBasic



Class ProjectsPage

    Private ReadOnly AvailableNames As New List(Of String) From {
        "Inlet Duct",
        "Casing",
        "Pressure Parts",
        "Pipe Supports"
    }

    Private EnvironmentItems As ObservableCollection(Of AvailableEnvironment)
    Private IsRefreshingUI As Boolean = False

    Private Sub ProjectsPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        InitializeEnvironmentItems()
        LoadProjects()
    End Sub

    Private Sub InitializeEnvironmentItems()
        EnvironmentItems = New ObservableCollection(Of AvailableEnvironment)(
            AvailableNames.Select(Function(name) New AvailableEnvironment With {
                .Name = name,
                .IsEnabled = False
            })
        )

        icEnvironments.ItemsSource = EnvironmentItems
    End Sub

    Private Sub LoadProjects(Optional projectIdToSelect As Integer? = Nothing)
        IsRefreshingUI = True

        Using db As New AppDbContext()
            Dim projects = db.Projects.
            OrderBy(Function(p) p.ProjectCode).
            ToList()

            lstProjects.ItemsSource = Nothing
            lstProjects.ItemsSource = projects

            If projects.Any() Then
                Dim projectToSelect As Project = Nothing

                If projectIdToSelect.HasValue Then
                    projectToSelect = projects.FirstOrDefault(Function(p) p.Id = projectIdToSelect.Value)
                End If

                If projectToSelect Is Nothing Then
                    projectToSelect = projects.First()
                End If

                lstProjects.SelectedItem = projectToSelect
            Else
                lstProjects.SelectedItem = Nothing
                ClearEnvironmentSelection()
            End If
        End Using

        IsRefreshingUI = False
        RefreshEnvironmentFlags()
    End Sub

    Private Sub btnNewProject_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New ProjectEditWindow("New Project", "", "")
        dlg.Owner = Window.GetWindow(Me)

        Dim result = dlg.ShowDialog()
        If result <> True Then Return

        Using db As New AppDbContext()
            Dim exists = db.Projects.Any(Function(p) p.ProjectCode = dlg.ProjectCodeValue)
            If exists Then
                MessageBox.Show("A project with this code already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            db.Projects.Add(New Project With {
            .ProjectCode = dlg.ProjectCodeValue,
            .ProjectName = dlg.ProjectNameValue
        })

            db.SaveChanges()
        End Using

        LoadProjects()
        AppState.RaiseProjectsChanged()
    End Sub

    Private Sub btnEditProject_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedProject = TryCast(lstProjects.SelectedItem, Project)
        If selectedProject Is Nothing Then
            MessageBox.Show("Select a project first.", "Edit Project", MessageBoxButton.OK, MessageBoxImage.Information)
            Return
        End If

        Dim dlg As New ProjectEditWindow("Edit Project", selectedProject.ProjectCode, selectedProject.ProjectName)
        dlg.Owner = Window.GetWindow(Me)

        Dim result = dlg.ShowDialog()
        If result <> True Then Return

        Using db As New AppDbContext()
            Dim duplicate = db.Projects.Any(Function(p) p.ProjectCode = dlg.ProjectCodeValue AndAlso p.Id <> selectedProject.Id)
            If duplicate Then
                MessageBox.Show("A project with this code already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim dbProject = db.Projects.FirstOrDefault(Function(p) p.Id = selectedProject.Id)
            If dbProject Is Nothing Then Return

            dbProject.ProjectCode = dlg.ProjectCodeValue
            dbProject.ProjectName = dlg.ProjectNameValue

            db.SaveChanges()
        End Using

        LoadProjects()
        ReselectProject(selectedProject.Id)
        AppState.RaiseProjectsChanged()
    End Sub

    Private Sub btnDeleteProject_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedProject = TryCast(lstProjects.SelectedItem, Project)
        If selectedProject Is Nothing Then
            Return
        End If

        Dim dlg As New ConfirmDialog(
        "Delete Project",
        "Are you sure you want to delete project '" & selectedProject.DisplayName & "'? This operation cannot be undone."
    )
        dlg.Owner = Window.GetWindow(Me)

        Dim result = dlg.ShowDialog()
        If result <> True Then Return

        Using db As New AppDbContext()
            Dim dbProject = db.Projects.
            Include(Function(p) p.Environments).
            FirstOrDefault(Function(p) p.Id = selectedProject.Id)

            If dbProject Is Nothing Then Return

            db.Projects.Remove(dbProject)
            db.SaveChanges()
        End Using

        LoadProjects()
        AppState.RaiseProjectsChanged()
        AppState.RaiseEnvironmentsChanged()
    End Sub

    Private Sub lstProjects_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If IsRefreshingUI Then Return
        RefreshEnvironmentFlags()
    End Sub

    Private Sub RefreshEnvironmentFlags()
        If EnvironmentItems Is Nothing Then Return

        IsRefreshingUI = True

        Try
            For Each item In EnvironmentItems
                item.IsEnabled = False
            Next

            Dim selectedProject = TryCast(lstProjects.SelectedItem, Project)

            If selectedProject IsNot Nothing Then
                Using db As New AppDbContext()
                    Dim assignedNames = db.Environments.
                    Where(Function(env) env.ProjectId = selectedProject.Id).
                    Select(Function(env) env.EnvironmentName).
                    ToList()

                    For Each item In EnvironmentItems
                        item.IsEnabled = assignedNames.Contains(item.Name)
                    Next
                End Using
            End If

            icEnvironments.ItemsSource = Nothing
            icEnvironments.ItemsSource = EnvironmentItems

        Finally
            IsRefreshingUI = False
        End Try
    End Sub

    Private Sub EnvironmentToggle_Changed(sender As Object, e As RoutedEventArgs)
        If IsRefreshingUI Then Return

        Dim selectedProject = TryCast(lstProjects.SelectedItem, Project)
        If selectedProject Is Nothing Then
            RefreshEnvironmentFlags()
            Return
        End If

        Dim toggle = TryCast(sender, ToggleButton)
        If toggle Is Nothing Then Return

        Dim envItem = TryCast(toggle.DataContext, AvailableEnvironment)
        If envItem Is Nothing Then Return

        Dim isChecked As Boolean = toggle.IsChecked.GetValueOrDefault()

        Using db As New AppDbContext()
            Dim existing = db.Environments.
            FirstOrDefault(Function(x) x.ProjectId = selectedProject.Id AndAlso x.EnvironmentName = envItem.Name)

            ' ATTIVAZIONE
            If isChecked Then
                If existing Is Nothing Then
                    db.Environments.Add(New ProjectEnvironment With {
                    .ProjectId = selectedProject.Id,
                    .EnvironmentName = envItem.Name
                })

                    db.SaveChanges()
                    AppState.RaiseEnvironmentsChanged()
                End If

                RefreshEnvironmentFlags()
                Return
            End If

            ' DISATTIVAZIONE
            If existing IsNot Nothing Then
                Dim documentCount = db.Documents.Count(Function(d) d.EnvironmentId = existing.Id)

                If documentCount = 0 Then
                    db.Environments.Remove(existing)
                    db.SaveChanges()
                    AppState.RaiseEnvironmentsChanged()
                    RefreshEnvironmentFlags()
                    Return
                End If

                Dim dlg As New ConfirmDialog(
                "Remove Environment",
                "Removing this environment will also delete " &
                documentCount.ToString() &
                " linked document(s), including revisions and review cycles. Continue?"
            )
                dlg.Owner = Window.GetWindow(Me)

                Dim result = dlg.ShowDialog()
                If result <> True Then
                    RefreshEnvironmentFlags()
                    Return
                End If

                db.Environments.Remove(existing)
                db.SaveChanges()
                AppState.RaiseEnvironmentsChanged()
            End If
        End Using

        RefreshEnvironmentFlags()
    End Sub

    Private Sub ClearEnvironmentSelection()
        IsRefreshingUI = True

        lstProjects.SelectedItem = Nothing

        If EnvironmentItems IsNot Nothing Then
            For Each item In EnvironmentItems
                item.IsEnabled = False
            Next

            icEnvironments.ItemsSource = Nothing
            icEnvironments.ItemsSource = EnvironmentItems
        End If

        IsRefreshingUI = False
    End Sub

    Private Sub ReselectProject(projectId As Integer)
        If lstProjects.ItemsSource Is Nothing Then Return

        For Each item In lstProjects.Items
            Dim project = TryCast(item, Project)
            If project IsNot Nothing AndAlso project.Id = projectId Then
                lstProjects.SelectedItem = project
                Exit For
            End If
        Next
    End Sub

End Class