using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class ContructionTrackingDbContext : DbContext
    {
        public ContructionTrackingDbContext()
        {
        }

        public ContructionTrackingDbContext(DbContextOptions<ContructionTrackingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tm_BU> tm_BU { get; set; } = null!;
        public virtual DbSet<tm_Ext> tm_Ext { get; set; } = null!;
        public virtual DbSet<tm_ExtType> tm_ExtType { get; set; } = null!;
        public virtual DbSet<tm_Form> tm_Form { get; set; } = null!;
        public virtual DbSet<tm_FormCheckList> tm_FormCheckList { get; set; } = null!;
        public virtual DbSet<tm_FormGroup> tm_FormGroup { get; set; } = null!;
        public virtual DbSet<tm_FormPackage> tm_FormPackage { get; set; } = null!;
        public virtual DbSet<tm_FormType> tm_FormType { get; set; } = null!;
        public virtual DbSet<tm_ModelType> tm_ModelType { get; set; } = null!;
        public virtual DbSet<tm_Project> tm_Project { get; set; } = null!;
        public virtual DbSet<tm_Resource> tm_Resource { get; set; } = null!;
        public virtual DbSet<tm_Unit> tm_Unit { get; set; } = null!;
        public virtual DbSet<tm_UnitType> tm_UnitType { get; set; } = null!;
        public virtual DbSet<tm_User> tm_User { get; set; } = null!;
        public virtual DbSet<tm_Vendor> tm_Vendor { get; set; } = null!;
        public virtual DbSet<tr_ProjectModelForm> tr_ProjectModelForm { get; set; } = null!;
        public virtual DbSet<tr_UnitForm> tr_UnitForm { get; set; } = null!;
        public virtual DbSet<tr_UnitFormAction> tr_UnitFormAction { get; set; } = null!;
        public virtual DbSet<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; } = null!;
        public virtual DbSet<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; } = null!;
        public virtual DbSet<tr_UnitFormGroup> tr_UnitFormGroup { get; set; } = null!;
        public virtual DbSet<tr_UnitFormPackage> tr_UnitFormPackage { get; set; } = null!;
        public virtual DbSet<tr_UnitFormResource> tr_UnitFormResource { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=10.0.20.14;Initial Catalog=ConstructionTracking;User ID=constructiontracking;Password=constructiontracking@2024;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Thai_CI_AS");

            modelBuilder.Entity<tm_BU>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_Ext>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ExtType)
                    .WithMany(p => p.tm_Ext)
                    .HasForeignKey(d => d.ExtTypeID)
                    .HasConstraintName("FK_tm_Ext_tm_ExtType");
            });

            modelBuilder.Entity<tm_ExtType>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_Form>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.FormType)
                    .WithMany(p => p.tm_Form)
                    .HasForeignKey(d => d.FormTypeID)
                    .HasConstraintName("FK_tm_Form_tm_FormType");
            });

            modelBuilder.Entity<tm_FormCheckList>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.tm_FormCheckList)
                    .HasForeignKey(d => d.PackageID)
                    .HasConstraintName("FK_tm_FormCheckList_tm_FormPackage");
            });

            modelBuilder.Entity<tm_FormGroup>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tm_FormGroup)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tm_FormPackage_tm_Form");
            });

            modelBuilder.Entity<tm_FormPackage>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tm_FormPackage)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tm_FormPackage_tm_FormGroup");
            });

            modelBuilder.Entity<tm_FormType>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_ModelType>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tm_ModelType)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tm_ModelType_tm_Project");
            });

            modelBuilder.Entity<tm_Project>(entity =>
            {
                entity.Property(e => e.ProjectID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.BU)
                    .WithMany(p => p.tm_Project)
                    .HasForeignKey(d => d.BUID)
                    .HasConstraintName("FK_tm_Project_tm_BU");

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.tm_Project)
                    .HasForeignKey(d => d.ProjectTypeID)
                    .HasConstraintName("FK_tm_Project_tm_Project");
            });

            modelBuilder.Entity<tm_Resource>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_Unit>(entity =>
            {
                entity.Property(e => e.UnitID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ModelType)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.ModelTypeID)
                    .HasConstraintName("FK_tm_Unit_tm_ModelType");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tm_Unit_tm_Project");

                entity.HasOne(d => d.UnitType)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.UnitTypeID)
                    .HasConstraintName("FK_tm_Unit_tm_Ext");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.VendorID)
                    .HasConstraintName("FK_tm_Unit_tm_Vendor");
            });

            modelBuilder.Entity<tm_UnitType>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_User>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.tm_UserDepartment)
                    .HasForeignKey(d => d.DepartmentID)
                    .HasConstraintName("FK_tm_User_tm_Ext1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tm_UserRole)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tm_User_tm_Ext");
            });

            modelBuilder.Entity<tr_ProjectModelForm>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModelTypeID).IsFixedLength();

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tr_UnitForm>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Project");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.UnitID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Unit");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.VendorID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Vendor");
            });

            modelBuilder.Entity<tr_UnitFormAction>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormAction)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Action_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormActionLog>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormActionLog)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Action_Log_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormCheckList>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormGroup>(entity =>
            {
                entity.Property(e => e.UpdateBy).IsFixedLength();
            });

            modelBuilder.Entity<tr_UnitFormPackage>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.UpdateBy).IsFixedLength();
            });

            modelBuilder.Entity<tr_UnitFormResource>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.tr_UnitFormResource)
                    .HasForeignKey(d => d.ResourceID)
                    .HasConstraintName("FK_tr_UnitForm_Resource_tm_Resource");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tr_UnitFormResource)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tr_UnitForm_Resource_tm_Ext");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormResource)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Resource_tr_UnitForm");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
