﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("SurveyProfileQuestionAnswer")]
    public partial class SurveyProfileQuestionAnswer
    {
        public SurveyProfileQuestionAnswer()
        {
            SurveyProfileQuestionAnswerSurveyQuestionFields = new HashSet<SurveyProfileQuestionAnswerSurveyQuestionField>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ProfileID")]
        public int? ProfileId { get; set; }
        [Column("SurveyID")]
        public int SurveyId { get; set; }
        [Column("SurveryQusetionID")]
        public int SurveryQusetionId { get; set; }
        [Column("SurveryQuestionTypeItemID")]
        public int SurveryQuestionTypeItemId { get; set; }
        public string Value { get; set; }
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
        [Column("SurveyProfileInfoID")]
        public int SurveyProfileInfoId { get; set; }

        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("SurveyProfileQuestionAnswers")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(SurveryQuestionTypeItemId))]
        [InverseProperty(nameof(LookupItem.SurveyProfileQuestionAnswers))]
        public virtual LookupItem SurveryQuestionTypeItem { get; set; }
        [ForeignKey(nameof(SurveryQusetionId))]
        [InverseProperty(nameof(SurveyQuestion.SurveyProfileQuestionAnswers))]
        public virtual SurveyQuestion SurveryQusetion { get; set; }
        [ForeignKey(nameof(SurveyId))]
        [InverseProperty("SurveyProfileQuestionAnswers")]
        public virtual Survey Survey { get; set; }
        [ForeignKey(nameof(SurveyProfileInfoId))]
        [InverseProperty("SurveyProfileQuestionAnswers")]
        public virtual SurveyProfileInfo SurveyProfileInfo { get; set; }
        [InverseProperty(nameof(SurveyProfileQuestionAnswerSurveyQuestionField.SurveyProfileQuestionAnswer))]
        public virtual ICollection<SurveyProfileQuestionAnswerSurveyQuestionField> SurveyProfileQuestionAnswerSurveyQuestionFields { get; set; }
    }
}