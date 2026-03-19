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
                Return New SolidColorBrush(Color.FromRgb(232, 244, 252))
            End If

            Return New SolidColorBrush(Colors.White)
        End Get
    End Property

    Public ReadOnly Property RowBorderBrush As Brush
        Get
            If NormalizeStatus(Status) = "ISSUED" Then
                Return New SolidColorBrush(Color.FromRgb(190, 220, 241))
            End If

            Return New SolidColorBrush(Color.FromRgb(222, 230, 238))
        End Get
    End Property

    Public ReadOnly Property StatusBackground As Brush
        Get
            Select Case NormalizeStatus(Status)
                Case "ISSUED"
                    Return New SolidColorBrush(Color.FromRgb(232, 244, 252))
                Case "READY FOR ISSUE"
                    Return New SolidColorBrush(Color.FromRgb(232, 241, 250))
                Case "UNDER INTERNAL REVIEW"
                    Return New SolidColorBrush(Color.FromRgb(255, 244, 224))
                Case "COMMENTS TO IMPLEMENT"
                    Return New SolidColorBrush(Color.FromRgb(252, 236, 238))
                Case "WORKING"
                    Return New SolidColorBrush(Color.FromRgb(235, 244, 251))
                Case Else
                    Return New SolidColorBrush(Color.FromRgb(244, 247, 250))
            End Select
        End Get
    End Property

    Public ReadOnly Property StatusForeground As Brush
        Get
            Select Case NormalizeStatus(Status)
                Case "ISSUED"
                    Return New SolidColorBrush(Color.FromRgb(31, 110, 165))
                Case "READY FOR ISSUE"
                    Return New SolidColorBrush(Color.FromRgb(43, 102, 153))
                Case "UNDER INTERNAL REVIEW"
                    Return New SolidColorBrush(Color.FromRgb(171, 111, 26))
                Case "COMMENTS TO IMPLEMENT"
                    Return New SolidColorBrush(Color.FromRgb(183, 83, 93))
                Case "WORKING"
                    Return New SolidColorBrush(Color.FromRgb(49, 115, 165))
                Case Else
                    Return New SolidColorBrush(Color.FromRgb(99, 114, 128))
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
            Return New SolidColorBrush(Color.FromRgb(119, 134, 149))
        End If

        If effectiveDate.Value.Date <= expectedDate.Value.Date Then
            Return New SolidColorBrush(Color.FromRgb(31, 110, 165))
        End If

        Return New SolidColorBrush(Color.FromRgb(201, 112, 69))
    End Function

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
