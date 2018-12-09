using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace ExampleServer
{
	public class Startup
	{
		public Startup()
		{
		}
        
		private static string StringifyHeader(IHeaderDictionary header)
		{
			var sb = new StringBuilder();
			foreach (var entry in header)
			{
				sb.AppendLine($"{entry.Key}: {entry.Value}");
			}

			return sb.ToString();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
		{
			app.Use(async (context, next) =>
			{
				var requestHeader = StringifyHeader(context.Request.Headers);
				logger.LogError(requestHeader);
				await next();
				var responseHeader = StringifyHeader(context.Response.Headers);
				logger.LogError(responseHeader);

				var bytes = Encoding.UTF8.GetBytes($"{requestHeader}\n-----------------------------\n{responseHeader}");
				context.Response.Body.Write(bytes);
			});

			app.Use((context, next) =>
			{
                var CookieOneOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Path = "/",
                    //Secure = true, // Toggle this by uncommenting to make the cookie secure and not secure
                    SameSite = SameSiteMode.Lax
                };

                var CustomCookieOne = "CookieOne";
                context.Response.Cookies.Append(CustomCookieOne, $"foo bar {DateTime.Now.Ticks}", CookieOneOptions);
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				context.Response.ContentType = "text/plain; charset=utf-8";
				return Task.CompletedTask;
			});
		}
	}
}
