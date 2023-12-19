using Google.Apis.Auth.AspNetCore3;
using KSB.Results;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    .AddTransient<DocumentEditor>()
    .AddTransient<AppJsonSerializerContext>()
    .AddTransient<StartsFromSpreadSheetLoader>();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", async (LicenseRequestsGenerator worker) => await worker.Run()).RequireAuthorization();


app.Run();


[JsonSerializable(typeof(PlayerStart))]
[JsonSerializable(typeof(PlayerStartsResult[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

}
