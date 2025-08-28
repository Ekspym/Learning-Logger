using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LogService.Infrastructure.Contexts
{
    public partial class LoggerContext : DbContext
    {
        public virtual DbSet<AppLog> AppLogs { get; set; }

        public LoggerContext()
        {
        }

        public LoggerContext(DbContextOptions<LoggerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppLog>(entity =>
            {
                entity.ToTable("AppLog");

                entity.HasIndex(e => e.LogTypeId, "LogTypeID_idx");

                entity.HasIndex(e => e.VirtualMachineName, "VirtualMachineName_idx");

                entity.Property(e => e.AppLogId).HasColumnName("AppLogID");

                entity.Property(e => e.ApplicationName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LogTypeId).HasColumnName("LogTypeID");

                entity.Property(e => e.Message)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SessionId)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("SessionID");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VirtualMachineName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
