using log4net;
using System;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public abstract class BaseAuthenticationWebService : BaseWebService, IAuthenticationWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseAuthenticationWebService).Name);

        protected BaseAuthenticationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract AccountCompletionResultView OnCompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId);

        public abstract MemberAuthenticationResultView OnLoginMemberEmail(string steamId, string authToken, string machineId);
        public abstract MemberAuthenticationResultView OnLoginSteam(string steamId, string authToken, string machineId);

        byte[] IAuthenticationWebServiceContract.CompleteAccount(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var cmid = Int32Proxy.Deserialize(bytes);
                    var name = StringProxy.Deserialize(bytes);
                    var channelType = EnumProxy<ChannelType>.Deserialize(bytes);
                    var locale = StringProxy.Deserialize(bytes);
                    var machineId = StringProxy.Deserialize(bytes);

                    var view = OnCompleteAccount(cmid, name, channelType, locale, machineId);
                    using (var outBytes = new MemoryStream())
                    {
                        AccountCompletionResultViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CompleteAccount request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.CreateUser(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CreateUser request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LinkSteamMember(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LinkSteamMember request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberEmail(byte[] data)
        {
            using (var bytes = new MemoryStream(data)) {
					var emailAddress = StringProxy.Deserialize(bytes);
					var password = StringProxy.Deserialize(bytes);
					var channel = EnumProxy<ChannelType>.Deserialize(bytes);
					var machineId = StringProxy.Deserialize(bytes);
                    Console.WriteLine("byte deserialized: ");
                    Console.WriteLine(emailAddress + " " + password + " " + channel + " " + machineId);

					using (var outputStream = new MemoryStream()) {
						MemberAuthenticationResultViewProxy.Serialize(outputStream, new MemberAuthenticationResultView { 
							IsAccountComplete = true,
							IsTutorialComplete = true,
							MemberAuthenticationResult = MemberAuthenticationResult.Ok,
							MemberView = new MemberView {
								PublicProfile = new PublicProfileView {
									Cmid = 1,
									Name = "xavgru",
									AccessLevel = MemberAccessLevel.Admin,
									EmailAddressStatus = EmailAddressStatus.Verified
								},
								MemberWallet = new MemberWalletView {
									Cmid = 1,
									Credits = 1337,
									CreditsExpiration = DateTime.Now,
									Points = 1337,
									PointsExpiration = DateTime.Now
								},
                                MemberItems = new List<int> {
								}
							},
							PlayerStatisticsView = new PlayerStatisticsView { 
								Cmid = 1,
								Xp = 1000
							},
							ServerTime = DateTime.UtcNow,
                            WeeklySpecial = new WeeklySpecialView {
                                StartDate = DateTime.MinValue,
                                EndDate = DateTime.MaxValue,
                                Id = 0,
                                ImageUrl = "http://127.0.0.1:8080/WeeklySpecial/TheWarehouse.jpg",
                                Text = "UberStrike 4.3.10 Beta",
                                Title = "Team UberStrike",
                                ItemId = 1003
                            }   
						});

					return outputStream.ToArray();
                    }
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberFacebookUnitySdk(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberFacebookUnitySdk request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberPortal(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberPortal request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginSteam(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var steamId = StringProxy.Deserialize(bytes);
                    var authToken = StringProxy.Deserialize(bytes);
                    var machineId = StringProxy.Deserialize(bytes);

                    var view = OnLoginSteam(steamId, authToken, machineId);
                    using (var outBytes = new MemoryStream())
                    {
                        MemberAuthenticationResultViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginSteam request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
