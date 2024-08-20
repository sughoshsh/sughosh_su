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

    public virtual DbSet<CustomerDetail> CustomerDetails { get; set; }

    public virtual DbSet<Eodpublisher> Eodpublishers { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<TradeFlow> TradeFlows { get; set; }

    public virtual DbSet<TradingBook> TradingBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:finacle-sql-server.database.windows.net,1433;Initial Catalog=finacle-sqldb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        modelBuilder.Entity<CustomerDetail>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CustCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("custCode");
            entity.Property(e => e.CustLivePosition).HasColumnName("cust_Live_Position");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("custStatus");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Eodpublisher>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EODPublisher");

            entity.Property(e => e.EodDate).HasColumnName("eodDate");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.PubStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("pubStatus");
            entity.Property(e => e.SystemName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("systemName");
        });
        
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

        modelBuilder.Entity<TradeFlow>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TradeFlow");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.LoadDate).HasColumnName("loadDate");
            entity.Property(e => e.LoadStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("loadStatus");
            entity.Property(e => e.TradeId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("trade_id");
        });

        modelBuilder.Entity<TradingBook>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TradingBook");

            entity.Property(e => e.BookLivePosition).HasColumnName("book_Live_Position");
            entity.Property(e => e.BookName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("bookName");
            entity.Property(e => e.BookStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bookStatus");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

       OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
