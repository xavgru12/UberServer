using Microsoft.AspNetCore.Http;
using MongoDB.Driver.Core.Servers;
using System;
using System.Buffers.Text;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Uberstrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Middleware
{
    public class JoinRoom : IMiddleware
    {

        public JoinRoom()
        {

        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine(context.Request.Path);
            if(context.Request.Path == "/join")
            {
                var id = AES.DecodeAndDecrypt(context.Request.Query["roomId"].ToString().Split(".")[0]);
                var server = AES.DecodeAndDecrypt(context.Request.Query["roomId"].ToString().Split(".")[1]);
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(File.ReadAllText("ProtocolUI\\Join.html").Replace("{{id}}", Base64Encode(Encoding.GetEncoding("ISO-8859-1").GetBytes(server +"|"+ id))));
                return;
            }
            await next(context);
        }

        string Base64Encode(byte[] plainText)
        {
            return Convert.ToBase64String(plainText);
        }
    }
}
