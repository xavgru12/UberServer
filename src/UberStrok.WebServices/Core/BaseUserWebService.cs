using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Core
{
    public abstract class BaseUserWebService : BaseWebService, IUserWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseUserWebService).Name);

        protected BaseUserWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract bool OnIsDuplicateMemberName(string username);
        public abstract MemberOperationResult OnSetLoaduout(string authToken, LoadoutView loadoutView);
        public abstract UberstrikeUserView OnGetMember(string authToken);
        public abstract LoadoutView OnGetLoadout(string authToken);
        public abstract LoadoutView OnGetLoadoutServer(string serviceAuth, string authToken);
        public abstract List<ItemInventoryView> OnGetInventory(string authToken);

        byte[] IUserWebServiceContract.ChangeMemberName(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle ChangeMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GenerateNonDuplicatedMemberNames(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GenerateNonDuplicateMemberNames request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetCurrencyDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetCurrentDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetInventory(byte[] data){
				using (var bytes = new MemoryStream(data)) {
					var cmid = Int32Proxy.Deserialize(bytes);

					using (var outputStream = new MemoryStream()) {
						ListProxy<ItemInventoryView>.Serialize(outputStream, new List<ItemInventoryView>(), ItemInventoryViewProxy.Serialize);

						return outputStream.ToArray();
					}
				}
		}


        byte[] IUserWebServiceContract.GetItemTransactions(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetItemTransactions request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetLoadout(byte[] data){
            try
            {
				using (var bytes = new MemoryStream(data)) {
					var cmid = Int32Proxy.Deserialize(bytes);

					using (var outputStream = new MemoryStream()) {
						LoadoutViewProxy.Serialize(outputStream, new LoadoutView());

						return outputStream.ToArray();
					}
				}
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLoadout request:");
                Log.Error(ex);
                return null;
            }
		}

        byte[] IUserWebServiceContract.SetScore(byte[] data) {
			try {
				using (var bytes = new MemoryStream(data)) {

					using (var outputStream = new MemoryStream()) {
						throw new NotImplementedException();

						//return outputStream.ToArray();
					}
				}
			} 
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetScore request:");
                Log.Error(ex);
                return null;
            }

		}

        byte[] IUserWebServiceContract.GetXPEventsView(byte[] data) {
			try {
				using (var bytes = new MemoryStream(data)) {

					using (var outputStream = new MemoryStream()) {
						throw new NotImplementedException();

						//return outputStream.ToArray();
					}
				}
			} 
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetXPEventsView request:");
                Log.Error(ex);
                return null;
            }
		}

        byte[] IUserWebServiceContract.GetLevelCapsView(byte[] data) {
			try {
				using (var bytes = new MemoryStream(data)) {


					using (var outputStream = new MemoryStream()) {
						ListProxy<PlayerLevelCapView>.Serialize(outputStream, new List<PlayerLevelCapView>(), PlayerLevelCapViewProxy.Serialize);

						return outputStream.ToArray();
					}
				}
			} 
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMember request:");
                Log.Error(ex);
                return null;
            }
		}

		byte[] IUserWebServiceContract.GetMember(byte[] data){
            try{
		
				using (var bytes = new MemoryStream(data)) {
					var cmid = Int32Proxy.Deserialize(bytes);

					using (var outputStream = new MemoryStream()) {
						UberstrikeUserViewProxy.Serialize(outputStream, new UberstrikeUserView {
							CmuneMemberView = new MemberView {
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
								},
							},
							UberstrikeMemberView = new UberstrikeMemberView {
								PlayerCardView = new PlayerCardView {
									Cmid = 1
								},
								PlayerStatisticsView = new PlayerStatisticsView {
									Cmid = 1
								}
							}
						});


						return outputStream.ToArray();
					}
				}
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMember request:");
                Log.Error(ex);
                return null;
            }
			
		}


        byte[] IUserWebServiceContract.GetMemberWallet(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberWallet request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetPointsDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetPointsDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.IsDuplicateMemberName(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var username = StringProxy.Deserialize(bytes);

                    var result = OnIsDuplicateMemberName(username);
                    using (var outBytes = new MemoryStream())
                    {
                        BooleanProxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle IsDuplicateMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.SetLoadout(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var authToken = StringProxy.Deserialize(bytes);
                    var loadoutView = LoadoutViewProxy.Deserialize(bytes);

                    var result = OnSetLoaduout(authToken, loadoutView);
                    using (var outBytes = new MemoryStream())
                    {
                        EnumProxy<MemberOperationResult>.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
