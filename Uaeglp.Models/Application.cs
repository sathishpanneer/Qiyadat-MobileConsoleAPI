﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Application")]
    public partial class Application
    {
        public Application()
        {
            ApplicationAchievements = new HashSet<ApplicationAchievement>();
            ApplicationCycleLogs = new HashSet<ApplicationCycleLog>();
            ApplicationProgresses = new HashSet<ApplicationProgress>();
            ApplicationTrainings = new HashSet<ApplicationTraining>();
            ProfileBatchAssessments = new HashSet<ProfileBatchAssessment>();
            QuestionAnswers = new HashSet<QuestionAnswer>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("BatchID")]
        public int BatchId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
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
        [Column("StatusItemID")]
        public int? StatusItemId { get; set; }
        [Column("ReviewerID")]
        public int? ReviewerId { get; set; }
        [Column("SecurityItemID")]
        public int? SecurityItemId { get; set; }
        [Column("ReviewStatusItemID")]
        public int? ReviewStatusItemId { get; set; }
        [Column("AssessmentItemID")]
        public int? AssessmentItemId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalScore { get; set; }
        public int CompletionPercentage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmitTime { get; set; }
        [Column("VideoAssessmentStatusID")]
        public int? VideoAssessmentStatusId { get; set; }
        public int? BatchRank { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalAssessmentScore { get; set; }
        public bool IsRecordUpdated { get; set; }
        public bool? IsRouted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AssessmentCompletionDatetime { get; set; }
        public bool? IsCloned { get; set; }
        public bool IsShortlisted { get; set; }

        public int? ParticipationReferenceID { get; set; }


        [ForeignKey(nameof(AssessmentItemId))]
        [InverseProperty(nameof(LookupItem.ApplicationAssessmentItems))]
        public virtual LookupItem AssessmentItem { get; set; }
        [ForeignKey(nameof(BatchId))]
        [InverseProperty("Applications")]
        public virtual Batch Batch { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("Applications")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(ReviewStatusItemId))]
        [InverseProperty(nameof(LookupItem.ApplicationReviewStatusItems))]
        public virtual LookupItem ReviewStatusItem { get; set; }
        [ForeignKey(nameof(SecurityItemId))]
        [InverseProperty(nameof(LookupItem.ApplicationSecurityItems))]
        public virtual LookupItem SecurityItem { get; set; }
        [ForeignKey(nameof(StatusItemId))]
        [InverseProperty(nameof(LookupItem.ApplicationStatusItems))]
        public virtual LookupItem StatusItem { get; set; }
        [ForeignKey(nameof(VideoAssessmentStatusId))]
        [InverseProperty(nameof(LookupItem.ApplicationVideoAssessmentStatuses))]
        public virtual LookupItem VideoAssessmentStatus { get; set; }
        [InverseProperty("Application")]
        public virtual ScoringApplication ScoringApplication { get; set; }
        [InverseProperty(nameof(ApplicationAchievement.Application))]
        public virtual ICollection<ApplicationAchievement> ApplicationAchievements { get; set; }
        [InverseProperty(nameof(ApplicationCycleLog.Application))]
        public virtual ICollection<ApplicationCycleLog> ApplicationCycleLogs { get; set; }
        [InverseProperty(nameof(ApplicationProgress.Application))]
        public virtual ICollection<ApplicationProgress> ApplicationProgresses { get; set; }
        [InverseProperty(nameof(ApplicationTraining.Application))]
        public virtual ICollection<ApplicationTraining> ApplicationTrainings { get; set; }
        [InverseProperty(nameof(ProfileBatchAssessment.Application))]
        public virtual ICollection<ProfileBatchAssessment> ProfileBatchAssessments { get; set; }
        [InverseProperty(nameof(QuestionAnswer.Application))]
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
    }
}