using System;
using Microsoft.AspNetCore.Http;

namespace SehirRehberi.API.Helpers
{
    public static class JwrExtension
    {
       public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Expose-Header","Application-Error");
        }
    }
}
