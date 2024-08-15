using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoreBot.Models;

public partial class FinacleSqldbContext : DbContext
{
    public FinacleSqldbContext()
    {
    }

    public FinacleSqldbContext(DbContextOptions<FinacleSqldbContext> options)
        : base(options)
    {
    }

   
    public virtual DbSet<Region> Regions { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:finacle-sql-server.database.windows.net,1433;Initial Catalog=finacle-sqldb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {    

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RegionalDisplayName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
