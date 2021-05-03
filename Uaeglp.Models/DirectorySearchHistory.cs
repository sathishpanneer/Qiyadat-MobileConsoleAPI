﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("DirectorySearchHistory")]
    public partial class DirectorySearchHistory
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("UserID")]
        public int UserId { get; set; }
        public string Filtetrs { get; set; }
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
        public bool Favourite { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("DirectorySearchHistories")]
        public virtual User User { get; set; }
    }
}