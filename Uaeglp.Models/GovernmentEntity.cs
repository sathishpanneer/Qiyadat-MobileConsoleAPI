// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("GovernmentEntity")]
    public partial class GovernmentEntity
    {
        public GovernmentEntity()
        {
            Challenges = new HashSet<Challenge>();
            GovernmentEntityCoordinators = new HashSet<GovernmentEntityCoordinator>();
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
        [Column("EntityTypeItemID")]
        public int EntityTypeItemId { get; set; }
        public int? NumberOfEmployees { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [StringLength(50)]
        public string Website { get; set; }
        [StringLength(50)]
        public string Fax { get; set; }
        [Column("LogoID")]
        public Guid? LogoId { get; set; }
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

        [ForeignKey(nameof(EntityTypeItemId))]
        [InverseProperty(nameof(LookupItem.GovernmentEntities))]
        public virtual LookupItem EntityTypeItem { get; set; }
        [InverseProperty(nameof(Challenge.GovernmentEntity))]
        public virtual ICollection<Challenge> Challenges { get; set; }
        [InverseProperty(nameof(GovernmentEntityCoordinator.GovernmentEntity))]
        public virtual ICollection<GovernmentEntityCoordinator> GovernmentEntityCoordinators { get; set; }
    }
}