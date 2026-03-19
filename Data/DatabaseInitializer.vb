Imports System
Imports System.Data
Imports Microsoft.EntityFrameworkCore

Public Class DatabaseInitializer

    Public Shared Sub Initialize()
        Using db As New AppDbContext()
            db.Database.EnsureCreated()
            EnsureDocumentsSchema(db)
        End Using
    End Sub

    Private Shared Sub EnsureDocumentsSchema(db As AppDbContext)
        If Not ColumnExists(db, "Documents", "ManufacturerEffectiveDate") Then
            db.Database.ExecuteSqlRaw("ALTER TABLE Documents ADD COLUMN ManufacturerEffectiveDate TEXT NULL")
        End If
    End Sub

    Private Shared Function ColumnExists(db As AppDbContext, tableName As String, columnName As String) As Boolean
        Dim connection = db.Database.GetDbConnection()

        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If

        Using command = connection.CreateCommand()
            command.CommandText = "PRAGMA table_info(" & tableName & ")"

            Using reader = command.ExecuteReader()
                While reader.Read()
                    If String.Equals(reader("name").ToString(), columnName, StringComparison.OrdinalIgnoreCase) Then
                        Return True
                    End If
                End While
            End Using
        End Using

        Return False
    End Function

End Class
