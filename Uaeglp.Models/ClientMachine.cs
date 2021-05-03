﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ClientMachine", Schema = "bbsf")]
    public partial class ClientMachine
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string MachineKey { get; set; }
        [StringLength(255)]
        public string MachineName { get; set; }
        [Column("InternalDNSName")]
        [StringLength(255)]
        public string InternalDnsname { get; set; }
        [Column("OS")]
        [StringLength(255)]
        public string Os { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastInitialized { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
    }
}