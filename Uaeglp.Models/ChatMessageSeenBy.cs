// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ChatMessageSeenBy", Schema = "bbsf")]
    public partial class ChatMessageSeenBy
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("UserID")]
        public int UserId { get; set; }
        [Column("MessageID")]
        public int MessageId { get; set; }
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

        [ForeignKey(nameof(MessageId))]
        [InverseProperty(nameof(ChatMessage.ChatMessageSeenBies))]
        public virtual ChatMessage Message { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("ChatMessageSeenBies")]
        public virtual User User { get; set; }
    }
}