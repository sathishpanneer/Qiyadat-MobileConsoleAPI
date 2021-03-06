// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("PermissionSet_User", Schema = "bbsf")]
    public partial class PermissionSetUser
    {
        [Key]
        [Column("PermissionSetID")]
        public int PermissionSetId { get; set; }
        [Key]
        [Column("UserID")]
        public int UserId { get; set; }

        [ForeignKey(nameof(PermissionSetId))]
        [InverseProperty("PermissionSetUsers")]
        public virtual PermissionSet PermissionSet { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("PermissionSetUsers")]
        public virtual User User { get; set; }
    }
}