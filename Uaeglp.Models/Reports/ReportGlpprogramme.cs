// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportGLPProgrammes", Schema = "report")]
    public partial class ReportGlpprogramme
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Column("NameEN")]
        public string NameEn { get; set; }
        [Column("NameAR")]
        public string NameAr { get; set; }
        public int TotalProfilesCount { get; set; }
        public int AcceptedParticipantsCount { get; set; }
        public int RejectedParticipantsCount { get; set; }
        [Column("ProgrammeID")]
        public int ProgrammeId { get; set; }
        public int SubmitionParticipantsCount { get; set; }
        public int ReviewParticipantsCount { get; set; }
        public int AssessmentParticipantsCount { get; set; }
        public int PhysicalAssessmentParticipantsCount { get; set; }
        public int ParticipantsTotalCount { get; set; }
        public int TotalMaleParticipants { get; set; }
        public int TotalFemaleParticipants { get; set; }
        [Column("ParticipantsAge0_25Count")]
        public int ParticipantsAge025count { get; set; }
        [Column("ParticipantsAge25_35Count")]
        public int ParticipantsAge2535count { get; set; }
        [Column("ParticipantsAge35_45Count")]
        public int ParticipantsAge3545count { get; set; }
        [Column("ParticipantsAge45_100Count")]
        public int ParticipantsAge45100count { get; set; }
        public int ParticipantsHigherDiplomasCount { get; set; }
        public int ParticipantsBachelorsCount { get; set; }
        public int ParticipantsMastersCount { get; set; }
        public int ParticipantsDoctoralsCount { get; set; }
        public int ParticipantsProfessionalCertificationsCount { get; set; }
        public int ParticipantsDiplomasCount { get; set; }
        public int ParticipantsAbuDhabiProfilesCount { get; set; }
        public int ParticipantsDubaiProfilesCount { get; set; }
        public int ParticipantsRasAlKhaimahProfilesCount { get; set; }
        public int ParticipantsSharjahProfilesCount { get; set; }
        public int ParticipantsUmmAlQuwainProfilesCount { get; set; }
        public int ParticipantsAjmanProfilesCount { get; set; }
        public int ParticipantsFujairahProfilesCount { get; set; }
        [Column("ParticipantsWorkExperince0_2Count")]
        public int ParticipantsWorkExperince02count { get; set; }
        [Column("ParticipantsWorkExperince2_5Count")]
        public int ParticipantsWorkExperince25count { get; set; }
        [Column("ParticipantsWorkExperince5_10Count")]
        public int ParticipantsWorkExperince510count { get; set; }
        [Column("ParticipantsWorkExperince10_50Count")]
        public int ParticipantsWorkExperince1050count { get; set; }
        public int ApplicationsAbuDhabiLocalSectorCount { get; set; }
        public int ApplicationsDubaiLocalSectorCount { get; set; }
        public int ApplicationsRasAlKhaimahLocalSectorCount { get; set; }
        public int ApplicationsPassedSecurityCount { get; set; }
        public int ApplicationsDidntPassedSecurityCount { get; set; }
        public int ApplicationCompletedAssessmentsCount { get; set; }
        public int ApplicationsDidntCompleteAssessmentsCount { get; set; }
        public int SectorFederalApplicationsCount { get; set; }
        public int SectorPrivateApplicationsCount { get; set; }
        public int SectorPrivateGovOwnersApplicationsCount { get; set; }
        public int SectorLocalApplicationsCount { get; set; }
        public int ApplicationsSharjahLocalSectorCount { get; set; }
        public int ApplicationsUmmAlQuwainLocalSectorCount { get; set; }
        public int ApplicationsAjmanLocalSectorCount { get; set; }
        public int ApplicationsFujairahLocalSectorCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ApplicationsDidntPassedSecurityPercentage { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ApplicationsPassedSecurityPercentage { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ApplicationsDidntCompleteAssessmentsPercentage { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ApplicationCompletedAssessmentsPercentage { get; set; }
        [Column("TopOrgNameEN1")]
        public string TopOrgNameEn1 { get; set; }
        [Column("TopOrgNameEN2")]
        public string TopOrgNameEn2 { get; set; }
        [Column("TopOrgNameEN3")]
        public string TopOrgNameEn3 { get; set; }
        [Column("TopOrgNameEN4")]
        public string TopOrgNameEn4 { get; set; }
        [Column("TopOrgNameEN5")]
        public string TopOrgNameEn5 { get; set; }
        [Column("TopOrgNameAR1")]
        public string TopOrgNameAr1 { get; set; }
        [Column("TopOrgNameAR2")]
        public string TopOrgNameAr2 { get; set; }
        [Column("TopOrgNameAR3")]
        public string TopOrgNameAr3 { get; set; }
        [Column("TopOrgNameAR4")]
        public string TopOrgNameAr4 { get; set; }
        [Column("TopOrgNameAR5")]
        public string TopOrgNameAr5 { get; set; }
        public int TopOrgCount1 { get; set; }
        public int TopOrgCount2 { get; set; }
        public int TopOrgCount3 { get; set; }
        public int TopOrgCount4 { get; set; }
        public int TopOrgCount5 { get; set; }
        [Column("FullNameEN")]
        public string FullNameEn { get; set; }
        [Column("FullNameAR")]
        public string FullNameAr { get; set; }
    }
}