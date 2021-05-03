﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("EResource")]
    public partial class Eresource
    {
        public Eresource()
        {
            EresourceLinks = new HashSet<EresourceLink>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column("DescriptionEN")]
        [StringLength(256)]
        public string DescriptionEn { get; set; }
        [Column("DescriptionAR")]
        [StringLength(256)]
        public string DescriptionAr { get; set; }
        [Column("CorrelationID")]
        public Guid? CorrelationId { get; set; }
        [Column("FolderID")]
        public int FolderId { get; set; }
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

        [ForeignKey(nameof(FolderId))]
        [InverseProperty("Eresources")]
        public virtual Folder Folder { get; set; }
        [InverseProperty(nameof(EresourceLink.Eresource))]
        public virtual ICollection<EresourceLink> EresourceLinks { get; set; }
    }
}