// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    public partial class ClientSecret
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Value { get; set; }
        [StringLength(250)]
        public string Type { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        [Column("Client_Id")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        [InverseProperty("ClientSecrets")]
        public virtual Client Client { get; set; }
    }
}