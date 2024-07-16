using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_User
    {
        [Key]
        public Guid ID { get; set; }
        public int? BUID { get; set; }
        public int? DepartmentID { get; set; }
        public int? RoleID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Username { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Password { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? FirstName { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? LastName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Email { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Mobile { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("DepartmentID")]
        [InverseProperty("tm_UserDepartment")]
        public virtual tm_Ext? Department { get; set; }
        [ForeignKey("RoleID")]
        [InverseProperty("tm_UserRole")]
        public virtual tm_Ext? Role { get; set; }
    }
}
