Imports System.Runtime.InteropServices

Imports System.ComponentModel

Public Class DashboardActivityItem
    Implements INotifyPropertyChanged

    Private _documentNumber As String
    Private _shortAction As String
    Private _activityDescription As String
    Private _activityTime As String
    Private _statusLabel As String
    Private _userName As String

    Public Property DocumentNumber As String
        Get
            Return _documentNumber
        End Get
        Set(value As String)
            _documentNumber = value
            OnPropertyChanged(NameOf(DocumentNumber))
        End Set
    End Property

    Public Property ShortAction As String
        Get
            Return _shortAction
        End Get
        Set(value As String)
            _shortAction = value
            OnPropertyChanged(NameOf(ShortAction))
        End Set
    End Property

    Public Property ActivityDescription As String
        Get
            Return _activityDescription
        End Get
        Set(value As String)
            _activityDescription = value
            OnPropertyChanged(NameOf(ActivityDescription))
        End Set
    End Property

    Public Property ActivityTime As String
        Get
            Return _activityTime
        End Get
        Set(value As String)
            _activityTime = value
            OnPropertyChanged(NameOf(ActivityTime))
        End Set
    End Property

    Public Property StatusLabel As String
        Get
            Return _statusLabel
        End Get
        Set(value As String)
            _statusLabel = value
            OnPropertyChanged(NameOf(StatusLabel))
        End Set
    End Property

    Public Property UserName As String
        Get
            Return _userName
        End Get
        Set(value As String)
            _userName = value
            OnPropertyChanged(NameOf(UserName))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class