using FileGuard.Identity.DataAccess.Models;
using FileGuard.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = FileGuard.Identity.Models.File;

namespace FileGuard.Identity.DataAccess;

public class FileGuardDbContext(DbContextOptions<FileGuardDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<File> Files { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<UserFolder> UserFolders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // many to many User - File
        builder.Entity<UserFile>()
            .HasKey(uf => new { uf.UserId, uf.FileId });

        builder.Entity<UserFile>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.UserFiles)
            .HasForeignKey(uf => uf.UserId);

        builder.Entity<UserFile>()
            .HasOne(uf => uf.File)
            .WithMany(f => f.UserFiles)
            .HasForeignKey(uf => uf.FileId);

        // many to many User - Folder
        builder.Entity<UserFolder>()
            .HasKey(uf => new { uf.UserId, uf.FolderId });

        builder.Entity<UserFolder>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.UserFolders)
            .HasForeignKey(uf => uf.UserId);

        builder.Entity<UserFolder>()
            .HasOne(uf => uf.Folder)
            .WithMany(f => f.UserFolders)
            .HasForeignKey(uf => uf.FolderId);

        // one to many Folder - SubFolders
        builder.Entity<Folder>()
            .HasMany(f => f.SubFolders)
            .WithOne(f => f.ParentFolder)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        // one to many Folder - Files
        builder.Entity<Folder>()
            .HasMany(f => f.Files)
            .WithOne(fi => fi.Folder)
            .HasForeignKey(fi => fi.FolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
