using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class IPodnikGaiaContext : DbContext
    {
        public virtual DbSet<AgentTask> AgentTasks { get; set; }
        public virtual DbSet<HostSystem> HostSystems { get; set; }
        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<TaskStatus> TaskStatuses { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        public IPodnikGaiaContext()
        {
        }

        public IPodnikGaiaContext(DbContextOptions<IPodnikGaiaContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgentTask>(entity =>
            {
                entity.ToTable("AgentTask");

                entity.HasIndex(e => e.MachineId, "IX_AgentTask_MachineId");

                entity.HasIndex(e => e.TaskStatusId, "IX_AgentTask_TaskStatusId");

                entity.HasIndex(e => e.TaskTypeId, "IX_AgentTask_TaskTypeId");

                entity.HasIndex(e => e.UserProfileId, "IX_AgentTask_UserProfileId");

                entity.Property(e => e.Comment).HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Result).HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Updated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.AgentTasks)
                    .HasForeignKey(d => d.MachineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AgentTask__MachineId");

                entity.HasOne(d => d.TaskStatus)
                    .WithMany(p => p.AgentTasks)
                    .HasForeignKey(d => d.TaskStatusId)
                    .HasConstraintName("FK_AgentTask_TaskStatusId");

                entity.HasOne(d => d.TaskType)
                    .WithMany(p => p.AgentTasks)
                    .HasForeignKey(d => d.TaskTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AgentTask__TaskTypeId");

                entity.HasOne(d => d.UserProfile)
                    .WithMany(p => p.AgentTasks)
                    .HasForeignKey(d => d.UserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AgentTask__UserProfileId");
            });

            modelBuilder.Entity<HostSystem>(entity =>
            {
                entity.ToTable("HostSystem");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.ToTable("Machine");

                entity.HasIndex(e => e.HostSystemId, "IX_Machine_HostSystemId");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(45)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Ram).HasColumnName("RAM");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.HostSystem)
                    .WithMany(p => p.Machines)
                    .HasForeignKey(d => d.HostSystemId)
                    .HasConstraintName("FK_Machine_HostSystemId");
            });

            modelBuilder.Entity<TaskStatus>(entity =>
            {
                entity.ToTable("TaskStatus");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TaskType>(entity =>
            {
                entity.ToTable("TaskType");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfile");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
