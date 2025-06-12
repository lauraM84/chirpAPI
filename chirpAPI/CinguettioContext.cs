using System;
using System.Collections.Generic;
using chirpApi.Services.Model;
using Microsoft.EntityFrameworkCore;

namespace chirpAPI;

public partial class CinguettioContext : DbContext
{
    public CinguettioContext()
    {
    }

    public CinguettioContext(DbContextOptions<CinguettioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chirp> Chirps { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cinguettio;Username=postgres;Password=superpippo;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chirp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chirp_pk");

            entity.ToTable("chirps");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("creation_time");
            entity.Property(e => e.ExtUrl)
                .HasMaxLength(2083)
                .HasColumnName("ext_Url");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.Text)
                .HasMaxLength(140)
                .HasColumnName("text");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comment_pk");

            entity.ToTable("comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChirpId).HasColumnName("chirp_id");
            entity.Property(e => e.CreationTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_time");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Text)
                .HasMaxLength(140)
                .HasColumnName("text");

            entity.HasOne(d => d.Chirp).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ChirpId)
                .HasConstraintName("chirp_fk");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("parent_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
