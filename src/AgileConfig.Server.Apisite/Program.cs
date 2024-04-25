using System;
using AgileConfig.Server.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using NLog.Web;

namespace AgileConfig.Server.Apisite
{
    public class Program
    {
        //public static IRemoteServerNodeProxy RemoteServerNodeProxy { get; private set; }

        public static void Main(string[] args)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("current dir path: " + basePath);
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath);
#if DEBUG
            Global.Config =
                builder
                    .AddJsonFile("appsettings.Development.json")
                    .AddEnvironmentVariables()
                    .Build();
#else
            Global.Config = builder.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
#endif
            var host = CreateWebHostBuilder(args)
                .Build();

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Global.Config)
                .UseNLog()
                .ConfigureKestrel(x =>
                {
                    x.ListenAnyIP(80);
                    x.ListenAnyIP(443, x =>
                    {
                        x.UseHttps(httpsOptions => { httpsOptions.UseLettuceEncrypt(x.ApplicationServices); });
                        x.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                    });
                })
                .UseStartup<Startup>();
        }
    }
}