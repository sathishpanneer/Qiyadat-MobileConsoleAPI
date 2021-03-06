// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("LibraryFolder", Schema = "bbsf")]
    public partial class LibraryFolder
    {
        public LibraryFolder()
        {
            Files = new HashSet<File>();
            InverseParentFolder = new HashSet<LibraryFolder>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(256)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(256)]
        public string NameAr { get; set; }
        [Column("ParentFolderID")]
        public int? ParentFolderId { get; set; }
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
        public string FolderPathAr { get; set; }
        public string FolderPathEn { get; set; }

        [ForeignKey(nameof(ParentFolderId))]
        [InverseProperty(nameof(LibraryFolder.InverseParentFolder))]
        public virtual LibraryFolder ParentFolder { get; set; }
        [InverseProperty(nameof(File.Folder))]
        public virtual ICollection<File> Files { get; set; }
        [InverseProperty(nameof(LibraryFolder.ParentFolder))]
        public virtual ICollection<LibraryFolder> InverseParentFolder { get; set; }
    }
}