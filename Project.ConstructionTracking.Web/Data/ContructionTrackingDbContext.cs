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

        public virtual DbSet<temp_unit_400H007> temp_unit_400H007 { get; set; } = null!;
        public virtual DbSet<tm_BU> tm_BU { get; set; } = null!;
        public virtual DbSet<tm_DefectArea> tm_DefectArea { get; set; } = null!;
        public virtual DbSet<tm_DefectAreaType_Mapping> tm_DefectAreaType_Mapping { get; set; } = null!;
        public virtual DbSet<tm_DefectDescription> tm_DefectDescription { get; set; } = null!;
        public virtual DbSet<tm_DefectType> tm_DefectType { get; set; } = null!;
        public virtual DbSet<tm_Ext> tm_Ext { get; set; } = null!;
        public virtual DbSet<tm_ExtType> tm_ExtType { get; set; } = null!;
        public virtual DbSet<tm_Form> tm_Form { get; set; } = null!;
        public virtual DbSet<tm_FormCheckList> tm_FormCheckList { get; set; } = null!;
        public virtual DbSet<tm_FormGroup> tm_FormGroup { get; set; } = null!;
        public virtual DbSet<tm_FormPackage> tm_FormPackage { get; set; } = null!;
        public virtual DbSet<tm_FormType> tm_FormType { get; set; } = null!;
        public virtual DbSet<tm_ModelType> tm_ModelType { get; set; } = null!;
        public virtual DbSet<tm_Project> tm_Project { get; set; } = null!;
        public virtual DbSet<tm_QC_CheckList> tm_QC_CheckList { get; set; } = null!;
        public virtual DbSet<tm_QC_CheckListDetail> tm_QC_CheckListDetail { get; set; } = null!;
        public virtual DbSet<tm_Resource> tm_Resource { get; set; } = null!;
        public virtual DbSet<tm_Role> tm_Role { get; set; } = null!;
        public virtual DbSet<tm_Unit> tm_Unit { get; set; } = null!;
        public virtual DbSet<tm_UnitFormStatus> tm_UnitFormStatus { get; set; } = null!;
        public virtual DbSet<tm_UnitQCStatus> tm_UnitQCStatus { get; set; } = null!;
        public virtual DbSet<tm_UnitType> tm_UnitType { get; set; } = null!;
        public virtual DbSet<tm_User> tm_User { get; set; } = null!;
        public virtual DbSet<tm_Vendor> tm_Vendor { get; set; } = null!;
        public virtual DbSet<tr_Form_QCCheckList> tr_Form_QCCheckList { get; set; } = null!;
        public virtual DbSet<tr_ProjectModelForm> tr_ProjectModelForm { get; set; } = null!;
        public virtual DbSet<tr_QC_UnitCheckList> tr_QC_UnitCheckList { get; set; } = null!;
        public virtual DbSet<tr_QC_UnitCheckList_Action> tr_QC_UnitCheckList_Action { get; set; } = null!;
        public virtual DbSet<tr_QC_UnitCheckList_Defect> tr_QC_UnitCheckList_Defect { get; set; } = null!;
        public virtual DbSet<tr_QC_UnitCheckList_Detail> tr_QC_UnitCheckList_Detail { get; set; } = null!;
        public virtual DbSet<tr_QC_UnitCheckList_Resource> tr_QC_UnitCheckList_Resource { get; set; } = null!;
        public virtual DbSet<tr_RoleActionStatus> tr_RoleActionStatus { get; set; } = null!;
        public virtual DbSet<tr_UnitForm> tr_UnitForm { get; set; } = null!;
        public virtual DbSet<tr_UnitFormAction> tr_UnitFormAction { get; set; } = null!;
        public virtual DbSet<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; } = null!;
        public virtual DbSet<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; } = null!;
        public virtual DbSet<tr_UnitFormPackage> tr_UnitFormPackage { get; set; } = null!;
        public virtual DbSet<tr_UnitFormPassCondition> tr_UnitFormPassCondition { get; set; } = null!;
        public virtual DbSet<tr_UnitFormResource> tr_UnitFormResource { get; set; } = null!;
        public virtual DbSet<vw_UnitForm_Action> vw_UnitForm_Action { get; set; } = null!;
        public virtual DbSet<vw_UnitQC_Action> vw_UnitQC_Action { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
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

            modelBuilder.Entity<tm_DefectArea>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsContactCenter).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.tm_DefectArea)
                    .HasForeignKey(d => d.ProjectTypeID)
                    .HasConstraintName("FK_tm_DefectArea_tm_Ext");
            });

            modelBuilder.Entity<tm_DefectAreaType_Mapping>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.DefectArea)
                    .WithMany(p => p.tm_DefectAreaType_Mapping)
                    .HasForeignKey(d => d.DefectAreaID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tm_DefectAreaType_Mapping_tm_DefectArea");

                entity.HasOne(d => d.DefectType)
                    .WithMany(p => p.tm_DefectAreaType_Mapping)
                    .HasForeignKey(d => d.DefectTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tm_DefectAreaType_Mapping_tm_DefectType");
            });

            modelBuilder.Entity<tm_DefectDescription>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.DefectType)
                    .WithMany(p => p.tm_DefectDescription)
                    .HasForeignKey(d => d.DefectTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tm_DefectDescription_tm_DefectType");
            });

            modelBuilder.Entity<tm_DefectType>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");
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

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.tm_FormType)
                    .HasForeignKey(d => d.ProjectTypeID)
                    .HasConstraintName("FK_tm_FormType_tm_Ext");
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

            modelBuilder.Entity<tm_QC_CheckList>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.tm_QC_CheckListProjectType)
                    .HasForeignKey(d => d.ProjectTypeID)
                    .HasConstraintName("FK_tm_QC_CheckList_tm_Ext");

                entity.HasOne(d => d.QCType)
                    .WithMany(p => p.tm_QC_CheckListQCType)
                    .HasForeignKey(d => d.QCTypeID)
                    .HasConstraintName("FK_tm_QC_CheckList_tm_Ext1");
            });

            modelBuilder.Entity<tm_QC_CheckListDetail>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.QCCheckList)
                    .WithMany(p => p.tm_QC_CheckListDetail)
                    .HasForeignKey(d => d.QCCheckListID)
                    .HasConstraintName("FK_tm_QC_CheckListDetail_tm_QC_CheckList");
            });

            modelBuilder.Entity<tm_Resource>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_Role>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

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

                entity.HasOne(d => d.UnitStatus)
                    .WithMany(p => p.tm_UnitUnitStatus)
                    .HasForeignKey(d => d.UnitStatusID)
                    .HasConstraintName("FK_tm_Unit_tm_Ext1");

                entity.HasOne(d => d.UnitType)
                    .WithMany(p => p.tm_UnitUnitType)
                    .HasForeignKey(d => d.UnitTypeID)
                    .HasConstraintName("FK_tm_Unit_tm_Ext");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.tm_Unit)
                    .HasForeignKey(d => d.VendorID)
                    .HasConstraintName("FK_tm_Unit_tm_Vendor");
            });

            modelBuilder.Entity<tm_UnitFormStatus>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tm_UnitQCStatus>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
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

                entity.HasOne(d => d.BU)
                    .WithMany(p => p.tm_User)
                    .HasForeignKey(d => d.BUID)
                    .HasConstraintName("FK_tm_User_tm_BU");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.tm_User)
                    .HasForeignKey(d => d.DepartmentID)
                    .HasConstraintName("FK_tm_User_tm_Ext1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tm_User)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tm_User_tm_Ext");
            });

            modelBuilder.Entity<tr_Form_QCCheckList>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CheckList)
                    .WithMany(p => p.tr_Form_QCCheckList)
                    .HasForeignKey(d => d.CheckListID)
                    .HasConstraintName("FK_tr_Form_QCCheckList_tm_QC_CheckList");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_Form_QCCheckList)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_Form_QCCheckList_tm_Form");
            });

            modelBuilder.Entity<tr_ProjectModelForm>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tr_QC_UnitCheckList>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CheckList)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.CheckListID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_QC_CheckList");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_Project");

                entity.HasOne(d => d.QCSign)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.QCSignID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_User");

                entity.HasOne(d => d.QCSignResource)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.QCSignResourceID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_Resource");

                entity.HasOne(d => d.QCStatus)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.QCStatusID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tr_RoleActionStatus");

                entity.HasOne(d => d.QCType)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.QCTypeID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_Ext");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.tr_QC_UnitCheckList)
                    .HasForeignKey(d => d.UnitID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_tm_Unit");
            });

            modelBuilder.Entity<tr_QC_UnitCheckList_Action>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.QCUnitCheckList)
                    .WithMany(p => p.tr_QC_UnitCheckList_Action)
                    .HasForeignKey(d => d.QCUnitCheckListID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Action_tr_QC_UnitCheckList");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tr_QC_UnitCheckList_Action)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Action_tm_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.tr_QC_UnitCheckList_Action)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Action_tr_RoleActionStatus");
            });

            modelBuilder.Entity<tr_QC_UnitCheckList_Defect>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tr_QC_UnitCheckList_Detail>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CheckListDetail)
                    .WithMany(p => p.tr_QC_UnitCheckList_Detail)
                    .HasForeignKey(d => d.CheckListDetailID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Detail_tm_QC_CheckListDetail");

                entity.HasOne(d => d.QCUnitCheckList)
                    .WithMany(p => p.tr_QC_UnitCheckList_Detail)
                    .HasForeignKey(d => d.QCUnitCheckListID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Detail_tr_QC_UnitCheckList_Detail");
            });

            modelBuilder.Entity<tr_QC_UnitCheckList_Resource>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Defect)
                    .WithMany(p => p.tr_QC_UnitCheckList_Resource)
                    .HasForeignKey(d => d.DefectID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Resource_tr_QC_UnitCheckList_Defect");

                entity.HasOne(d => d.QCUnitCheckListDetail)
                    .WithMany(p => p.tr_QC_UnitCheckList_Resource)
                    .HasForeignKey(d => d.QCUnitCheckListDetailID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Resource_tr_QC_UnitCheckList_Detail");

                entity.HasOne(d => d.QCUnitCheckList)
                    .WithMany(p => p.tr_QC_UnitCheckList_Resource)
                    .HasForeignKey(d => d.QCUnitCheckListID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Resource_tr_QC_UnitCheckList");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.tr_QC_UnitCheckList_Resource)
                    .HasForeignKey(d => d.ResourceID)
                    .HasConstraintName("FK_tr_QC_UnitCheckList_Resource_tm_Resource");
            });

            modelBuilder.Entity<tr_RoleActionStatus>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tr_RoleActionStatus)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tr_RoleActionStatus_tm_Role");
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
                    .HasConstraintName("FK_tr_UnitForm_tm_Form");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Project");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Ext");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Ext1");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.UnitID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Unit");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.VendorID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Vendor");

                entity.HasOne(d => d.VendorResource)
                    .WithMany(p => p.tr_UnitForm)
                    .HasForeignKey(d => d.VendorResourceID)
                    .HasConstraintName("FK_tr_UnitForm_tm_Resource");
            });

            modelBuilder.Entity<tr_UnitFormAction>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tr_UnitFormAction)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tr_UnitFormAction_tm_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.tr_UnitFormAction)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_UnitFormAction_tr_RoleActionStatus");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormAction)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Action_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormActionLog>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_UnitFormActionLog)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_UnitFormActionLog_tm_FormGroup");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.tr_UnitFormActionLog)
                    .HasForeignKey(d => d.RoleID)
                    .HasConstraintName("FK_tr_UnitFormActionLog_tm_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.tr_UnitFormActionLog)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_UnitFormActionLog_tr_RoleActionStatus");

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

                entity.HasOne(d => d.CheckList)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.CheckListID)
                    .HasConstraintName("FK_tr_UnitFormCheckList_tm_FormCheckList");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_UnitFormCheckList_tm_Form");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_UnitFormCheckList_tm_FormGroup");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.PackageID)
                    .HasConstraintName("FK_tr_UnitFormCheckList_tm_FormPackage");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.StatusID)
                    .HasConstraintName("FK_tr_UnitFormCheckList_tm_Ext");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormCheckList)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitForm_Detail_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormPackage>(entity =>
            {
                entity.Property(e => e.UpdateBy).IsFixedLength();

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.tr_UnitFormPackage)
                    .HasForeignKey(d => d.FormID)
                    .HasConstraintName("FK_tr_UnitFormPackage_tm_Form");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_UnitFormPackage)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_UnitFormPackage_tm_FormGroup");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.tr_UnitFormPackage)
                    .HasForeignKey(d => d.PackageID)
                    .HasConstraintName("FK_tr_UnitFormPackage_tm_FormPackage");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormPackage)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitFormPackage_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormPassCondition>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.tr_UnitFormPassCondition)
                    .HasForeignKey(d => d.GroupID)
                    .HasConstraintName("FK_tr_UnitFormPassCondition_tm_FormGroup");

                entity.HasOne(d => d.LockStatus)
                    .WithMany(p => p.tr_UnitFormPassCondition)
                    .HasForeignKey(d => d.LockStatusID)
                    .HasConstraintName("FK_tr_UnitFormPassCondition_tm_Ext");

                entity.HasOne(d => d.UnitForm)
                    .WithMany(p => p.tr_UnitFormPassCondition)
                    .HasForeignKey(d => d.UnitFormID)
                    .HasConstraintName("FK_tr_UnitFormPassCondition_tr_UnitForm");
            });

            modelBuilder.Entity<tr_UnitFormResource>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlagActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PassCondition)
                    .WithMany(p => p.tr_UnitFormResource)
                    .HasForeignKey(d => d.PassConditionID)
                    .HasConstraintName("FK_tr_UnitFormResource_tr_UnitFormPassCondition");

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

            modelBuilder.Entity<vw_UnitForm_Action>(entity =>
            {
                entity.ToView("vw_UnitForm_Action");
            });

            modelBuilder.Entity<vw_UnitQC_Action>(entity =>
            {
                entity.ToView("vw_UnitQC_Action");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
