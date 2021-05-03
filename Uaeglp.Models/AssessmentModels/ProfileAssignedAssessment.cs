﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileAssignedAssessment")]
    public partial class ProfileAssignedAssessment
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
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
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("AssignedAssessmentID")]
        public int AssignedAssessmentId { get; set; }
        [Column("ProfileAssessmenttoolID")]
        public int ProfileAssessmenttoolId { get; set; }

        [ForeignKey(nameof(AssignedAssessmentId))]
        [InverseProperty("ProfileAssignedAssessments")]
        public virtual AssignedAssessment AssignedAssessment { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileAssignedAssessments")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(ProfileAssessmenttoolId))]
        [InverseProperty(nameof(ProfileAssessmentToolScore.ProfileAssignedAssessments))]
        public virtual ProfileAssessmentToolScore ProfileAssessmenttool { get; set; }
    }
}