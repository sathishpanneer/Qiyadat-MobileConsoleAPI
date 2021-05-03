﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("AssessmentGroup")]
    public partial class AssessmentGroup
    {
        public AssessmentGroup()
        {
            AssessmentGroupMembers = new HashSet<AssessmentGroupMember>();
            ProfileGroupAssessments = new HashSet<ProfileGroupAssessment>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NameEN")]
        public string NameEn { get; set; }
        [Column("NameAR")]
        public string NameAr { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateFrom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTo { get; set; }
        public string Color { get; set; }
        [Column("AssessmentToolMatrixID")]
        public int AssessmentToolMatrixId { get; set; }
        [Column("LogoID")]
        public int LogoId { get; set; }
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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BestAssessmentScore { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AverageAssessmentScore { get; set; }

        [ForeignKey(nameof(AssessmentToolMatrixId))]
        [InverseProperty("AssessmentGroups")]
        public virtual AssessmentToolMatrix AssessmentToolMatrix { get; set; }
        [InverseProperty(nameof(AssessmentGroupMember.AssessmentGroup))]
        public virtual ICollection<AssessmentGroupMember> AssessmentGroupMembers { get; set; }
        [InverseProperty(nameof(ProfileGroupAssessment.Group))]
        public virtual ICollection<ProfileGroupAssessment> ProfileGroupAssessments { get; set; }
    }
}