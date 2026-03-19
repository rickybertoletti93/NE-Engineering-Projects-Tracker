Imports Microsoft.EntityFrameworkCore

Public Class AppDbContext
    Inherits DbContext

    Public Property Projects As DbSet(Of Project)
    Public Property Environments As DbSet(Of ProjectEnvironment)
    Public Property Documents As DbSet(Of ProjectDocument)
    Public Property DocumentRevisions As DbSet(Of DocumentRevision)
    Public Property RevisionCommentRounds As DbSet(Of RevisionCommentRound)

    Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
        optionsBuilder.UseSqlite("Data Source=EngineeringTracker.db")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
        MyBase.OnModelCreating(modelBuilder)

        modelBuilder.Entity(Of Project)().
            HasMany(Function(p) p.Environments).
            WithOne(Function(e) e.Project).
            HasForeignKey(Function(e) e.ProjectId).
            OnDelete(DeleteBehavior.Cascade)

        modelBuilder.Entity(Of Project)().
            HasMany(Function(p) p.Documents).
            WithOne(Function(d) d.Project).
            HasForeignKey(Function(d) d.ProjectId).
            OnDelete(DeleteBehavior.Restrict)

        modelBuilder.Entity(Of ProjectEnvironment)().
            HasMany(Function(e) e.Documents).
            WithOne(Function(d) d.Environment).
            HasForeignKey(Function(d) d.EnvironmentId).
            OnDelete(DeleteBehavior.Cascade)

        modelBuilder.Entity(Of ProjectDocument)().
            HasMany(Function(d) d.Revisions).
            WithOne(Function(r) r.Document).
            HasForeignKey(Function(r) r.ProjectDocumentId).
            OnDelete(DeleteBehavior.Cascade)

        modelBuilder.Entity(Of DocumentRevision)().
            HasMany(Function(r) r.CommentRounds).
            WithOne(Function(c) c.Revision).
            HasForeignKey(Function(c) c.DocumentRevisionId).
            OnDelete(DeleteBehavior.Cascade)

    End Sub
End Class