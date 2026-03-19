Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media
Imports Microsoft.EntityFrameworkCore

Public Class DashboardViewModel
    Implements INotifyPropertyChanged

    Private _totalDocuments As Integer
    Private _documentsIssued As Integer
    Private _inProgressDocuments As Integer
    Private _overdueDocuments As Integer
    Private _normalInProgressDocuments As Integer
    Private _remainingDocuments As Integer

    Private _documentsIssuedPercentage As Double
    Private _inProgressPercentage As Double
    Private _overduePercentage As Double

    Private _issuedSliceGeometry As Geometry
    Private _inProgressSliceGeometry As Geometry
    Private _overdueSliceGeometry As Geometry
    Private _remainingSliceGeometry As Geometry



    Public Sub New()
        LatestActivities = New ObservableCollection(Of DashboardActivityItem)()
    End Sub

    Public Property TotalDocuments As Integer
        Get
            Return _totalDocuments
        End Get
        Set(value As Integer)
            _totalDocuments = value
            OnPropertyChanged(NameOf(TotalDocuments))
        End Set
    End Property

    Public Property DocumentsIssued As Integer
        Get
            Return _documentsIssued
        End Get
        Set(value As Integer)
            _documentsIssued = value
            OnPropertyChanged(NameOf(DocumentsIssued))
        End Set
    End Property

    Public Property InProgressDocuments As Integer
        Get
            Return _inProgressDocuments
        End Get
        Set(value As Integer)
            _inProgressDocuments = value
            OnPropertyChanged(NameOf(InProgressDocuments))
        End Set
    End Property

    Public Property OverdueDocuments As Integer
        Get
            Return _overdueDocuments
        End Get
        Set(value As Integer)
            _overdueDocuments = value
            OnPropertyChanged(NameOf(OverdueDocuments))
        End Set
    End Property

    Public Property DocumentsIssuedPercentage As Double
        Get
            Return _documentsIssuedPercentage
        End Get
        Set(value As Double)
            _documentsIssuedPercentage = value
            OnPropertyChanged(NameOf(DocumentsIssuedPercentage))
        End Set
    End Property

    Public Property InProgressPercentage As Double
        Get
            Return _inProgressPercentage
        End Get
        Set(value As Double)
            _inProgressPercentage = value
            OnPropertyChanged(NameOf(InProgressPercentage))
        End Set
    End Property

    Public Property OverduePercentage As Double
        Get
            Return _overduePercentage
        End Get
        Set(value As Double)
            _overduePercentage = value
            OnPropertyChanged(NameOf(OverduePercentage))
        End Set
    End Property

    Public Property IssuedSliceGeometry As Geometry
        Get
            Return _issuedSliceGeometry
        End Get
        Set(value As Geometry)
            _issuedSliceGeometry = value
            OnPropertyChanged(NameOf(IssuedSliceGeometry))
        End Set
    End Property

    Public Property InProgressSliceGeometry As Geometry
        Get
            Return _inProgressSliceGeometry
        End Get
        Set(value As Geometry)
            _inProgressSliceGeometry = value
            OnPropertyChanged(NameOf(InProgressSliceGeometry))
        End Set
    End Property

    Public Property OverdueSliceGeometry As Geometry
        Get
            Return _overdueSliceGeometry
        End Get
        Set(value As Geometry)
            _overdueSliceGeometry = value
            OnPropertyChanged(NameOf(OverdueSliceGeometry))
        End Set
    End Property

    Public Property RemainingSliceGeometry As Geometry
        Get
            Return _remainingSliceGeometry
        End Get
        Set(value As Geometry)
            _remainingSliceGeometry = value
            OnPropertyChanged(NameOf(RemainingSliceGeometry))
        End Set
    End Property

    Public Property LatestActivities As ObservableCollection(Of DashboardActivityItem)

    Public Sub LoadData()
        Using db As New AppDbContext()

            Dim documentsQuery = db.Documents.AsQueryable()

            If AppState.SelectedProjectId.HasValue Then
                documentsQuery = documentsQuery.Where(Function(d) d.ProjectId = AppState.SelectedProjectId.Value)
            End If

            If AppState.SelectedEnvironmentId.HasValue Then
                documentsQuery = documentsQuery.Where(Function(d) d.EnvironmentId = AppState.SelectedEnvironmentId.Value)
            End If

            Dim documents = documentsQuery.ToList()
            Dim documentIds = documents.Select(Function(d) d.Id).ToList()

            Dim revisions = db.DocumentRevisions.
            Where(Function(r) documentIds.Contains(r.ProjectDocumentId)).
            ToList()

            Dim revisionIds = revisions.Select(Function(r) r.Id).ToList()

            Dim rounds = db.RevisionCommentRounds.
            Where(Function(c) revisionIds.Contains(c.DocumentRevisionId)).
            ToList()

            TotalDocuments = documents.Count
            DocumentsIssued = documents.Where(Function(d) NormalizeStatus(d.Status) = "ISSUED").Count()
            OverdueDocuments = CalculateOverdueDocuments(documents)

            _normalInProgressDocuments = documents.
            Where(Function(d) IsInProgressStatus(d.Status) AndAlso Not IsDocumentOverdue(d)).
            Count()

            InProgressDocuments = _normalInProgressDocuments

            _remainingDocuments = Math.Max(0, TotalDocuments - DocumentsIssued - InProgressDocuments - OverdueDocuments)

            If TotalDocuments > 0 Then
                DocumentsIssuedPercentage = Math.Round((DocumentsIssued / CDbl(TotalDocuments)) * 100, 1)
                InProgressPercentage = Math.Round((InProgressDocuments / CDbl(TotalDocuments)) * 100, 1)
                OverduePercentage = Math.Round((OverdueDocuments / CDbl(TotalDocuments)) * 100, 1)
            Else
                DocumentsIssuedPercentage = 0
                InProgressPercentage = 0
                OverduePercentage = 0
            End If

            BuildDonutChart()
            BuildLatestActivities(documents, revisions, rounds)
        End Using
    End Sub

    Private Function CalculateOverdueDocuments(documents As List(Of ProjectDocument)) As Integer
        Return documents.Where(Function(d) IsDocumentOverdue(d)).Count()
    End Function

    Private Function IsDocumentOverdue(doc As ProjectDocument) As Boolean
        If doc Is Nothing Then Return False

        If NormalizeStatus(doc.Status) = "ISSUED" Then
            Return False
        End If

        If doc.IssueDate.HasValue AndAlso doc.IssueDate.Value.Date < Date.Today Then
            Return True
        End If

        Return False
    End Function

    Private Sub BuildLatestActivities(documents As List(Of ProjectDocument),
                                      revisions As List(Of DocumentRevision),
                                      rounds As List(Of RevisionCommentRound))

        LatestActivities.Clear()

        Dim items = documents.
            Select(Function(doc)
                       Dim docRevisions = revisions.Where(Function(r) r.ProjectDocumentId = doc.Id).ToList()
                       Dim docRevisionIds = docRevisions.Select(Function(r) r.Id).ToList()
                       Dim docRounds = rounds.Where(Function(c) docRevisionIds.Contains(c.DocumentRevisionId)).ToList()

                       Dim lastDate = GetLastActivityDate(doc, docRevisions, docRounds)
                       Dim latestRevision = docRevisions.OrderByDescending(Function(r) r.RevisionCode).FirstOrDefault()

                       Return New With {
                           .Document = doc,
                           .LastDate = lastDate,
                           .LatestRevision = latestRevision
                       }
                   End Function).
            OrderByDescending(Function(x) x.LastDate).
            Take(3).
            ToList()

        For Each item In items
            LatestActivities.Add(New DashboardActivityItem With {
                .DocumentNumber = item.Document.DocumentNumber,
                .ShortAction = GetShortAction(item.Document.Status),
                .ActivityDescription = BuildActivityDescription(item.Document, item.LatestRevision),
                .ActivityTime = If(item.LastDate.HasValue, item.LastDate.Value.ToString("dd/MM/yyyy"), "-"),
                .StatusLabel = item.Document.Status,
                .UserName = "-"
            })
        Next
    End Sub

    Private Function GetLastActivityDate(doc As ProjectDocument,
                                         docRevisions As List(Of DocumentRevision),
                                         docRounds As List(Of RevisionCommentRound)) As Date?

        Dim dates As New List(Of Date)

        If doc.IssueDate.HasValue Then dates.Add(doc.IssueDate.Value)

        For Each rev In docRevisions
            If rev.InternalReceiveDate.HasValue Then dates.Add(rev.InternalReceiveDate.Value)
            If rev.DateExpected.HasValue Then dates.Add(rev.DateExpected.Value)
            If rev.OfficialIssueDate.HasValue Then dates.Add(rev.OfficialIssueDate.Value)
        Next

        For Each round In docRounds
            If round.DateReceived.HasValue Then dates.Add(round.DateReceived.Value)
            If round.CommentsDate.HasValue Then dates.Add(round.CommentsDate.Value)
        Next

        If dates.Any() Then
            Return dates.Max()
        End If

        Return Nothing
    End Function

    Private Function BuildActivityDescription(doc As ProjectDocument, rev As DocumentRevision) As String
        If rev Is Nothing Then
            Return "Current status: " & If(doc.Status, "-")
        End If

        Return "Revision " & rev.RevisionCode &
               " - " & If(rev.Description, "No description")
    End Function

    Private Function GetShortAction(status As String) As String
        Select Case NormalizeStatus(status)
            Case "ISSUED"
                Return "IS"
            Case "WORKING", "NOT STARTED"
                Return "WK"
            Case "UNDER INTERNAL REVIEW"
                Return "RV"
            Case "COMMENTS TO IMPLEMENT"
                Return "CM"
            Case Else
                Return "DC"
        End Select
    End Function

    Private Function IsInProgressStatus(status As String) As Boolean
        Dim s = NormalizeStatus(status)

        Return s = "NOT STARTED" OrElse
           s = "WORKING" OrElse
           s = "UNDER INTERNAL REVIEW" OrElse
           s = "COMMENTS TO IMPLEMENT" OrElse
           s = "READY FOR ISSUE"
    End Function

    Private Function NormalizeStatus(status As String) As String
        If String.IsNullOrWhiteSpace(status) Then Return ""
        Return status.Trim().ToUpperInvariant()
    End Function

    Private Sub BuildDonutChart()
        Dim total As Double = Math.Max(TotalDocuments, 1)

        Dim issuedAngle = 360.0 * (DocumentsIssued / total)
        Dim progressAngle = 360.0 * (_normalInProgressDocuments / total)
        Dim overdueAngle = 360.0 * (OverdueDocuments / total)
        Dim remainingAngle = 360.0 * (_remainingDocuments / total)

        Dim startAngle As Double = -90

        IssuedSliceGeometry = CreatePieSliceGeometry(startAngle, issuedAngle)
        startAngle += issuedAngle

        InProgressSliceGeometry = CreatePieSliceGeometry(startAngle, progressAngle)
        startAngle += progressAngle

        OverdueSliceGeometry = CreatePieSliceGeometry(startAngle, overdueAngle)
        startAngle += overdueAngle

        RemainingSliceGeometry = CreatePieSliceGeometry(startAngle, remainingAngle)
    End Sub

    Private Function CreatePieSliceGeometry(startAngle As Double, sweepAngle As Double) As Geometry
        If sweepAngle <= 0 Then
            Return System.Windows.Media.Geometry.Empty
        End If

        Dim centerX As Double = 120
        Dim centerY As Double = 120
        Dim radius As Double = 120

        Dim startRadians = startAngle * Math.PI / 180
        Dim endRadians = (startAngle + sweepAngle) * Math.PI / 180

        Dim startPoint As New Point(
        centerX + radius * Math.Cos(startRadians),
        centerY + radius * Math.Sin(startRadians))

        Dim endPoint As New Point(
        centerX + radius * Math.Cos(endRadians),
        centerY + radius * Math.Sin(endRadians))

        Dim isLargeArc = sweepAngle > 180

        Dim figure As New PathFigure With {
        .StartPoint = New Point(centerX, centerY),
        .IsClosed = True
    }

        figure.Segments.Add(New LineSegment(startPoint, True))
        figure.Segments.Add(New ArcSegment With {
        .Point = endPoint,
        .Size = New Size(radius, radius),
        .SweepDirection = SweepDirection.Clockwise,
        .IsLargeArc = isLargeArc
    })
        figure.Segments.Add(New LineSegment(New Point(centerX, centerY), True))

        Dim pathGeometry As New PathGeometry()
        pathGeometry.Figures.Add(figure)

        Return pathGeometry
    End Function

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class