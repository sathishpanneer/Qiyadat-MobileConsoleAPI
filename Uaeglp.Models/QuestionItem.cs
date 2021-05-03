﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("QuestionItem")]
    public partial class QuestionItem
    {
        public QuestionItem()
        {
            ProfileQuestionItemScores = new HashSet<ProfileQuestionItemScore>();
            Qanswers = new HashSet<Qanswer>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        public string NameAr { get; set; }
        public bool IsNegativeDirection { get; set; }
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
        [Column("SubScaleID")]
        public int? SubScaleId { get; set; }
        [Column("ScaleID")]
        public int? ScaleId { get; set; }
        [Column("LevelID")]
        public int? LevelId { get; set; }
        [Column("EnglishTestTypeID")]
        public int? EnglishTestTypeId { get; set; }
        [Column("AssessmentToolID")]
        public int? AssessmentToolId { get; set; }
        public bool HasAssessmentDirect { get; set; }
        [Column("CompetencyID")]
        public int? CompetencyId { get; set; }
        [Column("QuestionheadID")]
        public int? QuestionheadId { get; set; }
        [Column("ImageID")]
        public int? ImageId { get; set; }
        //[Column("MobImageID")]
        //public int? MobImageId { get; set; }
        public int? TimeTaken { get; set; }

        [ForeignKey(nameof(AssessmentToolId))]
        [InverseProperty("QuestionItems")]
        public virtual AssessmentTool AssessmentTool { get; set; }
        [ForeignKey(nameof(CompetencyId))]
        [InverseProperty("QuestionItems")]
        public virtual Competency Competency { get; set; }
        [ForeignKey(nameof(EnglishTestTypeId))]
        [InverseProperty(nameof(LookupItem.QuestionItemEnglishTestTypes))]
        public virtual LookupItem EnglishTestType { get; set; }
        [ForeignKey(nameof(ImageId))]
        [InverseProperty(nameof(File.QuestionItems))]
        public virtual File Image { get; set; }
        [ForeignKey(nameof(LevelId))]
        [InverseProperty(nameof(LookupItem.QuestionItemLevels))]
        public virtual LookupItem Level { get; set; }
        [ForeignKey(nameof(QuestionheadId))]
        [InverseProperty(nameof(QuestionHead.QuestionItems))]
        public virtual QuestionHead Questionhead { get; set; }
        [ForeignKey(nameof(ScaleId))]
        [InverseProperty("QuestionItems")]
        public virtual Scale Scale { get; set; }
        [ForeignKey(nameof(SubScaleId))]
        [InverseProperty("QuestionItems")]
        public virtual SubScale SubScale { get; set; }
        [InverseProperty(nameof(ProfileQuestionItemScore.QuestionItem))]
        public virtual ICollection<ProfileQuestionItemScore> ProfileQuestionItemScores { get; set; }
        [InverseProperty(nameof(Qanswer.Question))]
        public virtual ICollection<Qanswer> Qanswers { get; set; }
    }
}