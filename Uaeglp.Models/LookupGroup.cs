// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("LookupGroup", Schema = "bbsf")]
    public partial class LookupGroup
    {
        public LookupGroup()
        {
            Lookups = new HashSet<Lookup>();
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

        [InverseProperty(nameof(Lookup.LookupGroup))]
        public virtual ICollection<Lookup> Lookups { get; set; }
    }
}