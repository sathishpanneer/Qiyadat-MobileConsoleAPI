﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileSubAssessmentToolScore")]
    public partial class ProfileSubAssessmentToolScore
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("SubAssessmenttoolID")]
        public int SubAssessmenttoolId { get; set; }
        [Column("AssessmentToolID")]
        public int AssessmentToolId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Score { get; set; }
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
        public int Order { get; set; }
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? StandardScore { get; set; }
        [Column("TScore", TypeName = "decimal(20, 10)")]
        public decimal? Tscore { get; set; }
        [Column("STENScore", TypeName = "decimal(20, 10)")]
        public decimal? Stenscore { get; set; }
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? Mean { get; set; }
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? StandardDeviation { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RecalculatedDatetime { get; set; }

        [ForeignKey(nameof(AssessmentToolId))]
        [InverseProperty("ProfileSubAssessmentToolScores")]
        public virtual AssessmentTool AssessmentTool { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileSubAssessmentToolScores")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(SubAssessmenttoolId))]
        [InverseProperty(nameof(SubAssessmentTool.ProfileSubAssessmentToolScores))]
        public virtual SubAssessmentTool SubAssessmenttool { get; set; }
    }
}