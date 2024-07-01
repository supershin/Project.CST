using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Ext
    {
        public tm_Ext()
        {
            tm_FormFormType = new HashSet<tm_Form>();
            tm_FormProjectType = new HashSet<tm_Form>();
            tm_FormUnitType = new HashSet<tm_Form>();
            tm_Project = new HashSet<tm_Project>();
            tm_Unit = new HashSet<tm_Unit>();
            tm_UserDepartment = new HashSet<tm_User>();
            tm_UserRole = new HashSet<tm_User>();
            tr_ProjectForm = new HashSet<tr_ProjectForm>();
            tr_UnitForm_Resource = new HashSet<tr_UnitForm_Resource>();
        }

        [Key]
        public int ID { get; set; }
        public int? ExtTypeID { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("ExtTypeID")]
        [InverseProperty("tm_Ext")]
        public virtual tm_ExtType? ExtType { get; set; }
        [InverseProperty("FormType")]
        public virtual ICollection<tm_Form> tm_FormFormType { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_Form> tm_FormProjectType { get; set; }
        [InverseProperty("UnitType")]
        public virtual ICollection<tm_Form> tm_FormUnitType { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_Project> tm_Project { get; set; }
        [InverseProperty("UnitType")]
        public virtual ICollection<tm_Unit> tm_Unit { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<tm_User> tm_UserDepartment { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tm_User> tm_UserRole { get; set; }
        [InverseProperty("FormType")]
        public virtual ICollection<tr_ProjectForm> tr_ProjectForm { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_UnitForm_Resource> tr_UnitForm_Resource { get; set; }
    }
}
