using Google.Apis.Auth.AspNetCore3;
using KSB.Results.Db;
using KSB.Results.LicenseRequest;
using KSB.Results.LiveResults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddAuthorization();
builder.Services
       .AddAuthentication(o =>
       {
           // This forces challenge results to be handled by Google OpenID Handler, so there's no
           // need to add an AccountController that emits challenges for Login.
           o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
           // This forces forbid results to be handled by Google OpenID Handler, which checks if
           // extra scopes are required and does automatic incremental auth.
           o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
           // Default scheme that will handle everything else.
           // Once a user is authenticated, the OAuth2 token info is stored in cookies.
           o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       })
       .AddCookie()
       .AddGoogleOpenIdConnect(options =>
       {

           options.ClientId = builder.Configuration.GetValue<string>("GoogleClientId");
           options.ClientSecret = builder.Configuration.GetValue<string>("GoogleClientSecret");
           options.Scope.Add("https://www.googleapis.com/auth/drive.metadata.readonly");
           options.Scope.Add("https://www.googleapis.com/auth/spreadsheets");

       });

builder.Services.AddTransient<LicenseRequestsGenerator>()
    .AddTransient<LicenseRequestDocumentCreator>()
    .AddTransient<AppJsonSerializerContext>()
    .AddTransient<StartsFromSpreadSheetLoader>()
    .AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));
builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder.WithOrigins(
                    app.Configuration.GetValue<string>("CorsAllowedOrigins")!.Split(";")
                    )
                .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                .WithMethods("PUT", "DELETE", "GET", "POST", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials());

app.MapGet("/generateLicenseRequests", async (LicenseRequestsGenerator worker) => await worker.Run()).RequireAuthorization();
app.MapGet("/", () => "WOOOOOO!");
app.MapHub<LiveResultsHub>("/LiveResultsHub");

app.Run();


[JsonSerializable(typeof(PlayerStart))]
[JsonSerializable(typeof(PlayerStartsResult[]))]
[JsonSerializable(typeof(PlayerRunResult[]))]
[JsonSerializable(typeof(SingleResult))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

}
