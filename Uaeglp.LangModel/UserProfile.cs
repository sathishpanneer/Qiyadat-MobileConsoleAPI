﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Uaeglp.LangModel
{
    public partial class UserProfile
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
    }
}