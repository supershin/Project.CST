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
        public virtual DbSet<tm_Project> tm_Project { get; set; } = null!;
        public virtual DbSet<tm_Unit> tm_Unit { get; set; } = null!;
        public virtual DbSet<tm_UnitType> tm_UnitType { get; set; } = null!;
        public virtual DbSet<tm_User> tm_User { get; set; } = null!;
        public virtual DbSet<tr_ProjectForm> tr_ProjectForm { get; set; } = null!;
        public virtual DbSet<tr_ProjectFormCheckList> tr_ProjectFormCheckList { get; set; } = null!;
        public virtual DbSet<tr_ProjectFormGroup> tr_ProjectFormGroup { get; set; } = null!;
        public virtual DbSet<tr_ProjectFormPackage> tr_ProjectFormPackage { get; set; } = null!;
        public virtual DbSet<tr_UnitForm> tr_UnitForm { get; set; } = null!;
        public virtual DbSet<tr_UnitForm_Action> tr_UnitForm_Action { get; set; } = null!;
        public virtual DbSet<tr_UnitForm_Action_Log> tr_UnitForm_Action_Log { get; set; } = null!;
        public virtual DbSet<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("data source=F-147;database=ConstrcutionTracking;Integrated Security=SSPI;persist security info=True;");
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
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.FormType)
                    .WithMany(p => p.tm_FormFormType)
                    .HasForeignKey(d => d.FormTypeID)
                    .HasConstraintName("FK_tm_Form_tm_Ext2");

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.tm_FormProjectType)
                    .HasForeignKey(d => d.ProjectTypeID)
                    .HasConstraintName("FK_tm_Form_tm_Ext");

                entity.HasOne(d => d.UnitType)
                    .WithMany(p => p.tm_FormUnitType)
                    .HasForeignKey(d => d.UnitTypeID)
                    .HasConstraintName("FK_tm_Form_tm_Ext1");
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

            modelBuilder.Entity<tm_Unit>(entity =>
            {
                entity.Property(e => e.UnitID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tm_Unit_tm_Project");

                entity.HasOne(d => d.UnitType)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.UnitTypeID)
                    .HasConstraintName("FK_tm_Unit_tm_Ext");
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

            modelBuilder.Entity<tr_ProjectForm>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.FormType)
                    .WithMany(p => p.tr_ProjectForm)
                    .HasForeignKey(d => d.FormTypeID)
                    .HasConstraintName("FK_tr_ProjectForm_tm_Ext");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tr_ProjectForm)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tr_ProjectForm_tr_ProjectForm");

                entity.HasOne(d => d.UnitType)
                    .WithMany(p => p.tr_ProjectForm)
                    .HasForeignKey(d => d.UnitTypeID)
                    .HasConstraintName("FK_tr_ProjectForm_tm_UnitType");
            });

            modelBuilder.Entity<tr_ProjectFormCheckList>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.tr_ProjectFormCheckList)
                    .HasForeignKey(d => d.PackageID)
                    .HasConstraintName("FK_tr_ProjectFormCheckList_tr_ProjectFormPackage");
            });

            modelBuilder.Entity<tr_ProjectFormGroup>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_ProjectFormGroup)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_ProjectFormGroup_tr_ProjectForm");
            });

            modelBuilder.Entity<tr_ProjectFormPackage>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_ProjectFormPackage)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_ProjectFormPackage_tr_ProjectFormGroup");
            });

            modelBuilder.Entity<tr_UnitForm>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_UnitForm_tr_ProjectForm");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Project");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.UnitID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Unit");
            });

            modelBuilder.Entity<tr_UnitForm_Action>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitForm_Action)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Action_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitForm_Action_Log>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitForm_Action_Log)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Action_Log_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitForm_Detail>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CheckList)
                    .WithMany(p => p.tr_UnitForm_Detail)
                    .HasForeignKey(d => d.CheckListID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_ProjectFormCheckList");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_UnitForm_Detail)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_ProjectForm");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_UnitForm_Detail)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_ProjectFormGroup");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.tr_UnitForm_Detail)
                    .HasForeignKey(d => d.PackageID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_ProjectFormPackage");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitForm_Detail)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_UnitForm");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
