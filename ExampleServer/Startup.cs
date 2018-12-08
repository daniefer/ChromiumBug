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
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
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

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
		{
			var MyCustomCookie = "Cookie";

			app.Use(async (context, next) =>
			{
				var requestHeader = StringifyHeader(context.Request.Headers);
				logger.LogInformation(requestHeader);
				await next();
				var responseHeader = StringifyHeader(context.Response.Headers);
				logger.LogInformation(responseHeader);

				var bytes = Encoding.UTF8.GetBytes($"{requestHeader}\n-----------------------------\n{responseHeader}");
				context.Response.Body.Write(bytes);
			});

			app.Use((context, next) =>
			{
				if (context.Request.Path == "/")
				{
					var options = new CookieOptions
					{
						Expires = DateTimeOffset.Now.AddDays(1),
						HttpOnly = true,
						Secure = false,
						IsEssential = true,
						SameSite = SameSiteMode.Strict
					};
					context.Response.Cookies.Append(MyCustomCookie, "blah blah blah", options);
					context.Response.Headers.Add("Location", new StringValues("http://localhost:5000/found"));
					context.Response.StatusCode = (int)HttpStatusCode.Found;
					context.Response.ContentType = "text/plain; charset=utf-8";
					return Task.CompletedTask;
				}
				context.Response.Cookies.Delete(MyCustomCookie);
				context.Response.StatusCode = (int)HttpStatusCode.OK;

				return next();
			});
		}
	}
}
