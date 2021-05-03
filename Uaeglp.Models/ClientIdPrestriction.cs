﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ClientIdPRestrictions")]
    public partial class ClientIdPrestriction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Provider { get; set; }
        [Column("Client_Id")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        [InverseProperty("ClientIdPrestrictions")]
        public virtual Client Client { get; set; }
    }
}