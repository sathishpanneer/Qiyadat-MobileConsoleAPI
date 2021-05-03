﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportParticipantSectors", Schema = "report")]
    public partial class ReportParticipantSector
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int GovernmentSectorCount { get; set; }
        public int LocalSectorCount { get; set; }
        public int FedralSecorCount { get; set; }
        public int TotalPrivateSectorCount { get; set; }
        public int PrivateSectorCount { get; set; }
        public int PrivateGovCount { get; set; }
    }
}