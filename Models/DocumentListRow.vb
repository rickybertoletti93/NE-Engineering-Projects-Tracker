Imports System.ComponentModel
Imports System.Windows.Media

Public Class DocumentListRow
    Implements INotifyPropertyChanged

    Private _id As Integer
    Private _documentNumber As String
    Private _title As String
    Private _status As String
    Private _neDate As Date?
    Private _manufacturerDate As Date?
    Private _srlEffectiveDate As Date?
    Private _manufacturerEffectiveDate As Date?

    Public Property Id As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
            OnPropertyChanged(NameOf(Id))
        End Set
    End Property

    Public Property DocumentNumber As String
        Get
            Return _documentNumber
        End Get
        Set(value As String)
            _documentNumber = value
            OnPropertyChanged(NameOf(DocumentNumber))
        End Set
    End Property

    Public Property Title As String
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
            OnPropertyChanged(NameOf(Title))
        End Set
    End Property

    Public Property Status As String
        Get
            Return _status
        End Get
        Set(value As String)
            _status = value
            OnPropertyChanged(NameOf(Status))
            OnPropertyChanged(NameOf(StatusBackground))
            OnPropertyChanged(NameOf(StatusForeground))
            OnPropertyChanged(NameOf(StatusBorderBrush))
            OnPropertyChanged(NameOf(RowBackground))
            OnPropertyChanged(NameOf(RowBorderBrush))
        End Set
    End Property

    Public Property NEDate As Date?
        Get
            Return _neDate
        End Get
        Set(value As Date?)
            _neDate = value
            OnPropertyChanged(NameOf(NEDate))
            OnPropertyChanged(NameOf(SRLExpectedDate))
            OnPropertyChanged(NameOf(SRLResultDisplay))
            OnPropertyChanged(NameOf(SRLResultBrush))
            OnPropertyChanged(NameOf(RowBackground))
            OnPropertyChanged(NameOf(RowBorderBrush))
        End Set
    End Property

    Public Property ManufacturerDate As Date?
        Get
            Return _manufacturerDate
        End Get
        Set(value As Date?)
            _manufacturerDate = value
            OnPropertyChanged(NameOf(ManufacturerDate))
            OnPropertyChanged(NameOf(ManufacturerExpectedDate))
            OnPropertyChanged(NameOf(ManufacturerResultDisplay))
            OnPropertyChanged(NameOf(ManufacturerResultBrush))
            OnPropertyChanged(NameOf(RowBackground))
            OnPropertyChanged(NameOf(RowBorderBrush))
        End Set
    End Property

    Public Property SRLEffectiveDate As Date?
        Get
            Return _srlEffectiveDate
        End Get
        Set(value As Date?)
            _srlEffectiveDate = value
            OnPropertyChanged(NameOf(SRLEffectiveDate))
            OnPropertyChanged(NameOf(SRLResultDisplay))
            OnPropertyChanged(NameOf(SRLResultBrush))
            OnPropertyChanged(NameOf(RowBackground))
            OnPropertyChanged(NameOf(RowBorderBrush))
        End Set
    End Property

    Public Property ManufacturerEffectiveDate As Date?
        Get
            Return _manufacturerEffectiveDate
        End Get
        Set(value As Date?)
            _manufacturerEffectiveDate = value
            OnPropertyChanged(NameOf(ManufacturerEffectiveDate))
            OnPropertyChanged(NameOf(ManufacturerResultDisplay))
            OnPropertyChanged(NameOf(ManufacturerResultBrush))
            OnPropertyChanged(NameOf(RowBackground))
            OnPropertyChanged(NameOf(RowBorderBrush))
        End Set
    End Property

    Public ReadOnly Property SRLExpectedDate As Date?
        Get
            Return NEDate
        End Get
    End Property

    Public ReadOnly Property ManufacturerExpectedDate As Date?
        Get
            Return ManufacturerDate
        End Get
    End Property

    Public ReadOnly Property SRLResultDisplay As String
        Get
            Return FormatResult(SRLExpectedDate, SRLEffectiveDate)
        End Get
    End Property

    Public ReadOnly Property ManufacturerResultDisplay As String
        Get
            Return FormatResult(ManufacturerExpectedDate, ManufacturerEffectiveDate)
        End Get
    End Property

    Public ReadOnly Property SRLResultBrush As Brush
        Get
            Return GetResultBrush(SRLExpectedDate, SRLEffectiveDate)
        End Get
    End Property

    Public ReadOnly Property ManufacturerResultBrush As Brush
        Get
            Return GetResultBrush(ManufacturerExpectedDate, ManufacturerEffectiveDate)
        End Get
    End Property

    Public ReadOnly Property RowBackground As Brush
        Get
            If NormalizeStatus(Status) = "ISSUED" Then
                Return New SolidColorBrush(Color.FromRgb(20, 45, 34))
            End If

            If IsOverdue(SRLExpectedDate, SRLEffectiveDate) OrElse IsOverdue(ManufacturerExpectedDate, ManufacturerEffectiveDate) Then
                Return New SolidColorBrush(Color.FromRgb(58, 24, 28))
            End If

            Return New SolidColorBrush(Color.FromRgb(23, 29, 37))
        End Get
    End Property

    Public ReadOnly Property RowBorderBrush As Brush
        Get
            If NormalizeStatus(Status) = "ISSUED" Then
                Return New SolidColorBrush(Color.FromRgb(34, 197, 94))
            End If

            If IsOverdue(SRLExpectedDate, SRLEffectiveDate) OrElse IsOverdue(ManufacturerExpectedDate, ManufacturerEffectiveDate) Then
                Return New SolidColorBrush(Color.FromRgb(220, 38, 38))
            End If

            Return New SolidColorBrush(Color.FromRgb(35, 43, 54))
        End Get
    End Property

    Public ReadOnly Property StatusBackground As Brush
        Get
            Select Case NormalizeStatus(Status)
                Case "ISSUED"
                    Return New SolidColorBrush(Color.FromRgb(18, 59, 43))
                Case "READY FOR ISSUE"
                    Return New SolidColorBrush(Color.FromRgb(24, 56, 86))
                Case "UNDER INTERNAL REVIEW"
                    Return New SolidColorBrush(Color.FromRgb(74, 52, 20))
                Case "COMMENTS TO IMPLEMENT"
                    Return New SolidColorBrush(Color.FromRgb(82, 31, 35))
                Case "WORKING"
                    Return New SolidColorBrush(Color.FromRgb(33, 45, 66))
                Case Else
                    Return New SolidColorBrush(Color.FromRgb(39, 45, 56))
            End Select
        End Get
    End Property

    Public ReadOnly Property StatusForeground As Brush
        Get
            Select Case NormalizeStatus(Status)
                Case "ISSUED"
                    Return New SolidColorBrush(Color.FromRgb(110, 231, 183))
                Case "READY FOR ISSUE"
                    Return New SolidColorBrush(Color.FromRgb(147, 197, 253))
                Case "UNDER INTERNAL REVIEW"
                    Return New SolidColorBrush(Color.FromRgb(251, 191, 36))
                Case "COMMENTS TO IMPLEMENT"
                    Return New SolidColorBrush(Color.FromRgb(252, 165, 165))
                Case "WORKING"
                    Return New SolidColorBrush(Color.FromRgb(191, 219, 254))
                Case Else
                    Return New SolidColorBrush(Color.FromRgb(203, 213, 225))
            End Select
        End Get
    End Property

    Public ReadOnly Property StatusBorderBrush As Brush
        Get
            Return StatusForeground
        End Get
    End Property

    Private Function NormalizeStatus(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return ""
        Return value.Trim().ToUpperInvariant()
    End Function

    Private Function FormatResult(expectedDate As Date?, effectiveDate As Date?) As String
        If Not expectedDate.HasValue OrElse Not effectiveDate.HasValue Then
            Return "-"
        End If

        Dim lateDays = CInt((effectiveDate.Value.Date - expectedDate.Value.Date).TotalDays)

        If lateDays <= 0 Then
            Return "In Time"
        End If

        If lateDays = 1 Then
            Return "Late by 1 Day"
        End If

        Return "Late by " & lateDays.ToString() & " Days"
    End Function

    Private Function GetResultBrush(expectedDate As Date?, effectiveDate As Date?) As Brush
        If Not expectedDate.HasValue OrElse Not effectiveDate.HasValue Then
            Return New SolidColorBrush(Color.FromRgb(152, 162, 179))
        End If

        If effectiveDate.Value.Date <= expectedDate.Value.Date Then
            Return New SolidColorBrush(Color.FromRgb(110, 231, 183))
        End If

        Return New SolidColorBrush(Color.FromRgb(248, 113, 113))
    End Function

    Private Function IsOverdue(expectedDate As Date?, effectiveDate As Date?) As Boolean
        If Not expectedDate.HasValue Then
            Return False
        End If

        If effectiveDate.HasValue Then
            Return False
        End If

        Return expectedDate.Value.Date < Date.Today
    End Function

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
