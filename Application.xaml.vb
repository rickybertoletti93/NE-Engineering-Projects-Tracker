Class Application

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)

        DatabaseInitializer.Initialize()
    End Sub
End Class
