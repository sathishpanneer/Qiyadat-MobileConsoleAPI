﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    public partial class QuestionGroupsQuestion
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("GroupID")]
        public int GroupId { get; set; }
        [Column("QuestionID")]
        public int QuestionId { get; set; }
        public int QuestionOrder { get; set; }
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

        [ForeignKey(nameof(GroupId))]
        [InverseProperty(nameof(QuestionGroup.QuestionGroupsQuestions))]
        public virtual QuestionGroup Group { get; set; }
        [ForeignKey(nameof(QuestionId))]
        [InverseProperty("QuestionGroupsQuestions")]
        public virtual Question Question { get; set; }
    }
}