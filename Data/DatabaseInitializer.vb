Public Class DatabaseInitializer

    Public Shared Sub Initialize()
        Using db As New AppDbContext()
            db.Database.EnsureCreated()
        End Using
    End Sub

End Class