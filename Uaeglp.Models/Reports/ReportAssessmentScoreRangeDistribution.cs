﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportAssessmentScoreRangeDistribution", Schema = "report")]
    public partial class ReportAssessmentScoreRangeDistribution
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Column("AssessmentID")]
        public int AssessmentId { get; set; }
        [Column("AssessmentNameEN")]
        public string AssessmentNameEn { get; set; }
        [Column("AssessmentNameAR")]
        public string AssessmentNameAr { get; set; }
        public int FirstRangeCount { get; set; }
        public int SecondRangeCount { get; set; }
        public int ThirdRangeCount { get; set; }
        public int ForthRangeCount { get; set; }
        public int FifthRangeCount { get; set; }
    }
}