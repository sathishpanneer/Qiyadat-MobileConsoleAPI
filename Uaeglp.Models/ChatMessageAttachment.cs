﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ChatMessageAttachment", Schema = "bbsf")]
    public partial class ChatMessageAttachment
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("AttachmentTypeItemID")]
        public int AttachmentTypeItemId { get; set; }
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
        [Column("ChatMessage_ID")]
        public int? ChatMessageId { get; set; }

        [ForeignKey(nameof(AttachmentTypeItemId))]
        [InverseProperty(nameof(LookupItem.ChatMessageAttachments))]
        public virtual LookupItem AttachmentTypeItem { get; set; }
        [ForeignKey(nameof(ChatMessageId))]
        [InverseProperty("ChatMessageAttachments")]
        public virtual ChatMessage ChatMessage { get; set; }
    }
}