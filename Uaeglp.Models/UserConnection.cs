// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("UserConnection", Schema = "bbsf")]
    public partial class UserConnection
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("UserID")]
        public int UserId { get; set; }
        [Column("ConnectionID")]
        public string ConnectionId { get; set; }
        public string Agent { get; set; }
        [Column("IPAddress")]
        public string Ipaddress { get; set; }
        public bool IsOnline { get; set; }
        public string Form { get; set; }
        public string Identifier { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastConnected { get; set; }
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
        [Column("URL")]
        public string Url { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserConnections")]
        public virtual User User { get; set; }
    }
}