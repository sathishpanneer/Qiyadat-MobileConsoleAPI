﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileMenuItem")]
    public partial class ProfileMenuItem
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public bool IsVisible { get; set; }
        [Required]
        public string SysName { get; set; }
        [Required]
        [Column("NameEN")]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        public string NameAr { get; set; }
    }
}