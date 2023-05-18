using System;

namespace UberStrok.WebServices.Client.Tests
{
    public static class Program
    {
        //TODO: Write some stress test code or some stuff.

        public static void Main(string[] args)
        {
            var userServiceClient = new UserWebServiceClient("http://uberstroke.mya-hkvtuber.com/");
            var authenticationServiceClient = new AuthenticationWebServiceClient("http://uberstroke.mya-hkvtuber.com/");
            for(int x = 0; x < 10; x++)
            {
                var loginResult = authenticationServiceClient.LoginSteam("test", "", "");
                var member = userServiceClient.GetMember(loginResult.AuthToken);
                var inventory = userServiceClient.GetLoadout(loginResult.AuthToken);
                Console.WriteLine(member.CmuneMemberView);
                Console.WriteLine(member.UberstrikeMemberView);
                Console.WriteLine(inventory);
            }
            Console.ReadLine();
        }
    }
}
