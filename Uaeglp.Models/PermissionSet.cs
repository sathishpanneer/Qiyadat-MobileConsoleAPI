﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("PermissionSet", Schema = "bbsf")]
    public partial class PermissionSet
    {
        public PermissionSet()
        {
            Adconnections = new HashSet<Adconnection>();
            Configurations = new HashSet<Configuration>();
            Participants = new HashSet<Participant>();
            PermissionSetPermissions = new HashSet<PermissionSetPermission>();
            PermissionSetUsers = new HashSet<PermissionSetUser>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [StringLength(64)]
        public string ActiveDirectoryGroup { get; set; }
        [Required]
        public byte[] RowVersion { get; set; }
        [Column("OrganizationID")]
        public int OrganizationId { get; set; }
        [Required]
        [StringLength(100)]
        public string SysName { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        [InverseProperty("PermissionSets")]
        public virtual Organization Organization { get; set; }
        [InverseProperty(nameof(Adconnection.PermissionSet))]
        public virtual ICollection<Adconnection> Adconnections { get; set; }
        [InverseProperty(nameof(Configuration.PermissionSet))]
        public virtual ICollection<Configuration> Configurations { get; set; }
        [InverseProperty(nameof(Participant.PermissionSet))]
        public virtual ICollection<Participant> Participants { get; set; }
        [InverseProperty(nameof(PermissionSetPermission.PermissionSet))]
        public virtual ICollection<PermissionSetPermission> PermissionSetPermissions { get; set; }
        [InverseProperty(nameof(PermissionSetUser.PermissionSet))]
        public virtual ICollection<PermissionSetUser> PermissionSetUsers { get; set; }
    }
}