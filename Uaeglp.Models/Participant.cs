﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Participant", Schema = "bbsf")]
    public partial class Participant
    {
        public Participant()
        {
            ImpersonationRuleDestinationParticipants = new HashSet<ImpersonationRule>();
            ImpersonationRuleSourceParticipants = new HashSet<ImpersonationRule>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("DescriptionEN")]
        public string DescriptionEn { get; set; }
        [Column("DescriptionAR")]
        public string DescriptionAr { get; set; }
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
        [Column("ParticipantTypeItemID")]
        public int ParticipantTypeItemId { get; set; }
        [Column("WorkflowActivityID")]
        public int? WorkflowActivityId { get; set; }
        [Column("CorrelationID")]
        public Guid? CorrelationId { get; set; }
        [Column("PermissionSetID")]
        public int? PermissionSetId { get; set; }
        [Column("UserID")]
        public int? UserId { get; set; }
        [Column("WorkflowVariableID")]
        public int? WorkflowVariableId { get; set; }

        [ForeignKey(nameof(ParticipantTypeItemId))]
        [InverseProperty(nameof(LookupItem.Participants))]
        public virtual LookupItem ParticipantTypeItem { get; set; }
        [ForeignKey(nameof(PermissionSetId))]
        [InverseProperty("Participants")]
        public virtual PermissionSet PermissionSet { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Participants")]
        public virtual User User { get; set; }
        [ForeignKey(nameof(WorkflowActivityId))]
        [InverseProperty("Participants")]
        public virtual WorkflowActivity WorkflowActivity { get; set; }
        [ForeignKey(nameof(WorkflowVariableId))]
        [InverseProperty("Participants")]
        public virtual WorkflowVariable WorkflowVariable { get; set; }
        [InverseProperty(nameof(ImpersonationRule.DestinationParticipant))]
        public virtual ICollection<ImpersonationRule> ImpersonationRuleDestinationParticipants { get; set; }
        [InverseProperty(nameof(ImpersonationRule.SourceParticipant))]
        public virtual ICollection<ImpersonationRule> ImpersonationRuleSourceParticipants { get; set; }
    }
}