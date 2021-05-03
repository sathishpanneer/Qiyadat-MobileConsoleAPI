﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Announcement")]
    public partial class Announcement
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("UserID")]
        public int UserId { get; set; }
        [Column("ImageArID")]
        public int ImageArId { get; set; }
        [Column("ImageEnID")]
        public int ImageEnId { get; set; }
        public string LinkAr { get; set; }
        public string LinkEn { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
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
        public string MobileNavigationField { get; set; }
        public bool Highlights { get; set; }
        public string MobileLinkNameEN { get; set; }
        public string MobileLinkNameAR { get; set; }
        public string MobileLinkDescriptionEN { get; set; }
        public string MobileLinkDescriptionAR { get; set; }


        [ForeignKey(nameof(UserId))]
        [InverseProperty("Announcements")]
        public virtual User User { get; set; }
    }
}