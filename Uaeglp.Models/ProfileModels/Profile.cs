﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uaeglp.Models.ProfileModels;

namespace Uaeglp.Models
{
    [Table("Profile")]
    public partial class Profile
    {
        public Profile()
        {
            ApplicationAchievements = new HashSet<ApplicationAchievement>();
            ApplicationTrainings = new HashSet<ApplicationTraining>();
            Applications = new HashSet<Application>();
            AssessmentGroupMembers = new HashSet<AssessmentGroupMember>();
            AssignedAssessments = new HashSet<AssignedAssessment>();
            AssignmentAnswers = new HashSet<AssignmentAnswer>();
            CriteriaClaims = new HashSet<CriteriaClaim>();
            InitiativeProfiles = new HashSet<InitiativeProfile>();
            ParticipationReferences = new HashSet<ParticipationReference>();
            NetworkGroupProfiles = new HashSet<NetworkGroupProfile>();
            ProfileAchievements = new HashSet<ProfileAchievement>();
            ProfileAdminComments = new HashSet<ProfileAdminComment>();
            ProfileAssessmentToolScores = new HashSet<ProfileAssessmentToolScore>();
            ProfileAssignedAssessments = new HashSet<ProfileAssignedAssessment>();
            ProfileBatchAssessments = new HashSet<ProfileBatchAssessment>();
            ProfileCompetencyScores = new HashSet<ProfileCompetencyScore>();
            ProfileEducations = new HashSet<ProfileEducation>();
            ProfileEvents = new HashSet<ProfileEvent>();
            ProfileCourses = new HashSet<ProfileKnowledgeHubCourse>();
            ProfileFactorScores = new HashSet<ProfileFactorScore>();
            ProfileGroupAssessments = new HashSet<ProfileGroupAssessment>();
            ProfileGroups = new HashSet<ProfileGroup>();
            ProfileInterests = new HashSet<ProfileInterest>();
            ProfileLearningPreferences = new HashSet<ProfileLearningPreference>();
            ProfileMeetups = new HashSet<ProfileMeetup>();
            ProfileMemberships = new HashSet<ProfileMembership>();
            ProfilePillarScores = new HashSet<ProfilePillarScore>();
            ProfileQuestionItemScores = new HashSet<ProfileQuestionItemScore>();
            ProfileScaleScores = new HashSet<ProfileScaleScore>();
            ProfileSubAssessmentToolScores = new HashSet<ProfileSubAssessmentToolScore>();
            ProfileSubScaleScores = new HashSet<ProfileSubScaleScore>();
            ProfileTags = new HashSet<ProfileTag>();
            ProfileTrainings = new HashSet<ProfileTraining>();
            ProfileVideoAssessmentAnswerScores = new HashSet<ProfileVideoAssessmentAnswerScore>();
            ProfileVideoAssessmentCriteriaScores = new HashSet<ProfileVideoAssessmentCriteriaScore>();
            ProfileVideoAssessmentScores = new HashSet<ProfileVideoAssessmentScore>();
            ProfileWorkExperiences = new HashSet<ProfileWorkExperience>();
            QuestionAnswers = new HashSet<QuestionAnswer>();
            ScoringApplications = new HashSet<ScoringApplication>();
            SurveyProfileInfos = new HashSet<SurveyProfileInfo>();
            SurveyProfileQuestionAnswers = new HashSet<SurveyProfileQuestionAnswer>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PassportFileID")]
        public int? PassportFileId { get; set; }
        [Column("UAEIDFileID")]
        public int? UaeidfileId { get; set; }
        [Column("LastEducationCertificateFileID")]
        public int? LastEducationCertificateFileId { get; set; }
        [Column("CVFileID")]
        public int? CvfileId { get; set; }
        [Column("ResidenceCountryID")]
        public int? ResidenceCountryId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        [StringLength(50)]
        public string LandLine { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [Column("EID")]
        [StringLength(50)]
        public string Eid { get; set; }
        [StringLength(50)]
        public string PassportNumber { get; set; }
        [Column("TwitterURL")]
        [StringLength(2500)]
        public string TwitterUrl { get; set; }
        [Column("LinkedInURL")]
        [StringLength(2500)]
        public string LinkedInUrl { get; set; }
        public string ExpressYourself { get; set; }
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
        [Column("FirstNameEn")]
        [StringLength(25)]
        public string FirstNameEn { get; set; }
        [Column("SecondNameEn")]
        [StringLength(25)]
        public string SecondNameEn { get; set; }
        [Column("ThirdNameEn")]
        [StringLength(25)]
        public string ThirdNameEn { get; set; }
        [Column("LastNameEn")]
        [StringLength(25)]
        public string LastNameEn { get; set; }
        [Column("FirstNameAr")]
        [StringLength(25)]
        public string FirstNameAr { get; set; }
        [Column("SecondNameAr")]
        [StringLength(25)]
        public string SecondNameAr { get; set; }
        [Column("ThirdNameAr")]
        [StringLength(25)]
        public string ThirdNameAr { get; set; }
        [Column("LastNameAr")]
        [StringLength(25)]
        public string LastNameAr { get; set; }
        [Column("EmirateItemID")]
        public int? EmirateItemId { get; set; }
        [Column("PassportIssueEmirateItemID")]
        public int? PassportIssueEmirateItemId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalYearsOfExperinceWriteOnly { get; set; }
        public int? Age { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CompletenessPercentage { get; set; }
        public int PostsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        [Column("ExpressYourselfURL")]
        [StringLength(500)]
        public string ExpressYourselfUrl { get; set; }
        public bool IsInfluencer { get; set; }
        public bool IsPublicFigure { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ProfileLastModified { get; set; }
        [Column("FamilyBookFileID")]
        public int? FamilyBookFileId { get; set; }
        [Column("CitizenshipStatusID")]
        public int? CitizenshipStatusId { get; set; }
        [StringLength(100)]
        public string BusinessEmail { get; set; }
        public string UnifiedPassportNumber { get; set; }

        [Column("BadgeID")]
        public int? BadgeId { get; set; }
        [Column("LPSPoints")]
        public int Lpspoints { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TotalYearsOfExperinceLastUpdated { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalYearsOfExperinceReadOnly { get; set; }
        public string ExceptionalAspectsAndAchievements { get; set; }
        public string QRCode { get; set; }
        public string PassportUnifiedNumber { get; set; }

        [ForeignKey(nameof(BadgeId))]
        [InverseProperty("Profiles")]
        public virtual Badge Badge { get; set; }
        [ForeignKey(nameof(CitizenshipStatusId))]
        [InverseProperty(nameof(LookupItem.ProfileCitizenshipStatuses))]
        public virtual LookupItem CitizenshipStatus { get; set; }
        [ForeignKey(nameof(CvfileId))]
        [InverseProperty(nameof(File.ProfileCvfiles))]
        public virtual File Cvfile { get; set; }
        [ForeignKey(nameof(EmirateItemId))]
        [InverseProperty(nameof(LookupItem.ProfileEmirateItems))]
        public virtual LookupItem EmirateItem { get; set; }
        [ForeignKey(nameof(FamilyBookFileId))]
        [InverseProperty(nameof(File.ProfileFamilyBookFiles))]
        public virtual File FamilyBookFile { get; set; }
        [ForeignKey(nameof(Id))]
        [InverseProperty(nameof(User.Profile))]
        public virtual User IdNavigation { get; set; }
        [ForeignKey(nameof(LastEducationCertificateFileId))]
        [InverseProperty(nameof(File.ProfileLastEducationCertificateFiles))]
        public virtual File LastEducationCertificateFile { get; set; }
        [ForeignKey(nameof(PassportFileId))]
        [InverseProperty(nameof(File.ProfilePassportFiles))]
        public virtual File PassportFile { get; set; }
        [ForeignKey(nameof(PassportIssueEmirateItemId))]
        [InverseProperty(nameof(LookupItem.ProfilePassportIssueEmirateItems))]
        public virtual LookupItem PassportIssueEmirateItem { get; set; }
        [ForeignKey(nameof(ResidenceCountryId))]
        [InverseProperty(nameof(Country.Profiles))]
        public virtual Country ResidenceCountry { get; set; }
        [ForeignKey(nameof(UaeidfileId))]
        [InverseProperty(nameof(File.ProfileUaeidfiles))]
        public virtual File Uaeidfile { get; set; }
        [InverseProperty("Profile")]
        public virtual ScoringProfile ScoringProfile { get; set; }
        [InverseProperty(nameof(ApplicationAchievement.Profile))]
        public virtual ICollection<ApplicationAchievement> ApplicationAchievements { get; set; } 
        [InverseProperty(nameof(ParticipationReference.Profile))]
        public virtual ICollection<ParticipationReference> ParticipationReferences { get; set; }
        [InverseProperty(nameof(ApplicationTraining.Profile))]
        public virtual ICollection<ApplicationTraining> ApplicationTrainings { get; set; }
        [InverseProperty(nameof(Application.Profile))]
        public virtual ICollection<Application> Applications { get; set; }
        [InverseProperty(nameof(AssessmentGroupMember.Profile))]
        public virtual ICollection<AssessmentGroupMember> AssessmentGroupMembers { get; set; }
        [InverseProperty(nameof(AssignedAssessment.Profile))]
        public virtual ICollection<AssignedAssessment> AssignedAssessments { get; set; }
        [InverseProperty(nameof(AssignmentAnswer.Profile))]
        public virtual ICollection<AssignmentAnswer> AssignmentAnswers { get; set; }
        [InverseProperty(nameof(CriteriaClaim.Profile))]
        public virtual ICollection<CriteriaClaim> CriteriaClaims { get; set; }
        [InverseProperty(nameof(InitiativeProfile.Profile))]
        public virtual ICollection<InitiativeProfile> InitiativeProfiles { get; set; }
        [InverseProperty(nameof(NetworkGroupProfile.Profile))]
        public virtual ICollection<NetworkGroupProfile> NetworkGroupProfiles { get; set; }
        [InverseProperty(nameof(ProfileAchievement.Profile))]
        public virtual ICollection<ProfileAchievement> ProfileAchievements { get; set; }
        [InverseProperty(nameof(ProfileAdminComment.Profile))]
        public virtual ICollection<ProfileAdminComment> ProfileAdminComments { get; set; }
        [InverseProperty(nameof(ProfileAssessmentToolScore.Profile))]
        public virtual ICollection<ProfileAssessmentToolScore> ProfileAssessmentToolScores { get; set; }
        [InverseProperty(nameof(ProfileAssignedAssessment.Profile))]
        public virtual ICollection<ProfileAssignedAssessment> ProfileAssignedAssessments { get; set; }
        [InverseProperty(nameof(ProfileBatchAssessment.Profile))]
        public virtual ICollection<ProfileBatchAssessment> ProfileBatchAssessments { get; set; }
        [InverseProperty(nameof(ProfileCompetencyScore.Profile))]
        public virtual ICollection<ProfileCompetencyScore> ProfileCompetencyScores { get; set; }
        [InverseProperty(nameof(ProfileEducation.Profile))]
        public virtual ICollection<ProfileEducation> ProfileEducations { get; set; }
        [InverseProperty(nameof(ProfileEvent.Profile))]
        public virtual ICollection<ProfileEvent> ProfileEvents { get; set; }

       // [InverseProperty(nameof(ProfileKnowledgeHubCourse.Course))]
        public virtual ICollection<ProfileKnowledgeHubCourse> ProfileCourses { get; set; }

        [InverseProperty(nameof(ProfileFactorScore.Profile))]
        public virtual ICollection<ProfileFactorScore> ProfileFactorScores { get; set; }
        [InverseProperty(nameof(ProfileGroupAssessment.Profile))]
        public virtual ICollection<ProfileGroupAssessment> ProfileGroupAssessments { get; set; }
        [InverseProperty(nameof(ProfileGroup.Profile))]
        public virtual ICollection<ProfileGroup> ProfileGroups { get; set; }
        [InverseProperty(nameof(ProfileInterest.Profile))]
        public virtual ICollection<ProfileInterest> ProfileInterests { get; set; }
        [InverseProperty(nameof(ProfileLearningPreference.Profile))]
        public virtual ICollection<ProfileLearningPreference> ProfileLearningPreferences { get; set; }
        [InverseProperty(nameof(ProfileMeetup.Profile))]
        public virtual ICollection<ProfileMeetup> ProfileMeetups { get; set; }
        [InverseProperty(nameof(ProfileMembership.Profile))]
        public virtual ICollection<ProfileMembership> ProfileMemberships { get; set; }
        [InverseProperty(nameof(ProfilePillarScore.Profile))]
        public virtual ICollection<ProfilePillarScore> ProfilePillarScores { get; set; }
        [InverseProperty(nameof(ProfileQuestionItemScore.Profile))]
        public virtual ICollection<ProfileQuestionItemScore> ProfileQuestionItemScores { get; set; }
        [InverseProperty(nameof(ProfileScaleScore.Profile))]
        public virtual ICollection<ProfileScaleScore> ProfileScaleScores { get; set; }
        [InverseProperty(nameof(ProfileSubAssessmentToolScore.Profile))]
        public virtual ICollection<ProfileSubAssessmentToolScore> ProfileSubAssessmentToolScores { get; set; }
        [InverseProperty(nameof(ProfileSubScaleScore.Profile))]
        public virtual ICollection<ProfileSubScaleScore> ProfileSubScaleScores { get; set; }
        [InverseProperty(nameof(ProfileTag.Profile))]
        public virtual ICollection<ProfileTag> ProfileTags { get; set; }
        [InverseProperty(nameof(ProfileTraining.Profile))]
        public virtual ICollection<ProfileTraining> ProfileTrainings { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentAnswerScore.Profile))]
        public virtual ICollection<ProfileVideoAssessmentAnswerScore> ProfileVideoAssessmentAnswerScores { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentCriteriaScore.Profile))]
        public virtual ICollection<ProfileVideoAssessmentCriteriaScore> ProfileVideoAssessmentCriteriaScores { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentScore.Profile))]
        public virtual ICollection<ProfileVideoAssessmentScore> ProfileVideoAssessmentScores { get; set; }
        [InverseProperty(nameof(ProfileWorkExperience.Profile))]
        public virtual ICollection<ProfileWorkExperience> ProfileWorkExperiences { get; set; }
        [InverseProperty(nameof(QuestionAnswer.Profile))]
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
        [InverseProperty(nameof(ScoringApplication.Profile))]
        public virtual ICollection<ScoringApplication> ScoringApplications { get; set; }
        [InverseProperty(nameof(SurveyProfileInfo.Profile))]
        public virtual ICollection<SurveyProfileInfo> SurveyProfileInfos { get; set; }
        [InverseProperty(nameof(SurveyProfileQuestionAnswer.Profile))]
        public virtual ICollection<SurveyProfileQuestionAnswer> SurveyProfileQuestionAnswers { get; set; }
    }
}