using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio.DataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Models.ProfileModels;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using Profile = Uaeglp.Models.Profile;
using User = Uaeglp.MongoModels.User;

namespace Uaeglp.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IProfilePercentageCalculationService _completePercentageService;
        private readonly IMapper _mapper;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IEncryptionManager _Encryptor;
        private readonly IUserIPAddress _userIPAddress;
        private readonly CVParsingConfig _cvParsingConfig;

        public ProfileService(AppDbContext appDbContext, IMapper mapper, IEncryptionManager Encryption, MongoDbContext mongoDbContext, IProfilePercentageCalculationService completePercentageService, IPushNotificationService pushNotificationService, IUserIPAddress userIPAddress, IOptions<CVParsingConfig> cvParsingConfig)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _mongoDbContext = mongoDbContext;
            _completePercentageService = completePercentageService;
            _pushNotificationService = pushNotificationService;
            _Encryptor = Encryption;
            _userIPAddress = userIPAddress;
            _cvParsingConfig = cvParsingConfig.Value;
        }

        public async Task<IProfileResponse> GenerateUserQRCodeAsync(int userId)
        {
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                UseQRCode userQRCode = new UseQRCode();
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.Id == userId);
                if (profile == null)
                    throw new ArgumentException("Invalid Profile Id: " + userId);
                if (profile.QRCode == null)
                {
                    profile.QRCode = RandomNumber().ToString();
                    profile.Modified = DateTime.Now;
                    await _appDbContext.SaveChangesAsync();
                }

                var QRCodeData = "User-" + profile.QRCode;

                userQRCode.QRCodeData = QRCodeData;
                var shortcode = profile.Id - 100;
                userQRCode.ShortCode = shortcode.ToString();


                return new ProfileResponse(userQRCode);


            }
            catch (Exception e)
            {

                return new ProfileResponse(e);
            }
            //return null;
        }
        public int RandomNumber()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999);
        }
        public async Task<IProfileResponse> GetUserFavoriteProfilesAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                logger.Info($" {this.GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var mongoUser = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (mongoUser == null)
                {
                    mongoUser = new User() { Id = userId };
                    await _mongoDbContext.Users.InsertOneAsync(mongoUser);
                }
                var profileCount = _appDbContext.Profiles.Where(x => mongoUser.MyFavouriteProfilesIDs.Contains(x.Id)).ToList();
                var profiles = _appDbContext.Profiles.Where(x => mongoUser.MyFavouriteProfilesIDs.Contains(x.Id)).Skip(skip).Take(limit).ToList();
                var profileViews = new List<ProfileView>();
                foreach (var item in profiles)
                {
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == item.Id).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.Id);


                    profileViews.Add(new ProfileView
                    {
                        Id = item.Id,
                        FirstNameAr = item.FirstNameAr,
                        FirstNameEn = item.FirstNameEn,
                        LastNameAr = item.LastNameAr,
                        LastNameEn = item.LastNameEn,
                        DesignationEn = workExperience?.Title?.TitleEn,
                        DesignationAr = workExperience?.Title?.TitleAr,
                        UserImageFileId = user?.OriginalImageFileId ?? 0,

                    });
                }

                //return new ProfileResponse(profileViews);
                return new ProfileResponse(new FavoriteProfile()
                {
                    Profiles = profileViews,
                    TotalCount = profileCount.Count()
                });
            }
            catch (Exception e)
            {
                return new ProfileResponse(e);
            }
        }

        public async Task<IProfileResponse> AddUserFavoriteProfileAsync(int userId, int profileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} profileId : {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new ProfileResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                user.MyFavouriteProfilesIDs.Add(profileId);

                await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
                return new ProfileResponse();
            }
            catch (Exception e)
            {
                return new ProfileResponse(e);
            }
        }

        public async Task<IProfileResponse> UpdateProfileFollowersCountAsync(int profileId, bool increment)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
                if (profile == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                if (increment)
                    profile.FollowersCount++;
                else
                    profile.FollowersCount--;

                await _appDbContext.SaveChangesAsync();
                return new ProfileResponse(new ProfileView { Id = profile.Id });
            }
            catch (System.Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task<IProfileResponse> UpdateProfileFollowingCountAsync(int profileId, bool increment)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
                if (profile == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                if (increment)
                    profile.FollowingCount++;
                else
                    profile.FollowingCount--;

                await _appDbContext.SaveChangesAsync();
                return new ProfileResponse(new ProfileView { Id = profile.Id });
            }
            catch (System.Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task<IProfileResponse> GetPersonalInfoAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                var userInfo = await _appDbContext.UserInfos.Include(m => m.User)
                    .FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                //var profileLanguages = await _appDbContext.ProfileLanguage.Include(k => k.LookupLanguage)
                //    .Include(m => m.LookupProficiency).Where(k => k.ProfileId == userId).ToListAsync();


                return new ProfileResponse(new PersonalInfoView()
                {
                    UserId = profile.Id,
                    BirthDate = profile.BirthDate,
                    UserImageFileId = userInfo.User.OriginalImageFileId ?? 0,
                    Email = userInfo?.Email,
                    EmiratesId = profile.Eid,
                    GenderItemId = userInfo?.User.GenderItemId ?? 0,
                    PhoneNumber = userInfo?.Mobile,
                    // LanguageKnown = _mapper.Map<List<LanguageItemView>>(profileLanguages),
                    FullNameEN = userInfo?.User?.NameEn,
                    FullNameAR = userInfo?.User?.NameAr
                });
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task<IProfileResponse> UpdatePersonalInfoAsync(PersonalInfoView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId);
                    if (profile == null)
                    {
                        return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                    }
                    profile.BirthDate = model.BirthDate;
                    await _appDbContext.SaveChangesAsync();

                    var userInfo = await _appDbContext.UserInfos.Include(m => m.User).FirstOrDefaultAsync(k => k.Id == model.UserId);
                    userInfo.User.GenderItemId = model.GenderItemId;
                    userInfo.Mobile = model.PhoneNumber;
                    userInfo.Email = model.Email;
                    await _appDbContext.SaveChangesAsync();


                    await transaction.CommitAsync();
                }

                return new ProfileResponse(model);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> AddOrUpdateKnownLanguageAsync(LanguageItemView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                ProfileLanguage lang;
                if (model.Id == 0)
                {
                    lang = new ProfileLanguage()
                    {
                        LanguageItemId = model.LanguageItemId,
                        ProficiencyItemId = model.ProficiencyItemId,
                        ProfileId = model.ProfileId
                    };

                    await _appDbContext.ProfileLanguage.AddAsync(lang);
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    lang = await _appDbContext.ProfileLanguage.FirstOrDefaultAsync(k => k.Id == model.Id);
                    lang.LanguageItemId = model.LanguageItemId;
                    lang.ProficiencyItemId = model.ProficiencyItemId;
                    await _appDbContext.SaveChangesAsync();
                }
                model.Id = lang.Id;
                return new ProfileResponse(model);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }


        }

        public async Task<IProfileResponse> DeleteKnownLanguageAsync(int profileId, int id)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var lang = await _appDbContext.ProfileLanguage.FirstOrDefaultAsync(k => k.ProfileId == profileId && k.Id == id);

                if (lang != null)
                {
                    _appDbContext.ProfileLanguage.Remove(lang);
                    await _appDbContext.SaveChangesAsync();
                }

                return new ProfileResponse();
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> GetProfileNameAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null)
                {
                    return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                return new ProfileResponse(new ProfileNameView()
                {
                    FirstNameAR = profile.FirstNameAr,
                    FirstNameEN = profile.FirstNameEn,
                    LastNameAR = profile.LastNameAr,
                    LastNameEN = profile.LastNameEn,
                    SecondNameAR = profile.SecondNameAr,
                    SecondNameEN = profile.SecondNameEn,
                    ThirdNameAR = profile.ThirdNameAr,
                    ThirdNameEN = profile.ThirdNameEn,
                    UserId = profile.Id
                });
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }


        }

        public async Task<IProfileResponse> SearchPublicProfilesAsync(string text, int skip = 0, int limit = 5)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  text: {text} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                //text = text.Trim().ToLower().Replace(" ","");
                var name = text != null ? text.Trim().Split(new char[0]) : null;

                //var profiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(fullname[0]) || x.FirstNameEn.StartsWith(fullname[1])
                //text.Contains(x.FirstNameAr.Trim().ToLower()) ||
                //text.Contains(x.SecondNameAr.Trim().ToLower()) ||
                //text.Contains(x.ThirdNameAr.Trim().ToLower()) ||
                //text.Contains(x.LastNameAr.Trim().ToLower()) ||
                //text.Contains(x.FirstNameEn.Trim().ToLower()) ||
                //text.Contains(x.SecondNameEn.Trim().ToLower()) ||
                //text.Contains(x.ThirdNameEn.Trim().ToLower()) ||
                //text.Contains(x.LastNameEn.Trim().ToLower())).Skip(skip).Take(limit).ToListAsync();

                List<Profile> profiles = new List<Profile>();
                if (name != null)
                {
                    if (name.Length == 1)
                    {
                        profiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(name[0]) || x.FirstNameAr.StartsWith(name[0])).ToListAsync();
                    }
                    else if (name.Length == 2)
                    {
                        profiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(name[0]) || x.FirstNameAr.StartsWith(name[0]) ||
                                                                                x.LastNameEn.StartsWith(name[1]) || x.LastNameEn.StartsWith(name[1])).ToListAsync();
                    }
                    else if (name.Length == 3)
                    {
                        profiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(name[0]) || x.FirstNameAr.StartsWith(name[0]) ||
                                                                                x.SecondNameEn.StartsWith(name[1]) || x.SecondNameAr.StartsWith(name[1]) ||
                                                                                x.LastNameEn.StartsWith(name[2]) || x.LastNameEn.StartsWith(name[2])).ToListAsync();
                    }
                }

                //var profileCount = await _appDbContext.Profiles.Where(x =>
                //text.Contains(x.FirstNameAr.Trim().ToLower()) ||
                //text.Contains(x.SecondNameAr.Trim().ToLower()) ||
                //text.Contains(x.ThirdNameAr.Trim().ToLower()) ||
                //text.Contains(x.LastNameAr.Trim().ToLower()) ||
                //text.Contains(x.FirstNameEn.Trim().ToLower()) ||
                //text.Contains(x.SecondNameEn.Trim().ToLower()) ||
                //text.Contains(x.ThirdNameEn.Trim().ToLower()) ||
                //text.Contains(x.LastNameEn.Trim().ToLower())).ToListAsync();
                var profileViews = new List<MyProfileView>();
                foreach (var profile in profiles)
                {
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == profile.Id);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                    .Where(k => k.ProfileId == profile.Id).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();

                    profileViews.Add(new MyProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAR = profile.FirstNameAr,
                        FirstNameEN = profile.FirstNameEn,
                        LastNameAR = profile.LastNameAr,
                        LastNameEN = profile.LastNameEn,
                        SecondNameAR = profile.SecondNameAr,
                        SecondNameEN = profile.SecondNameEn,
                        ThirdNameAR = profile.ThirdNameAr,
                        ThirdNameEN = profile.ThirdNameEn,

                        FollowersCount = profile.FollowersCount,
                        FollowingCount = profile.FollowingCount,
                        PostCount = profile.PostsCount,

                        LPSPoint = profile.Lpspoints,
                        CompletePercentage = profile.CompletenessPercentage,

                        IsInfluencer = profile.IsInfluencer,
                        IsPublicFigure = profile.IsPublicFigure,

                        Bio = new BioView()
                        {
                            ExpressYourSelf = profile.ExpressYourself,
                            UserId = profile.Id,
                            ExpressYourSelfURL = string.IsNullOrWhiteSpace(profile.ExpressYourselfUrl) ? "" : $"/api/File/get-download-video/{profile.Id}"
                        },
                        UserImageFileId = user?.OriginalImageFileId ?? 0,

                        Designation = workExperience?.Title?.TitleEn,
                        DesignationAr = workExperience?.Title?.TitleAr,
                    });
                }

                //profileViews
                //return new ProfileResponse(profileViews);
                return new ProfileResponse(new SearchPublicProfileView()
                {
                    Recommendations = profileViews.Skip(skip).Take(limit).ToList(),
                    TotalCount = profileViews.Count()
                });
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task<IProfileResponse> GetPublicProfileAsync(int userId, int publicProfileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid: {userId}  publicProfileId: {publicProfileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var myProfile = await GetMyProfileAsync(publicProfileId);
                if (!myProfile.Success)
                {
                    return myProfile;
                }

                var users = await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync();
                myProfile.MyProfileView.IsAmFollowing = users.FollowingIDs?.Contains(publicProfileId) ?? false;
                myProfile.MyProfileView.IsFavoritePerson =
                    users.MyFavouriteProfilesIDs?.Contains(publicProfileId) ?? false;
                var data = myProfile.MyProfileView.SkillAndInterest;

                foreach (var skill in data.ProfileSkillItems)
                {
                    skill.IsEndorsed = await _appDbContext.ProfileSkillEndorsements.AnyAsync(k =>
                        k.ProfileId == publicProfileId && k.ProfileSkillId == skill.Id &&
                        k.PublicProfileId == userId);
                }
                var rec = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == publicProfileId && x.isAccepted).FirstOrDefaultAsync();
                if (rec != null)
                {
                    var userRecommend = _mapper.Map<UserRecommendationModelView>(rec);
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userRecommend.SenderUserID);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == userRecommend.SenderUserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == userRecommend.SenderUserID);
                    userRecommend.SenderInfo = new PublicProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAr = profile.FirstNameAr ?? "",
                        FirstNameEn = profile.FirstNameEn ?? "",
                        LastNameAr = profile.LastNameAr ?? "",
                        LastNameEn = profile.LastNameEn ?? "",
                        SecondNameAr = profile.SecondNameAr ?? "",
                        SecondNameEn = profile.SecondNameEn ?? "",
                        ThirdNameAr = profile.ThirdNameAr ?? "",
                        ThirdNameEn = profile.ThirdNameEn ?? "",
                        Designation = workExperience?.Title?.TitleEn ?? "",
                        DesignationAr = workExperience?.Title?.TitleAr ?? "",
                        UserImageFileId = user?.OriginalImageFileId ?? 0,
                        About = ""
                    };
                    myProfile.MyProfileView.Recommendation = userRecommend;
                }
                //var recommendDetails = _mapper.Map<UserRecommendationModelView>(data);

                return myProfile;
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task<IProfileResponse> GetAllUploadedDocumentsAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid : {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null)
                {
                    return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var files = await GetFileDetailsAsync(profile);

                return new ProfileResponse(files);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }


        }

        public async Task<IProfileResponse> UpdateProfileNameAsync(ProfileNameView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId);
                    if (profile == null)
                    {
                        return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                    }
                    profile.FirstNameAr = model.FirstNameAR;
                    profile.SecondNameAr = model.SecondNameAR;
                    profile.ThirdNameAr = model.ThirdNameAR;
                    profile.LastNameAr = model.LastNameAR;

                    profile.FirstNameEn = model.FirstNameEN;
                    profile.SecondNameEn = model.SecondNameEN;
                    profile.ThirdNameEn = model.ThirdNameEN;
                    profile.LastNameEn = model.LastNameEN;

                    await _appDbContext.SaveChangesAsync();

                    var userInfo = await _appDbContext.UserInfos.Include(m => m.User).FirstOrDefaultAsync(k => k.Id == model.UserId);

                    userInfo.User.NameEn = model.FullNameEN;
                    userInfo.User.NameAr = model.FullNameAR;

                    await _appDbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }

                model.ProfileCompletedPercentage =
                    await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.UserId);

                return new ProfileResponse(model);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> GetContactDetailsAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var profile = await _appDbContext.Profiles.Include(k => k.ResidenceCountry).FirstOrDefaultAsync(k => k.Id == userId);

                if (profile == null)
                {
                    return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var userInfo = await _appDbContext.UserInfos.Include(m => m.User).FirstOrDefaultAsync(k => k.Id == userId);
                var lookupEmirate = await _appDbContext.LookupItems.FirstOrDefaultAsync(k => k.Id == profile.EmirateItemId);
                var lookupPassportIssue = await _appDbContext.LookupItems.FirstOrDefaultAsync(k => k.Id == profile.PassportIssueEmirateItemId);

                var data = new ContactDetailsView()
                {
                    UserId = userId,
                    UserLocation = new UserLocationView()
                    {
                        Address = profile.Address,
                        Country = _mapper.Map<CountryView>(profile.ResidenceCountry),
                        Emirate = _mapper.Map<LookupItemView>(lookupEmirate)
                    },
                    UserPassport = new UserPassportView()
                    {
                        EmiratesId = profile.Eid,
                        UnifiedPassportNumber = profile.UnifiedPassportNumber,
                        PassportNumber = profile.PassportNumber,
                        PassportIssue = _mapper.Map<LookupItemView>(lookupPassportIssue)
                    },
                    UserProfessionalDetail = new UserProfessionalDetailsView()
                    {
                        PhoneNumber = userInfo.Mobile,
                        BusinessEmail = profile.BusinessEmail,
                        Email = userInfo.Email,
                        LinkedInUrl = profile.LinkedInUrl,
                        OfficeNumber = profile.LandLine,
                        TwitterUrl = profile.TwitterUrl
                    }
                };
                return new ProfileResponse(data);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }


        }

        public async Task<IProfileResponse> UpdateContactDetailsAsync(ContactDetailsView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var profile = await _appDbContext.Profiles.Include(k => k.ResidenceCountry).FirstOrDefaultAsync(k => k.Id == model.UserId);
                if (profile == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                var userInfo = await _appDbContext.UserInfos.Include(m => m.User).FirstOrDefaultAsync(k => k.Id == model.UserId);

                using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
                {
                    profile.Address = model.UserLocation.Address;
                    profile.ResidenceCountryId = model.UserLocation?.Country?.Id;
                    profile.PassportNumber = model.UserPassport?.PassportNumber;
                    profile.PassportIssueEmirateItemId = model.UserPassport?.PassportIssue?.Id;
                    profile.BusinessEmail = model.UserProfessionalDetail?.BusinessEmail;
                    profile.LinkedInUrl = model.UserProfessionalDetail?.LinkedInUrl;
                    profile.LandLine = model.UserProfessionalDetail?.OfficeNumber;
                    profile.TwitterUrl = model.UserProfessionalDetail?.TwitterUrl;
                    profile.EmirateItemId = model.UserLocation?.Emirate?.Id;
                    profile.UnifiedPassportNumber = model.UserPassport?.UnifiedPassportNumber;
                    userInfo.Mobile = model.UserProfessionalDetail?.PhoneNumber;

                    await _appDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                model.ProfileCompletedPercentage =
                   await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.UserId);

                return new ProfileResponse(model);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> GetLanguagesAndProficiencyAsync()
        {
            try
            {

                var langList = await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.Languages).OrderBy(k => k.NameEn).ToListAsync();

                var proficiencyList = await _appDbContext.LookupItems.Where(y => y.LookupId == (int)LookupType.Proficiency).OrderByDescending(k => k.Id).ToListAsync();

                var data = new LanguageAndProficiencyView()
                {
                    LanguageList = _mapper.Map<List<LookupItemView>>(langList),
                    ProficiencyList = _mapper.Map<List<LookupItemView>>(proficiencyList)
                };

                return new ProfileResponse(data);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> GetCountriesAndEmiratesAsync()
        {
            try
            {
                var countries = await _appDbContext.Countries.OrderBy(k => k.NameEn).ToListAsync();

                var emirateList = await _appDbContext.LookupItems.Where(y => y.LookupId == (int)LookupType.UAEEmirates).OrderBy(k => k.NameEn).ToListAsync();

                var data = new CountriesAndEmiratesView()
                {
                    CountryList = _mapper.Map<List<CountryView>>(countries),
                    EmirateList = _mapper.Map<List<LookupItemView>>(emirateList)
                };

                return new ProfileResponse(data);
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }

        }

        public async Task<IProfileResponse> GetMyProfileAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId : {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var posts = await _mongoDbContext.Posts.Find(x => x.UserID == userId && x.IsAdminCreated != true && x.IsDeleted != true).ToListAsync();
                var data = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if(data != null)
                {
                    data.PostsCount = posts.Count();
                    await _appDbContext.SaveChangesAsync();

                }

                var follow = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
                var followData = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if(follow != null && followData != null)
                {
                    followData.FollowingCount = follow.FollowingIDs.Count();
                    followData.FollowersCount = follow.FollowersIDs.Count();
                    await _appDbContext.SaveChangesAsync();
                }

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == userId);

                var profileEducation = await ProfileEducationAsync(userId);

                var profileWorkExperience = await ProfileWorkExperienceAsync(userId);

                var skillsAndInterest = await ProfileSkillAsync(userId);

                var learningPreference = await LearningPreferenceAsync(userId);

                var profileTraining = await ProfileTrainingAsync(userId);

                var profileAchievement = await ProfileAchievementAsync(userId);

                var profileMembership = await ProfileMembershipAsync(userId);

                var profileAlumni = await GetAlumniAysnc(userId);

                var profileCompetency = await GetProfileCompetencyAysnc(userId);

                var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
    .Where(k => k.ProfileId == userId).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();

                var rec = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == userId && x.isAccepted).FirstOrDefaultAsync();
                var userRecommend = _mapper.Map<UserRecommendationModelView>(rec);
                if (rec != null)
                {
                    var profileDetail = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userRecommend.SenderUserID);
                    var wrkExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == userRecommend.SenderUserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var userDetail = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == userRecommend.SenderUserID);
                    userRecommend.SenderInfo = new PublicProfileView()
                    {
                        Id = profileDetail.Id,
                        FirstNameAr = profileDetail.FirstNameAr ?? "",
                        FirstNameEn = profileDetail.FirstNameEn ?? "",
                        LastNameAr = profileDetail.LastNameAr ?? "",
                        LastNameEn = profileDetail.LastNameEn ?? "",
                        SecondNameAr = profileDetail.SecondNameAr ?? "",
                        SecondNameEn = profileDetail.SecondNameEn ?? "",
                        ThirdNameAr = profileDetail.ThirdNameAr ?? "",
                        ThirdNameEn = profileDetail.ThirdNameEn ?? "",
                        Designation = wrkExperience?.Title?.TitleEn ?? "",
                        DesignationAr = wrkExperience?.Title?.TitleAr ?? "",
                        UserImageFileId = userDetail?.OriginalImageFileId ?? 0,
                        About = ""
                    };
                }
                var myProfileModel = new MyProfileView()
                {
                    Id = profile.Id,
                    FirstNameAR = profile.FirstNameAr,
                    FirstNameEN = profile.FirstNameEn,
                    LastNameAR = profile.LastNameAr,
                    LastNameEN = profile.LastNameEn,
                    SecondNameAR = profile.SecondNameAr,
                    SecondNameEN = profile.SecondNameEn,
                    ThirdNameAR = profile.ThirdNameAr,
                    ThirdNameEN = profile.ThirdNameEn,

                    Designation = workExperience?.Title?.TitleEn ?? "",
                    DesignationAr = workExperience?.Title?.TitleAr ?? "",

                    FollowersCount = profile.FollowersCount,
                    FollowingCount = profile.FollowingCount,
                    PostCount = profile.PostsCount,

                    LPSPoint = profile.Lpspoints,
                    TotalYearsOfExperience = Convert.ToString(Math.Round(profile.TotalYearsOfExperinceReadOnly, 1)),

                    CompletePercentage = profile.CompletenessPercentage,

                    IsInfluencer = profile.IsInfluencer,
                    IsPublicFigure = profile.IsPublicFigure,
                    IsAccepted = await _appDbContext.Applications.AnyAsync(k => k.ProfileId == profile.Id && k.StatusItemId == (int)ApplicationProgressStatus.Accepted),
                    IsAlumni = await _appDbContext.Applications.AnyAsync(k => k.ProfileId == profile.Id && k.StatusItemId == (int)ApplicationProgressStatus.Alumni),

                    Bio = new BioView()
                    {
                        ExpressYourSelf = profile.ExpressYourself,
                        UserId = profile.Id,
                        ExpressYourSelfURL = string.IsNullOrWhiteSpace(profile.ExpressYourselfUrl) ? "" : $"/api/File/get-download-video/{profile.Id}"
                    },
                    ProfileEducation = profileEducation.OrderBy(k => k.Year).ToList(),
                    ProfileWorkExperience = profileWorkExperience.OrderBy(k => k.DateFrom).ToList(),
                    SkillAndInterest = skillsAndInterest,
                    ProfileLearningPreference = learningPreference,
                    ProfileTraining = profileTraining.OrderBy(k => k.Date).ToList(),
                    ProfileAchievement = profileAchievement.OrderBy(k => k.Date).ToList(),
                    ProfileMembership = profileMembership.OrderBy(k => k.Date).ToList(),
                    UserImageFileId = user?.OriginalImageFileId ?? 0,
                    ProfileAlumni = profileAlumni.ToList(),
                    ProfileCompetency = profileCompetency,
                    Recommendation = userRecommend

                };

                return new ProfileResponse(myProfileModel);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ProfileResponse(ex);
            }


        }

        public async Task<IProfileResponse> GetFollowingListAsync(int userId, int take, int page, string search, LanguageType languageType = LanguageType.EN)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(m => m.User)
                    .FirstOrDefaultAsync(k => k.Id == userId);

                if (userInfo == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                var mongoUser = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (mongoUser == null)
                {
                    mongoUser = new User() { Id = userId };
                    await _mongoDbContext.Users.InsertOneAsync(mongoUser);
                }

                var userViewModel = _mapper.Map<MongoModels.User>(mongoUser);

                var users = await _appDbContext.Users.Where(k =>
                    userViewModel.FollowingIDs.Contains(k.Id) && (string.IsNullOrWhiteSpace(search)
                                                                  || (search.IsEnglishString() ? k.NameEn.Contains(search) : k.NameAr.Contains(search))
                                                                  )).OrderBy(k => k.NameEn)
                                            .ToListAsync();

                var userIds = users.Select(k => k.Id).ToList();

                var followingProfiles = await _appDbContext.Profiles
                                                        .Include(k => k.ResidenceCountry)
                                                        .Include(k => k.ProfileWorkExperiences)
                                                        .Where(k => userIds.Contains(k.Id))
                                                        .OrderByDescending(k => k.Id)
                                                        .Skip(take * (page - 1)).Take(take)
                                                        .ToListAsync();


                var followings = await GetProfileViewModelAsync(followingProfiles);

                var peopleMayKnows = await PeopleMayKnowsAsync(userId, userViewModel, search);


                var newFollowings = await GetProfileViewModelAsync(peopleMayKnows);

                return new ProfileResponse(new ProfileFollowingsView()
                {
                    FollowingsList = followings,
                    PeopleMayKnowList = newFollowings,
                    Page = page,
                    Take = take,
                    TotalCount = userIds.Distinct().Count()
                });
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        private async Task<List<Profile>> PeopleMayKnowsAsync(int userId, User userViewModel, string search)
        {
            //var workOrgIds = await _appDbContext.ProfileWorkExperiences.Where(k => k.ProfileId == userId)
            //    .Select(k => k.OrganizationId).ToListAsync();
            //var commonIds = await _appDbContext.ProfileWorkExperiences.Where(k => workOrgIds.Contains(k.OrganizationId))
            //    .Select(k => k.ProfileId).ToListAsync();

            var users = await _appDbContext.Users.Where(k => !userViewModel.FollowingIDs.Contains(k.Id) && k.Id != userId && (string.IsNullOrWhiteSpace(search)
                                                                  || (search.IsEnglishString() ? k.NameEn.Contains(search) : k.NameAr.Contains(search))
                    )).OrderBy(k => k.NameEn)
                .ToListAsync();

            var userIds = users.Select(k => k.Id).ToList();

            var peopleMayKnows = await _appDbContext.Profiles
                .Include(k => k.ResidenceCountry)
                .Include(k => k.ProfileWorkExperiences)
                .Where(k => userIds.Contains(k.Id)).Take(20)
                .ToListAsync();

            return peopleMayKnows;
        }

        public async Task<IProfileResponse> GetFollowersListAsync(int userId, int take, int page, string search, LanguageType languageType = LanguageType.EN)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(m => m.User).FirstOrDefaultAsync(k => k.Id == userId).ConfigureAwait(false);

                if (userInfo == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);

                var mongoUser = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync().ConfigureAwait(false);

                if (mongoUser == null)
                {
                    mongoUser = new MongoModels.User() { Id = userId };
                    await _mongoDbContext.Users.InsertOneAsync(mongoUser).ConfigureAwait(false);
                }

                var userViewModel = _mapper.Map<MongoModels.User>(mongoUser);

                var users = await _appDbContext.Users.Where(k =>
                        userViewModel.FollowersIDs.Contains(k.Id) && (string.IsNullOrWhiteSpace(search)
                                                                      || (languageType == LanguageType.EN ? k.NameEn.Contains(search) : k.NameAr.Contains(search)))).OrderBy(k => k.NameEn)
                    .ToListAsync().ConfigureAwait(false);

                var userIds = users.Select(k => k.Id).ToList();

                var followersList = await _appDbContext.Profiles
                                                        .Include(k => k.ResidenceCountry)
                                                        .Include(k => k.ProfileWorkExperiences)
                                                        .Where(k => userIds.Contains(k.Id))
                                                        .OrderByDescending(k => k.Id)
                                                        .Skip(take * (page - 1)).Take(take)
                                                        .ToListAsync().ConfigureAwait(false);


                var followers = await GetProfileViewModelAsync(followersList);

                foreach (var follower in followers)
                {
                    var user = await _mongoDbContext.Users.Find(x => x.Id == follower.Id).FirstOrDefaultAsync();
                    if (user == null) { continue; }
                    follower.IsAmFollowing = user.FollowersIDs.Any(k => k == userId);
                }

                return new ProfileResponse(new ProfileFollowersView()
                {
                    FollowersList = followers,
                    Page = page,
                    Take = take,
                    TotalCount = followers.Count
                });
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        //public async Task<IProfileResponse> AddUserFollowerAsync(int userId, int followerId)
        //{
        //    try
        //    {
        //        var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

        //        if (user == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
        //        if (user.FollowersIDs == null) user.FollowersIDs = new BsonArray();

        //        user.FollowersIDs.Add(followerId);

        //        var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
        //        if (result.IsAcknowledged)
        //            await UpdateProfileFollowersCountAsync(userId, true);
        //        return new ProfileResponse();
        //    }
        //    catch (Exception e)
        //    {
        //         return new ProfileResponse(ex);
        //    }
        //}

        //public async Task<IProfileResponse> DeleteUserFollowerAsync(int userId, int followerId)
        //{
        //    try
        //    {
        //        var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
        //        if (user == null) return new ProfileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
        //        if ((user.FollowersIDs == null)) return new ProfileResponse();

        //        user.FollowersIDs.Remove(followerId);
        //        var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
        //        if (result.IsAcknowledged)
        //            await UpdateProfileFollowersCountAsync(userId, false);
        //        return new ProfileResponse();
        //    }
        //    catch (Exception e)
        //    {
        //         return new ProfileResponse(ex);
        //    }
        //}

        public async Task<IProfileResponse> AddUserFollowingAsync(int userId, int followingId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid : {userId} followingid : {followingId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (user == null)
                {
                    user = new MongoModels.User() { Id = userId };
                    await _mongoDbContext.Users.InsertOneAsync(user).ConfigureAwait(false);
                }

                user.FollowingIDs.Add(followingId);

                user.FollowingIDs = user.FollowingIDs.Distinct().ToList();

                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user).ConfigureAwait(false);
                if (result.IsAcknowledged)
                {
                    await UpdateFollowCountAsync(userId, user);
                }
                await UpdateFollowersCountAsync(followingId, userId).ConfigureAwait(false);

                //activityLog
                await AddNotificationAsync(userId, ActionType.Follow, followingId.ToString(), ParentType.User, userId);

                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == followingId && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                {
                    await AddNotificationAsync(followingId, ActionType.Follow, followingId.ToString(), ParentType.User, userId);
                }

                return new ProfileResponse();
            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        public async Task AddNotificationAsync(int userId, ActionType actionId, string parentId, ParentType parentTypeId, int senderUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(userId);

                var notificationObj = new Notification
                {
                    ID = ObjectId.GenerateNewId(),
                    ActionID = (int)actionId,
                    IsRead = false,
                    ParentID = parentId,
                    ParentTypeID = (int)parentTypeId,
                    SenderID = senderUserId,
                    IsPushed = true
                };

                notificationGenericObject.Notifications.Add(notificationObj);

                if (userId != senderUserId)
                {
                    notificationGenericObject.UnseenNotificationCounter += 1;
                    var notificationView = _mapper.Map<NotificationView>(notificationObj);
                    await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView });
                    
                    PushNotification(notificationView, userId);
                }



                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);


            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        public async Task<bool> PushNotification(NotificationView notification, int userId)
        {
            var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
            foreach (var deviceId in deviceIds)
            {
                await _pushNotificationService.SendPushNotificationAsync(notification, deviceId);
            }
            return true;
        }

        public async Task<IProfileResponse> GetRecommendedPeople(int userId)
        {
            try
            {
                var mongoUser = await _mongoDbContext.Users.Find(e => e.Id == userId).FirstOrDefaultAsync();
                var user = await _appDbContext.Users.Where(e => e.Id == userId).FirstOrDefaultAsync();

                var profileViews = new List<MyProfileView>();
                var profiles = new List<Profile>();
                if (user != null)
                {
                    profiles = await _appDbContext.Profiles.Distinct().Where(p => p.Id != userId && !mongoUser.FollowingIDs.Contains(p.Id)).Take(5).ToListAsync();
                }
                foreach (var profile in profiles)
                {
                    profileViews.Add(new MyProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAR = profile.FirstNameAr,
                        FirstNameEN = profile.FirstNameEn,
                        LastNameAR = profile.LastNameAr,
                        LastNameEN = profile.LastNameEn,
                        SecondNameAR = profile.SecondNameAr,
                        SecondNameEN = profile.SecondNameEn,
                        ThirdNameAR = profile.ThirdNameAr,
                        ThirdNameEN = profile.ThirdNameEn,

                        FollowersCount = profile.FollowersCount,
                        FollowingCount = profile.FollowingCount,
                        PostCount = profile.PostsCount,
                        UserImageFileId = user?.OriginalImageFileId ?? 0
                    });
                }

                return new ProfileResponse(profileViews);
            }
            catch (Exception e)
            {
                return new ProfileResponse(e);
            }
        }

        private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList)
        {
            foreach (var notification in notificationsList)
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == notification.SenderID);
                notification.UserNameEn = user?.NameEn;
                notification.UserNameAr = user?.NameAr;
                notification.UserImageFileId = user?.OriginalImageFileId ?? 0;
                notification.RedirectUrlPath = notification.ParentTypeID == ParentType.User
                    ? $"/api/Profile/get-public-profile/{userId}/{notification.SenderID}"
                    : notification.RedirectUrlPath;
            }
        }

        private async Task<NotificationGenericObject> AddNotificationObjectAsync(int userId)
        {
            try
            {
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

                if (notificationGenericObject != null) return notificationGenericObject;

                notificationGenericObject = new NotificationGenericObject
                {
                    ID = ObjectId.GenerateNewId(),
                    UserID = userId,
                    UnseenNotificationCounter = 0,
                    Notifications = new List<Notification>()
                };

                await _mongoDbContext.NotificationGenericObjects.InsertOneAsync(notificationGenericObject);
                return notificationGenericObject;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task UpdateFollowersCountAsync(int followingId, int userId)
        {
            var user = await _mongoDbContext.Users.Find(x => x.Id == followingId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (user == null)
            {
                user = new User() { Id = followingId };
                await _mongoDbContext.Users.InsertOneAsync(user).ConfigureAwait(false);
            };

            user.FollowersIDs.Add(userId);
            user.FollowersIDs = user.FollowersIDs.Distinct().ToList();
            var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == followingId, user).ConfigureAwait(false);
            if (result.IsAcknowledged)
            {
                await UpdateFollowCountAsync(followingId, user);
            }

        }

        public async Task<IProfileResponse> DeleteUserFollowingAsync(int userId, int followingId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (user == null)
                {
                    user = new MongoModels.User() { Id = userId };
                    await _mongoDbContext.Users.InsertOneAsync(user).ConfigureAwait(false);
                }

                if (!user.FollowingIDs.Any()) return new ProfileResponse();
                user.FollowingIDs.Remove(followingId);
                user.FollowingIDs = user.FollowingIDs.Distinct().ToList();
                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user).ConfigureAwait(false);
                if (result.IsAcknowledged)
                {
                    await UpdateFollowCountAsync(userId, user);
                }
                await DeleteFollowersCountAsync(followingId, userId).ConfigureAwait(false);
                //removing activity log 
                await DeleteNotificationAsync(userId, ActionType.Follow, followingId.ToString(), ParentType.User,
                    userId);

                await DeleteNotificationAsync(followingId, ActionType.Follow, userId.ToString(), ParentType.User,
                    userId);

                return new ProfileResponse();
            }
            catch (Exception e)
            {
                return new ProfileResponse(message: ClientMessageConstant.WeAreUnableToProcessYourRequest, HttpStatusCode.InternalServerError);
            }
        }


        public async Task DeleteNotificationAsync(int userId, ActionType actionId, string parentId, ParentType parentTypeId, int senderUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userid: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects
                    .Find(x => x.UserID == userId).FirstOrDefaultAsync();

                if (notificationGenericObject == null)
                {
                    return;
                }

                var notification = notificationGenericObject.Notifications.Find(k =>
                    k.SenderID == senderUserId && k.ActionID == (int)actionId &&
                    k.ParentID.Equals(parentId) && k.ParentTypeID == (int)parentTypeId);

                notificationGenericObject.Notifications.Remove(notification);

                notificationGenericObject.UnseenNotificationCounter = (notificationGenericObject.UnseenNotificationCounter - 1).ToUint();

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);


            }
            catch (Exception e)
            {

            }
        }

        private async Task DeleteFollowersCountAsync(int followingId, int userId)
        {
            var mongoUser = await _mongoDbContext.Users.Find(x => x.Id == followingId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (mongoUser == null) return;
            if (!mongoUser.FollowersIDs.Any()) return;

            mongoUser.FollowersIDs.Remove(userId);
            mongoUser.FollowersIDs = mongoUser.FollowersIDs.Distinct().ToList();
            var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == followingId, mongoUser).ConfigureAwait(false);
            if (result.IsAcknowledged)
            {
                await UpdateFollowCountAsync(followingId, mongoUser);
            }
        }

        private async Task UpdateFollowCountAsync(int profileId, User user)
        {
            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId).ConfigureAwait(false);
            profile.FollowersCount = user.FollowersIDs?.Count ?? 0;
            profile.FollowingCount = user.FollowingIDs?.Count ?? 0;

            await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IProfileResponse> AddEndorsementCountAsync(EndorsementView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var endorsement = new ProfileSkillEndorsement()
                {
                    ProfileId = model.PublicProfileId,
                    PublicProfileId = model.ProfileId,
                    ProfileSkillId = model.ProfileSkillId
                };

                var isExist = await _appDbContext.ProfileSkillEndorsements.AnyAsync(k =>
                    k.ProfileId == endorsement.ProfileId && k.ProfileSkillId == endorsement.ProfileSkillId &&
                    k.PublicProfileId == endorsement.PublicProfileId);

                if (!isExist)
                {
                    await _appDbContext.ProfileSkillEndorsements.AddAsync(endorsement);
                    await _appDbContext.SaveChangesAsync();
                }

                return new ProfileResponse();

            }
            catch (Exception ex)
            {
                return new ProfileResponse(ex);
            }
        }

        private async Task<List<FileView>> GetFileDetailsAsync(Profile profile)
        {

            var listOfFiles = new List<FileView>();

            var documentsTypes = Enum.GetValues(typeof(DocumentType));

            foreach (var documentType in documentsTypes)
            {

                switch (documentType)
                {
                    case DocumentType.Passport:
                        if (profile.PassportFileId == null) { break; }
                        var file = _mapper.Map<FileView>(await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == profile.PassportFileId));
                        file.DocumentType = DocumentType.Passport;
                        listOfFiles.Add(file);
                        break;
                    case DocumentType.Education:
                        if (profile.LastEducationCertificateFileId == null) { break; }
                        var education = _mapper.Map<FileView>(await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == profile.LastEducationCertificateFileId));
                        education.DocumentType = DocumentType.Education;
                        listOfFiles.Add(education);
                        break;
                    case DocumentType.CV:
                        if (profile.CvfileId == null) { break; }
                        var cvView = _mapper.Map<FileView>(await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == profile.CvfileId));
                        cvView.DocumentType = DocumentType.CV;
                        listOfFiles.Add(cvView);
                        break;
                    case DocumentType.Emirates:
                        if (profile.UaeidfileId == null) { break; }
                        var uaeView = _mapper.Map<FileView>(await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == profile.UaeidfileId));
                        uaeView.DocumentType = DocumentType.Emirates;
                        listOfFiles.Add(uaeView);
                        break;
                    case DocumentType.FamilyBook:

                        if (profile.FamilyBookFileId == null) { break; }
                        var familyView = _mapper.Map<FileView>(await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == profile.FamilyBookFileId));
                        familyView.DocumentType = DocumentType.FamilyBook;
                        listOfFiles.Add(familyView);
                        break;

                }
            }

            return listOfFiles;
        }

        private async Task<List<PublicProfileView>> GetProfileViewModelAsync(List<Profile> followingProfiles)
        {
            try
            {
                var followers = followingProfiles.Select(k => new PublicProfileView()
                {
                    Id = k.Id,
                    FirstNameAr = k.FirstNameAr,
                    FirstNameEn = k.FirstNameEn,
                    LastNameAr = k.LastNameAr,
                    LastNameEn = k.LastNameEn,
                    SecondNameAr = k.SecondNameAr,
                    SecondNameEn = k.SecondNameEn,
                    ThirdNameAr = k.ThirdNameAr,
                    ThirdNameEn = k.ThirdNameEn,
                    FollowersCount = k.FollowersCount,
                    FollowingCount = k.FollowingCount,
                    PostCount = k.PostsCount,
                    LPSPoint = k.Lpspoints,
                    CompletePercentage = k.CompletenessPercentage
                }).ToList();

                foreach (var data in followers)
                {
                    var experiences = await ProfileWorkExperienceAsync(data.Id);
                    data.Designation = experiences.OrderByDescending(k => k.DateFrom).FirstOrDefault()
                        ?.ExperienceJobTitleView.TitleEn;
                    data.DesignationAr = experiences.OrderByDescending(k => k.DateFrom).FirstOrDefault()
                        ?.ExperienceJobTitleView.TitleAr;
                    data.UserImageFileId = (await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == data.Id))
                                           ?.OriginalImageFileId ?? 0;
                }

                return followers;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        private async Task<List<ProfileMembershipView>> ProfileMembershipAsync(int userId)
        {
            var membership = await _appDbContext.ProfileMemberships.Include(k => k.Organization).Where(k => k.ProfileId == userId).ToListAsync();

            return _mapper.Map<List<ProfileMembershipView>>(membership);
        }

        private async Task<List<ProfileAchievementView>> ProfileAchievementAsync(int userId)
        {
            var achievements = await _appDbContext.ProfileAchievements.Include(k => k.Orgnization)
                                                                    .Include(k => k.AwardItem)
                                                                    .Include(k => k.ImpactItem)
                                                                    .Include(k => k.MedalItem)
                                                                    .Include(k => k.ReachedItem)
                                                                    .Include(k => k.VerbItem)
                                                                    .Where(k => k.ProfileId == userId).ToListAsync();

            return _mapper.Map<List<ProfileAchievementView>>(achievements);
        }

        private async Task<List<ProfileTrainingView>> ProfileTrainingAsync(int userId)
        {
            var profileTraining = await _appDbContext.ProfileTrainings.Include(k => k.Organization).Where(k => k.ProfileId == userId).ToListAsync();

            return _mapper.Map<List<ProfileTrainingView>>(profileTraining);
        }

        private async Task<List<ProfileLearningPreferenceView>> LearningPreferenceAsync(int userId)
        {
            var learning = await _appDbContext.ProfileLearningPreferences.Include(y => y.LearningPreferenceItem).Where(k => k.ProfileId == userId).ToListAsync();

            return _mapper.Map<List<ProfileLearningPreferenceView>>(learning);

        }

        private async Task<SkillAndInterestView> ProfileSkillAsync(int profileId)
        {

            var profileSkillProfiles = await _appDbContext.ProfileSkillProfiles.Where(k => k.ProfileId == profileId).ToListAsync();

            var skills =
                await _appDbContext.ProfileSkills.Where(k => profileSkillProfiles.Select(y => y.Id).Contains(k.Id)).ToListAsync();

            var profileInterest =
                await _appDbContext.ProfileInterests.Where(k => k.ProfileId == profileId).ToListAsync();

            var interest = await _appDbContext.LookupItems.Where(k => profileInterest.Select(y => y.Id).Contains(k.Id)).ToListAsync();

            var profileLanguages = await _appDbContext.ProfileLanguage.Include(k => k.LookupLanguage)
                .Include(m => m.LookupProficiency).Where(k => k.ProfileId == profileId).OrderByDescending(k => k.ProficiencyItemId).ToListAsync();

            var data = new SkillAndInterestView()
            {
                ProfileId = profileId,
                InterestedItems = _mapper.Map<List<LookupItemView>>(interest),
                ProfileSkillItems = _mapper.Map<List<ProfileSkillView>>(skills),
                LanguageKnown = _mapper.Map<List<LanguageItemView>>(profileLanguages),
            };

            foreach (var skill in data.ProfileSkillItems)
            {
                skill.EndorsementCount =
                    await _appDbContext.ProfileSkillEndorsements.CountAsync(k =>
                         k.ProfileId == profileId && k.ProfileSkillId == skill.Id);
            }



            return data;
        }

        private async Task<List<ProfileWorkExperienceView>> ProfileWorkExperienceAsync(int userId)
        {
            var workExperiences = await _appDbContext.ProfileWorkExperiences.Include(k => k.Organization)
                .Include(k => k.Country).Include(k => k.FieldOfwork).Include(k => k.Industry)
                .Include(k => k.LineManagerTitle).Include(k => k.Title).Where(k => k.ProfileId == userId).ToListAsync();

            var data = _mapper.Map<List<ProfileWorkExperienceView>>(workExperiences);

            return data;
        }

        private async Task<List<ProfileEducationView>> ProfileEducationAsync(int userId)
        {
            var profileEducations = await _appDbContext.ProfileEducations.Include(k => k.Organization).Include(k => k.Country).Include(k => k.DegreeItem).Include(k => k.FieldOfStudyNavigation)
                .Where(k => k.ProfileId == userId).ToListAsync();

            var data = profileEducations.Select(k => new ProfileEducationView()
            {
                ProfileId = k.ProfileId,
                OrganizationName = k.Organization.NameEn,
                Organization = _mapper.Map<OrganizationView>(k.Organization),
                Country = _mapper.Map<CountryView>(k.Country),
                DegreeItem = _mapper.Map<LookupItemView>(k.DegreeItem),
                FieldOfStudy = _mapper.Map<ProfileEducationFieldOfStudyView>(k.FieldOfStudyNavigation),
                IsStudied = k.Finshed,
                Title = k.Title,
                Id = k.Id,
                OrganizationId = k.OrganizationId,
                CountryId = k.CountryId,
                Year = k.Year,
                FieldOfStudyString = k.FieldOfStudy,
                DegreeLookupItemId = k.DegreeItemId,
                FieldOfStudyId = k.FieldOfStudyId
            }).ToList();

            return data;
        }

        public async Task<List<ProfileAlumniList>> GetAlumniAysnc(int userId)
        {
            try
            {
                var data = from app in _appDbContext.Applications
                           join batch in _appDbContext.Batches on app.BatchId equals batch.Id
                           join pgrm in _appDbContext.Programmes on batch.ProgrammeId equals pgrm.Id
                           join lookup in _appDbContext.LookupItems on pgrm.ProgrammeTypeItemId equals lookup.Id
                           where app.ProfileId == userId && (app.StatusItemId == 59006 || app.StatusItemId == 59009)
                           select new
                           {
                               BatchNumber = batch.Number,
                               BatchYear = batch.Year,
                               ProgramTitleEn = pgrm.ShortTitleEn,
                               ProgramTitleAr = pgrm.ShortTitleAr,
                               ProgramCategoryID = lookup.Id,
                               ProgramCategoryEn = lookup.NameEn,
                               ProgramCategoryAr = lookup.NameAr,
                               StatusEn = app.StatusItemId == 59009 ? "graduate" : "participant",
                               StatusAr = app.StatusItemId == 59009 ? "تخرج" : "مشارك"

                           };

                var alumniDetails = await data.ToListAsync();

                var alumniList = new List<ProfileAlumniList>();

                foreach (var item in alumniDetails)
                {
                    ProfileAlumniList alumni = new ProfileAlumniList()
                    {
                        BatchNumber = item.BatchNumber,
                        BatchYear = item.BatchYear,
                        ProgramTitleEn = item.ProgramTitleEn,
                        ProgramTitleAr = item.ProgramTitleAr,
                        ProgramCategoryID = item.ProgramCategoryID,
                        ProgramCategoryEn = item.ProgramCategoryEn,
                        ProgramCategoryAr = item.ProgramCategoryAr,
                        StatusEn = item.StatusEn,
                        StatusAr = item.StatusAr
                    };

                    alumniList.Add(alumni);
                }


                return alumniList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IProfileResponse> GetTrackApplicationAsync(int userId, int applicationTypeId, int skip, int limit)
        {
            try
            {
                var participateData = await _appDbContext.ParticipationReferences.Where(x => x.ProfileID == userId).ToListAsync();

                var programList = participateData.Where(x => x.ApplicationID.HasValue).ToList();
                var challengeList = participateData.Where(x => x.InitiativeParticipationID.HasValue).ToList();

                var applications = new List<TrackApplication>();
                if (programList.Count > 0 && applicationTypeId == (int)TrackApplicationType.Program)
                {
                    var programData = from participate in _appDbContext.ParticipationReferences
                                      join app in _appDbContext.Applications on participate.ApplicationID equals app.Id
                                      join batch in _appDbContext.Batches on app.BatchId equals batch.Id
                                      join pgrm in _appDbContext.Programmes on batch.ProgrammeId equals pgrm.Id
                                      join lookup in _appDbContext.LookupItems on app.StatusItemId equals lookup.Id
                                      where participate.ProfileID == userId
                                      select new
                                      {

                                          TitleEn = pgrm.TitleEn,
                                          TitleAr = pgrm.TitleAr,
                                          LookUpId = lookup.Id,
                                          //StatusAr = lookup.NameAr,
                                          ReferenceNumber = participate.ReferenceNumber,
                                          BatchNumber = batch.Number,
                                          BatchID = batch.Id,
                                          EndDate = batch.DateRegTo,
                                          ApplicationTypeID = (int)TrackApplicationType.Program,
                                          Created = pgrm.Created,
                                          ApplicationTypeEn = "Programmes",
                                          ApplicationTypeAr = "البرامج"

                                      };

                    var programDetails = await programData.ToListAsync();
                    foreach (var item in programDetails)
                    {
                        var lookupDetails = await _appDbContext.LookupItems.Where(k => k.Id == item.LookUpId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
                        var applicationStatus = _mapper.Map<LookupItemView>(lookupDetails);
                        TrackApplication program = new TrackApplication()
                        {
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            ApplicationStatus = applicationStatus,
                            //StatusEn = item.StatusEn,
                            //StatusAr = item.StatusAr,
                            ReferenceNumber = item.ReferenceNumber,
                            EndDate = item.EndDate,
                            BatchNumber = item.BatchNumber,
                            BatchID = item.BatchID,
                            ApplicationTypeID = item.ApplicationTypeID,
                            Created = item.Created
                        };

                        applications.Add(program);
                    }
                }

                if (challengeList.Count > 0 && applicationTypeId != (int)TrackApplicationType.Program)
                {
                    var challengeData = from participate in _appDbContext.ParticipationReferences
                                        join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                        join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                        join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                        where initiativeProfile.ProfileId == userId
                                        select new
                                        {
                                            TitleEn = initiative.TitleEn,
                                            TitleAr = initiative.TitleAr,
                                            LookUpId = lookup.Id,
                                            //StatusAr = lookup.NameAr,
                                            ReferenceNumber = participate.ReferenceNumber,
                                            EndDate = initiative.RegistrationEndDate,
                                            ActivityID = initiative.Id,
                                            CategoryID = initiative.CategoryId,
                                            InitiativeTypeItemID = initiative.InitiativeTypeItemId,
                                            //ApplicationTypeID = (int)TrackApplicationType.Challenge,
                                            Created = initiative.Created,
                                            ApplicationTypeEn = "Challenges",
                                            ApplicationTypeAr = "التحديات"
                                        };

                    var challengeDetails = await challengeData.ToListAsync();
                    foreach (var item in challengeDetails)
                    {
                        
                        var lookupDetails = await _appDbContext.LookupItems.Where(k => k.Id == item.LookUpId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
                        var applicationStatus = _mapper.Map<LookupItemView>(lookupDetails);
                        TrackApplication challenge = new TrackApplication()
                        {
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            ApplicationStatus = applicationStatus,
                            //StatusEn = item.StatusEn,
                            //StatusAr = item.StatusAr,
                            ReferenceNumber = item.ReferenceNumber,
                            EndDate = item.EndDate,
                            ActivityID = item.ActivityID,
                            CategoryID = item.CategoryID,
                            InitiativeTypeItemID = item.InitiativeTypeItemID,
                            ApplicationTypeID = item.InitiativeTypeItemID == 72001 ? (int)TrackApplicationType.ExperienceExchange : 
                                                item.InitiativeTypeItemID == 72002 ? (int)TrackApplicationType.Challenge :
                                                item.InitiativeTypeItemID == 72003 ? (int)TrackApplicationType.EngagementActivities : (int)TrackApplicationType.Project,
                            Created = item.Created
                        };
                        if (applicationTypeId == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (applicationTypeId == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (applicationTypeId == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (applicationTypeId == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }

                    }
                }

                return new ProfileResponse(applications.OrderByDescending(x => x.Created).Skip(skip).Take(limit).ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IProfileResponse> GetAllTrackApplicationAsync(int userId, string text = "")
        {
            try
            {
                var participateData = await _appDbContext.ParticipationReferences.Where(x => x.ProfileID == userId).ToListAsync();

                var programList = participateData.Where(x => x.ApplicationID.HasValue).ToList();
                var challengeList = participateData.Where(x => x.InitiativeParticipationID.HasValue).ToList();

                var applicationsList = new List<AllTrackApplicationView>();
                if (programList.Count >= 0)
                {
                    var programData = from participate in _appDbContext.ParticipationReferences
                                      join app in _appDbContext.Applications on participate.ApplicationID equals app.Id
                                      join batch in _appDbContext.Batches on app.BatchId equals batch.Id
                                      join pgrm in _appDbContext.Programmes on batch.ProgrammeId equals pgrm.Id
                                      join lookup in _appDbContext.LookupItems on app.StatusItemId equals lookup.Id
                                      where participate.ProfileID == userId
                                      select new { };

                    var programDetails = await programData.ToListAsync();

                    AllTrackApplicationView applicationDataList = new AllTrackApplicationView()
                    {
                        ApplicationTypeID = (int)TrackApplicationType.Program,
                        Count = programDetails.Count()
                    };

                    applicationsList.Add(applicationDataList);
                }

                if (challengeList.Count >= 0)
                {
                     var challenge = from participate in _appDbContext.ParticipationReferences
                                        join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                        join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                        join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                        where initiativeProfile.ProfileId == userId && initiative.InitiativeTypeItemId == 72002
                                        select new { };

                      var challengeDetails = await challenge.ToListAsync();

                    if (challengeDetails.Count >= 0)
                    {
                        AllTrackApplicationView challengeDataList = new AllTrackApplicationView()
                        {
                            ApplicationTypeID = (int)TrackApplicationType.Challenge,
                            Count = challengeDetails.Count()
                        };

                        applicationsList.Add(challengeDataList);
                    }

                    var experience = from participate in _appDbContext.ParticipationReferences
                                     join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                     join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                     join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                     where initiativeProfile.ProfileId == userId && initiative.InitiativeTypeItemId == 72001
                                     select new { };

                    var experienceDetails = await experience.ToListAsync();
                    if (experienceDetails.Count >= 0)
                    {
                        AllTrackApplicationView experienceDataList = new AllTrackApplicationView()
                        {
                            ApplicationTypeID = (int)TrackApplicationType.ExperienceExchange,
                            Count = experienceDetails.Count()
                        };

                        applicationsList.Add(experienceDataList);
                    }

                    var engagementData = from participate in _appDbContext.ParticipationReferences
                                        join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                        join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                        join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                        where initiativeProfile.ProfileId == userId && initiative.InitiativeTypeItemId == 72003
                                        select new { };

                    var engagementDetails = await engagementData.ToListAsync();
                    if (engagementDetails.Count >= 0)
                    {
                        AllTrackApplicationView engagementDataList = new AllTrackApplicationView()
                        {
                            ApplicationTypeID = (int)TrackApplicationType.EngagementActivities,
                            Count = engagementDetails.Count()
                        };

                        applicationsList.Add(engagementDataList);
                    }

                    var projectData = from participate in _appDbContext.ParticipationReferences
                                        join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                        join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                        join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                        where initiativeProfile.ProfileId == userId && initiative.InitiativeTypeItemId == 72004
                                        select new { };

                    var projectDataDetails = await projectData.ToListAsync();

                    if (projectDataDetails.Count >= 0)
                    {
                        AllTrackApplicationView projectDataDataList = new AllTrackApplicationView()
                        {
                            ApplicationTypeID = (int)TrackApplicationType.Project,
                            Count = projectDataDetails.Count()
                        };

                        applicationsList.Add(projectDataDataList);
                    }
                }

                return new ProfileResponse(applicationsList);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
        public async Task<IProfileResponse> SearchTrackApplicationAsync(int userId, int applicationTypeId, string text)
        {
            try
            {
                var participateData = await _appDbContext.ParticipationReferences.Where(x => x.ProfileID == userId).ToListAsync();

                var programList = participateData.Where(x => x.ApplicationID.HasValue).ToList();
                var challengeList = participateData.Where(x => x.InitiativeParticipationID.HasValue).ToList();

                text = text.ToLower().Trim();

                var applications = new List<TrackApplication>();
                if (programList.Count > 0 && applicationTypeId == (int)TrackApplicationType.Program)
                {
                    var programData = from participate in _appDbContext.ParticipationReferences
                                      join app in _appDbContext.Applications on participate.ApplicationID equals app.Id
                                      join batch in _appDbContext.Batches on app.BatchId equals batch.Id
                                      join pgrm in _appDbContext.Programmes on batch.ProgrammeId equals pgrm.Id
                                      join lookup in _appDbContext.LookupItems on app.StatusItemId equals lookup.Id
                                      where participate.ProfileID == userId
                                      select new
                                      {

                                          TitleEn = pgrm.TitleEn,
                                          TitleAr = pgrm.TitleAr,
                                          LookUpId = lookup.Id,
                                          //StatusAr = lookup.NameAr,
                                          ReferenceNumber = participate.ReferenceNumber,
                                          BatchNumber = batch.Number,
                                          BatchID = batch.Id,
                                          EndDate = batch.DateRegTo,
                                          ApplicationTypeID = (int)TrackApplicationType.Program,
                                          Created = pgrm.Created,
                                          ApplicationTypeEn = "Programmes",
                                          ApplicationTypeAr = "البرامج"

                                      };

                      var programDetails = await programData.Where(x => x.TitleEn.ToLower().Contains(text) || x.TitleAr.ToLower().Contains(text) || x.ApplicationTypeEn.ToLower().Contains(text) || x.ApplicationTypeAr.ToLower().Contains(text)).ToListAsync();

                    foreach (var item in programDetails)
                    {
                        var lookupDetails = await _appDbContext.LookupItems.Where(k => k.Id == item.LookUpId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
                        var applicationStatus = _mapper.Map<LookupItemView>(lookupDetails);
                        TrackApplication program = new TrackApplication()
                        {
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            ApplicationStatus = applicationStatus,
                            //StatusEn = item.StatusEn,
                            //StatusAr = item.StatusAr,
                            ReferenceNumber = item.ReferenceNumber,
                            EndDate = item.EndDate,
                            BatchNumber = item.BatchNumber,
                            BatchID = item.BatchID,
                            ApplicationTypeID = item.ApplicationTypeID,
                            Created = item.Created
                        };

                        applications.Add(program);
                    }
                }

                if (challengeList.Count > 0 && applicationTypeId != (int)TrackApplicationType.Program)
                {
                    var challengeData = from participate in _appDbContext.ParticipationReferences
                                        join initiativeProfile in _appDbContext.InitiativeProfiles on participate.InitiativeParticipationID equals initiativeProfile.Id
                                        join initiative in _appDbContext.Initiatives on initiativeProfile.InitiativeId equals initiative.Id
                                        join lookup in _appDbContext.LookupItems on initiativeProfile.StatusItemId equals lookup.Id
                                        where initiativeProfile.ProfileId == userId
                                        select new
                                        {
                                            TitleEn = initiative.TitleEn,
                                            TitleAr = initiative.TitleAr,
                                            LookUpId = lookup.Id,
                                            //StatusAr = lookup.NameAr,
                                            ReferenceNumber = participate.ReferenceNumber,
                                            EndDate = initiative.RegistrationEndDate,
                                            ActivityID = initiative.Id,
                                            CategoryID = initiative.CategoryId,
                                            InitiativeTypeItemID = initiative.InitiativeTypeItemId,
                                            ApplicationTypeID = (int)TrackApplicationType.Challenge,
                                            Created = initiative.Created,
                                            ApplicationTypeEn = "Challenges",
                                            ApplicationTypeAr = "التحديات"
                                        };

                        var challengeDetails = await challengeData.Where(x => x.TitleEn.ToLower().Contains(text) || x.TitleAr.ToLower().Contains(text) || x.ApplicationTypeEn.ToLower().Contains(text) || x.ApplicationTypeAr.ToLower().Contains(text)).ToListAsync();

                    foreach (var item in challengeDetails)
                    {
                        var lookupDetails = await _appDbContext.LookupItems.Where(k => k.Id == item.LookUpId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
                        var applicationStatus = _mapper.Map<LookupItemView>(lookupDetails);
                        TrackApplication challenge = new TrackApplication()
                        {
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            ApplicationStatus = applicationStatus,
                            //StatusEn = item.StatusEn,
                            //StatusAr = item.StatusAr,
                            ReferenceNumber = item.ReferenceNumber,
                            EndDate = item.EndDate,
                            ActivityID = item.ActivityID,
                            CategoryID = item.CategoryID,
                            InitiativeTypeItemID = item.InitiativeTypeItemID,
                            ApplicationTypeID = item.InitiativeTypeItemID == 72001 ? (int)TrackApplicationType.ExperienceExchange :
                                                item.InitiativeTypeItemID == 72002 ? (int)TrackApplicationType.Challenge :
                                                item.InitiativeTypeItemID == 72003 ? (int)TrackApplicationType.EngagementActivities : (int)TrackApplicationType.Project,
                            Created = item.Created
                        };
                        if (applicationTypeId == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (item.InitiativeTypeItemID == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (item.InitiativeTypeItemID == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                        else if (item.InitiativeTypeItemID == challenge.ApplicationTypeID)
                        {
                            applications.Add(challenge);
                        }
                    }
                }
                
                return new ProfileResponse(new SearchTrackApplicationView()
                {
                    TrackApplication = applications.OrderByDescending(x => x.Created).ToList()
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public async Task<IProfileResponse> UserCustomNotificationAsync(UserCustomNotificationView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var notificationData = await _appDbContext.CustomNotifications.FirstOrDefaultAsync(x => x.ProfileID == view.ProfileID && x.CategoryID == view.CategoryID);

                if (notificationData != null)
                {
                    notificationData.isEnabled = view.isEnabled;
                    await _appDbContext.SaveChangesAsync();

                }
                else
                {
                    notificationData = new CustomNotification()
                    {
                        ProfileID = view.ProfileID,
                        CategoryID = view.CategoryID,
                        isEnabled = view.isEnabled
                    };

                    await _appDbContext.CustomNotifications.AddAsync(notificationData);
                    await _appDbContext.SaveChangesAsync();
                }

                var data = _mapper.Map<UserCustomNotificationView>(notificationData);

                return new ProfileResponse(data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }


        public async Task<IProfileResponse> GetCustomNotificationAsync(int userId)
        {
            try
            {
                var notificationList = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == userId).ToListAsync();
                var data = _mapper.Map<List<UserCustomNotificationView>>(notificationList);
                return new ProfileResponse(data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public async Task<ProfileCompetencyView> GetProfileCompetencyAysnc(int userId)
        {
            try
            {
                var latestOrder = await _appDbContext.ProfileCompetencyScores.Where(x => x.ProfileId == userId && x.Assessmenttool.AssessmentToolCategory == 82001).OrderByDescending(x => x.Order).Select(x => x.Order).FirstOrDefaultAsync();
                var profileCompetency = await _appDbContext.ProfileCompetencyScores.Where(x => x.ProfileId == userId && x.Assessmenttool.AssessmentToolCategory == 82001 && x.Order == latestOrder).OrderByDescending(x => x.Stenscore).FirstOrDefaultAsync();

                if (profileCompetency != null)
                {
                    var competency = await _appDbContext.Competencies.Where(x => x.Id == profileCompetency.CompetencyId).FirstOrDefaultAsync();

                    var data = new ProfileCompetencyView()
                   {
                        CompetencyID = competency.Id,
                        CompetencyNameEn = competency?.NameEn,
                        CompetencyNameAr = competency?.NameAr
                   };

                    return data;

                } else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IProfileResponse> SystemNotificationAsync(SystemNotificationView view)
        {
            var data = new SystemNotificationView()
            {
                Message = view.Message
            };
            //var righttoleft = " "+((Char)0x200F)+" ";
            //data.Message = string.Format("{0,10}", data.Message.PadRight(25));
            //data.Message = data.Message.Replace(" ", righttoleft);

            var deviceIds = await _appDbContext.UserDeviceInfos.Select(k => k.DeviceId).ToListAsync();
            PushNotification(data, deviceIds);

            //if (!String.IsNullOrEmpty(data.MessageAr))
            //{
            //    bool isArabic = true;
            //    PushNotification(data, deviceIds, isArabic);

            //}
            //if(!String.IsNullOrEmpty(data.MessageEn))
            //{
            //    bool isArabic = false;
            //    PushNotification(data, deviceIds, isArabic);
            //}


            return new ProfileResponse();
        }

        public async Task<bool> PushNotification(SystemNotificationView message, List<string> deviceIds)
        {
            
            foreach (var deviceId in deviceIds)
            {
                await _pushNotificationService.SendSystemPushNotificationAsync(message, deviceId);
            }
            return true;
        }

        public async Task<string> CVParsingAsync(IFormFile Resume)
        {

            var appSetting = new CVParsingConfig()
            {
                APIUrl = _cvParsingConfig.APIUrl,
                UserKey = _cvParsingConfig.UserKey,
                Version = _cvParsingConfig.Version,
                SubUserID = _cvParsingConfig.SubUserID
            };

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  APIUrl : {appSetting.APIUrl} UserKey : {appSetting.UserKey} Version : {appSetting.Version} SubUserID : {appSetting.SubUserID}");

            try
            {
                //Set APIURL as rest api url for resume parsing provided by RChilli.

                String APIURL = appSetting.APIUrl;
                //Set USERKEY as provided by RChilli.
                String USERKEY = appSetting.UserKey;
                String VERSION = appSetting.Version;

                //String filePath = @"D:\Angular\Sathish _Kumar_Resume.pdf";
                //FileInfo file = new FileInfo(filePath);
                byte[] DataFile = converttoBase64(Resume);
                String subUserId = appSetting.SubUserID;
                String request = "{\"filedata\":\"" + Convert.ToBase64String(DataFile)
                                    + "\",\"filename\":\"" + Resume.FileName
                                    + "\",\"userkey\":\"" + USERKEY
                                    + "\",\"version\":\"" + VERSION
                                    + "\",\"subuserid\":\"" + subUserId
                                    + "\"}";
                byte[] byteArray = Encoding.UTF8.GetBytes(request);
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(APIURL);
                httpRequest.Credentials = CredentialCache.DefaultCredentials;
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.ContentLength = byteArray.Length;
                httpRequest.Timeout = 300000;
                Stream dataStream = httpRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                String response = streamReader.ReadToEnd();
                //Console.WriteLine(response);
                //Console.ReadKey();

                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public static byte[] converttoBase64(IFormFile fno)
        {
            try
            {
                Int64 numofbyte = fno.Length;
                //FileStream fs = new FileStream(fno.FileName, FileMode.Open);
                //BinaryReader br = new BinaryReader(fs);
                //byte[] DataFile = br.ReadBytes(Convert.ToInt32(numofbyte));
                byte[] DataFile = StreamToBytes(fno.OpenReadStream());

                //fs.Close();
                //fs.Dispose();
                return DataFile;
            }
            catch (Exception ex)
            {
            }
            byte[] error1 = new byte[1];
            error1[0] = (byte)' ';
            return error1;
        }

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

}
