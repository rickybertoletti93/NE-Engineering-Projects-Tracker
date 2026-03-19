Imports System.Collections.ObjectModel
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports ClosedXML.Excel
Imports Microsoft.Win32

Class DocumentsPage
    Private ReadOnly DocumentStatuses As New List(Of String) From {
    "NOT STARTED",
    "WORKING",
    "UNDER INTERNAL REVIEW",
    "COMMENTS TO IMPLEMENT",
    "READY FOR ISSUE",
    "ISSUED"
}
    Private DocumentRows As ObservableCollection(Of DocumentListRow)

    Public ReadOnly Property StatusOptions As List(Of String)
        Get
            Return DocumentStatuses
        End Get
    End Property

    Private Sub DocumentsPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AddHandler AppState.SelectionChanged, AddressOf OnTopSelectionChanged

        Me.DataContext = Me

        DocumentRows = New ObservableCollection(Of DocumentListRow)()
        dgDocuments.ItemsSource = DocumentRows

        LoadDocumentsForCurrentSelection()
    End Sub

    Private Sub OnTopSelectionChanged()
        LoadDocumentsForCurrentSelection()
    End Sub

    Private Sub LoadDocumentsForCurrentSelection(Optional documentIdToSelect As Integer? = Nothing)
        DocumentRows.Clear()

        If Not AppState.SelectedProjectId.HasValue OrElse Not AppState.SelectedEnvironmentId.HasValue Then
            txtFilterSummary.Text = "No project/environment selected"
            dgDocuments.SelectedItem = Nothing
            Return
        End If

        Using db As New AppDbContext()

            Dim project = db.Projects.FirstOrDefault(Function(p) p.Id = AppState.SelectedProjectId.Value)
            Dim environment = db.Environments.FirstOrDefault(Function(e) e.Id = AppState.SelectedEnvironmentId.Value)

            If project Is Nothing OrElse environment Is Nothing Then
                txtFilterSummary.Text = "No project/environment selected"
                dgDocuments.SelectedItem = Nothing
                Return
            End If

            txtFilterSummary.Text = project.DisplayName & "  |  " & environment.EnvironmentName

            Dim documents = db.Documents.
                Where(Function(d) d.ProjectId = project.Id AndAlso d.EnvironmentId = environment.Id).
                OrderBy(Function(d) d.DocumentNumber).
                ToList()

            Dim documentIds = documents.Select(Function(d) d.Id).ToList()
            Dim latestRevisionByDocumentId = db.DocumentRevisions.
                Where(Function(r) documentIds.Contains(r.ProjectDocumentId)).
                AsEnumerable().
                GroupBy(Function(r) r.ProjectDocumentId).
                ToDictionary(Function(g) g.Key,
                             Function(g) g.OrderByDescending(Function(r) r.RevisionCode).ThenByDescending(Function(r) r.Id).FirstOrDefault())

            For Each doc In documents
                Dim latestRevision As DocumentRevision = Nothing

                If latestRevisionByDocumentId.ContainsKey(doc.Id) Then
                    latestRevision = latestRevisionByDocumentId(doc.Id)
                End If

                DocumentRows.Add(New DocumentListRow With {
                    .Id = doc.Id,
                    .DocumentNumber = doc.DocumentNumber,
                    .Title = doc.Title,
                    .Status = doc.Status,
                    .NEDate = doc.IssueDate,
                    .ManufacturerDate = doc.DateToManufacturer,
                    .SRLEffectiveDate = If(latestRevision IsNot Nothing, latestRevision.OfficialIssueDate, Nothing),
                    .ManufacturerEffectiveDate = Nothing
                })
            Next

            If DocumentRows.Any() Then
                Dim rowToSelect As DocumentListRow = Nothing

                If documentIdToSelect.HasValue Then
                    rowToSelect = DocumentRows.FirstOrDefault(Function(r) r.Id = documentIdToSelect.Value)
                End If

                If rowToSelect Is Nothing Then
                    rowToSelect = DocumentRows.First()
                End If

                dgDocuments.SelectedItem = rowToSelect
            Else
                dgDocuments.SelectedItem = Nothing
            End If
        End Using
    End Sub

    Private Sub dgDocuments_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        ' Per ora non serve altro.
    End Sub

    Private Sub btnNewDocument_Click(sender As Object, e As RoutedEventArgs)
        If Not AppState.SelectedProjectId.HasValue OrElse Not AppState.SelectedEnvironmentId.HasValue Then
            Return
        End If

        Dim dlg As New DocumentEditWindow(
            "New Document",
            "",
            "",
            "NOT STARTED",
            Date.Today,
            Nothing
        )
        dlg.Owner = Window.GetWindow(Me)

        Dim result = dlg.ShowDialog()
        If result <> True Then Return

        Dim newDocumentId As Integer

        Using db As New AppDbContext()

            Dim exists = db.Documents.Any(Function(d) d.ProjectId = AppState.SelectedProjectId.Value AndAlso
                                                      d.EnvironmentId = AppState.SelectedEnvironmentId.Value AndAlso
                                                      d.DocumentNumber = dlg.DocumentNumberValue)

            If exists Then Return

            Dim document As New ProjectDocument With {
                .ProjectId = AppState.SelectedProjectId.Value,
                .EnvironmentId = AppState.SelectedEnvironmentId.Value,
                .DocumentNumber = dlg.DocumentNumberValue,
                .Title = dlg.DocumentTitleValue,
                .CurrentRevision = "00",
                .IssueDate = dlg.NEDateValue,
                .DateToManufacturer = dlg.ManufacturerDateValue,
                .Status = dlg.StatusValue
            }

            db.Documents.Add(document)
            db.SaveChanges()

            newDocumentId = document.Id

            Dim revision As New DocumentRevision With {
                .ProjectDocumentId = document.Id,
                .RevisionCode = "00",
                .Description = "FIRST ISSUE",
                .AssignedTo = "",
                .InternalReceiveDate = dlg.NEDateValue,
                .DateExpected = Nothing,
                .OfficialIssueDate = Nothing
            }

            db.DocumentRevisions.Add(revision)
            db.SaveChanges()
        End Using

        LoadDocumentsForCurrentSelection(newDocumentId)
        AppState.RaiseDocumentsChanged()
    End Sub

    Private Sub btnEditDocument_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedRow = TryCast(dgDocuments.SelectedItem, DocumentListRow)
        If selectedRow Is Nothing Then Return

        Using db As New AppDbContext()

            Dim document = db.Documents.FirstOrDefault(Function(d) d.Id = selectedRow.Id)
            If document Is Nothing Then Return

            Dim dlg As New DocumentEditWindow(
                "Edit Document",
                document.DocumentNumber,
                document.Title,
                document.Status,
                document.IssueDate,
                document.DateToManufacturer
            )

            dlg.Owner = Window.GetWindow(Me)

            Dim result = dlg.ShowDialog()
            If result <> True Then Return

            Dim duplicate = db.Documents.Any(Function(d) d.Id <> document.Id AndAlso
                                                         d.ProjectId = document.ProjectId AndAlso
                                                         d.EnvironmentId = document.EnvironmentId AndAlso
                                                         d.DocumentNumber = dlg.DocumentNumberValue)

            If duplicate Then Return

            document.DocumentNumber = dlg.DocumentNumberValue
            document.Title = dlg.DocumentTitleValue
            document.Status = dlg.StatusValue
            document.IssueDate = dlg.NEDateValue
            document.DateToManufacturer = dlg.ManufacturerDateValue

            db.SaveChanges()
        End Using

        LoadDocumentsForCurrentSelection(selectedRow.Id)
        AppState.RaiseDocumentsChanged()
    End Sub

    Private Sub btnDeleteDocument_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedRow = TryCast(dgDocuments.SelectedItem, DocumentListRow)
        If selectedRow Is Nothing Then Return

        Dim dlg As New ConfirmDialog(
            "Delete Document",
            "Are you sure you want to delete document '" & selectedRow.DocumentNumber & "'? This operation cannot be undone."
        )
        dlg.Owner = Window.GetWindow(Me)

        Dim result = dlg.ShowDialog()
        If result <> True Then Return

        Using db As New AppDbContext()
            Dim document = db.Documents.FirstOrDefault(Function(d) d.Id = selectedRow.Id)
            If document Is Nothing Then Return

            db.Documents.Remove(document)
            db.SaveChanges()
        End Using

        LoadDocumentsForCurrentSelection()
        AppState.RaiseDocumentsChanged()
    End Sub

    Private Sub StatusComboBox_DropDownClosed(sender As Object, e As EventArgs)
        Dim combo = TryCast(sender, ComboBox)
        If combo Is Nothing Then Return
        If combo.SelectedItem Is Nothing Then Return

        Dim row = TryCast(combo.DataContext, DocumentListRow)
        If row Is Nothing Then Return

        Dim newStatus = combo.SelectedItem.ToString().Trim()

        Using db As New AppDbContext()
            Dim document = db.Documents.FirstOrDefault(Function(d) d.Id = row.Id)
            If document Is Nothing Then Return

            If String.Equals(document.Status, newStatus, StringComparison.OrdinalIgnoreCase) Then Return

            document.Status = newStatus
            db.SaveChanges()
        End Using

        row.Status = newStatus
        AppState.RaiseDocumentsChanged()
    End Sub

    Private Sub btnImportList_Click(sender As Object, e As RoutedEventArgs)
        If Not AppState.SelectedProjectId.HasValue OrElse Not AppState.SelectedEnvironmentId.HasValue Then
            Return
        End If

        Dim openFileDialog As New OpenFileDialog With {
            .Title = "Select Excel file",
            .Filter = "Excel files (*.xlsx)|*.xlsx",
            .Multiselect = False
        }

        Dim result = openFileDialog.ShowDialog()
        If result <> True Then Return

        Dim importedCount As Integer = 0
        Dim skippedCount As Integer = 0

        Try
            Using workbook As New XLWorkbook(openFileDialog.FileName)
                Dim worksheet = workbook.Worksheet(1)

                Using db As New AppDbContext()

                    Dim lastRow = worksheet.LastRowUsed().RowNumber()

                    For rowIndex As Integer = 2 To lastRow
                        Dim documentNumber = worksheet.Cell(rowIndex, 1).GetString().Trim()
                        Dim documentTitle = worksheet.Cell(rowIndex, 2).GetString().Trim()

                        If String.IsNullOrWhiteSpace(documentNumber) Then
                            skippedCount += 1
                            Continue For
                        End If

                        If String.IsNullOrWhiteSpace(documentTitle) Then
                            skippedCount += 1
                            Continue For
                        End If

                        Dim neDate As Date? = GetExcelDateValue(worksheet.Cell(rowIndex, 3))
                        Dim manufacturerDate As Date? = GetExcelDateValue(worksheet.Cell(rowIndex, 4))

                        Dim exists = db.Documents.Any(Function(d) d.ProjectId = AppState.SelectedProjectId.Value AndAlso
                                                                  d.EnvironmentId = AppState.SelectedEnvironmentId.Value AndAlso
                                                                  d.DocumentNumber = documentNumber)

                        If exists Then
                            skippedCount += 1
                            Continue For
                        End If

                        Dim newDocument As New ProjectDocument With {
                            .ProjectId = AppState.SelectedProjectId.Value,
                            .EnvironmentId = AppState.SelectedEnvironmentId.Value,
                            .DocumentNumber = documentNumber,
                            .Title = documentTitle,
                            .CurrentRevision = "00",
                            .IssueDate = neDate,
                            .DateToManufacturer = manufacturerDate,
                            .Status = "NOT STARTED"
                        }

                        db.Documents.Add(newDocument)
                        db.SaveChanges()

                        Dim firstRevision As New DocumentRevision With {
                            .ProjectDocumentId = newDocument.Id,
                            .RevisionCode = "00",
                            .Description = "FIRST ISSUE",
                            .AssignedTo = "",
                            .InternalReceiveDate = neDate,
                            .DateExpected = Nothing,
                            .OfficialIssueDate = Nothing
                        }

                        db.DocumentRevisions.Add(firstRevision)
                        db.SaveChanges()

                        importedCount += 1
                    Next

                End Using
            End Using

            LoadDocumentsForCurrentSelection()
            AppState.RaiseDocumentsChanged()

        Catch ex As Exception
            MessageBox.Show(
                "Error while importing Excel file:" & Environment.NewLine & ex.Message,
                "Import Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            )
        End Try
    End Sub

    Private Function GetExcelDateValue(cell As IXLCell) As Date?
        Try
            If cell Is Nothing OrElse cell.IsEmpty() Then
                Return Nothing
            End If

            If cell.DataType = XLDataType.DateTime Then
                Return cell.GetDateTime().Date
            End If

            Dim rawValue = cell.GetString().Trim()

            If String.IsNullOrWhiteSpace(rawValue) Then
                Return Nothing
            End If

            Dim parsedDate As Date
            If Date.TryParse(rawValue, parsedDate) Then
                Return parsedDate.Date
            End If

            Return Nothing

        Catch
            Return Nothing
        End Try
    End Function

End Class
