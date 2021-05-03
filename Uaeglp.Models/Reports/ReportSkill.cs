﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportSkills", Schema = "report")]
    public partial class ReportSkill
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int TotalProfilesCount { get; set; }
        public int TotalChoosedThisCount { get; set; }
        [Column("SkillID")]
        public int SkillId { get; set; }
        [Column("NameAR")]
        public string NameAr { get; set; }
        [Column("NameEN")]
        public string NameEn { get; set; }
    }
}