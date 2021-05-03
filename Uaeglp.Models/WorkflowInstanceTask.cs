﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("WorkflowInstanceTask", Schema = "bbsf")]
    public partial class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
            Tmetasks = new HashSet<Tmetask>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("WorkflowInstanceID")]
        public int WorkflowInstanceId { get; set; }
        [Column("ActivityID")]
        public int ActivityId { get; set; }
        public bool IsCompleted { get; set; }
        [Column("TaskCorrelationID")]
        public Guid? TaskCorrelationId { get; set; }
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

        [ForeignKey(nameof(ActivityId))]
        [InverseProperty(nameof(WorkflowActivity.WorkflowInstanceTasks))]
        public virtual WorkflowActivity Activity { get; set; }
        [ForeignKey(nameof(WorkflowInstanceId))]
        [InverseProperty("WorkflowInstanceTasks")]
        public virtual WorkflowInstance WorkflowInstance { get; set; }
        [InverseProperty(nameof(Tmetask.WfinstanceTask))]
        public virtual ICollection<Tmetask> Tmetasks { get; set; }
    }
}