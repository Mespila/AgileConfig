using AgileConfig.Server.Apisite;
using AgileConfig.Server.Apisite.UIExtension;
using AgileConfig.Server.Apisite.Websocket;
using AgileConfig.Server.Common;
using AgileConfig.Server.Common.RestClient;
using AgileConfig.Server.Data.Abstraction;
using AgileConfig.Server.Data.Freesql;
using AgileConfig.Server.Data.Repository.Selector;
using AgileConfig.Server.OIDC;
using AgileConfig.Server.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddEnvironmentVariables();
builder.Host.UseSystemd();
builder.Logging.ClearProviders();
builder.Logging.AddNLog();
builder.WebHost.ConfigureKestrel(x =>
{
    var appServices = x.ApplicationServices;
    x.ListenAnyIP(80);
    x.ListenAnyIP(443, x =>
    {
        x.UseHttps(httpsOptions => { httpsOptions.UseLettuceEncrypt(appServices); });
        x.Protocols = HttpProtocols.Http1AndHttp2;
    });
});


builder.Services.AddDefaultHttpClient(true);
builder.Services.AddRestClient();

builder.Services.AddMemoryCache();
builder.Services.AddSystemd();
builder.Host.UseSystemd();
builder.Services.AddCors();
builder.Services.AddMvc().AddRazorRuntimeCompilation();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
    var xmlPath = Path.Combine(basePath, "AgileConfig.Server.Apisite.xml");
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddEnvAccessor();
builder.Services.AddDbConfigInfoFactory();
builder.Services.AddFreeSqlFactory();
// Add freesqlRepositories or other repositories
builder.Services.AddRepositories();
builder.Services.AddBusinessServices();

builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddHostedService<InitService>();
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

builder.Services.AddOIDC();
builder.Services.AddLettuceEncrypt();

var app = builder.Build();

var basePath = app.Configuration.GetValue<string>("pathBase");
if (!string.IsNullOrWhiteSpace(basePath))
{
    app.UsePathBase(basePath);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "My API V1"); });

app.UseMiddleware<ReactUIMiddleware>();
app.UseCors(op =>
{
    op.AllowAnyOrigin();
    op.AllowAnyMethod();
    op.AllowAnyHeader();
});
app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(60),
});

app.UseMiddleware<WebsocketHandlerMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();


await app.RunAsync();