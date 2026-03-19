Imports System.ComponentModel

Public Class AvailableEnvironment
    Implements INotifyPropertyChanged

    Private _name As String
    Private _isEnabled As Boolean

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            OnPropertyChanged(NameOf(Name))
        End Set
    End Property

    Public Property IsEnabled As Boolean
        Get
            Return _isEnabled
        End Get
        Set(value As Boolean)
            _isEnabled = value
            OnPropertyChanged(NameOf(IsEnabled))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class