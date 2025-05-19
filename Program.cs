using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using WebAppNGS.Data;



var builder = WebApplication.CreateBuilder(args);

// Auth Azure AD
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = "roles";

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;

            if (identity == null)
                return;

            var groupClaims = identity.FindAll("groups").Select(c => c.Value).ToList();

            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var groupMap = config.GetSection("GroupIds").Get<Dictionary<string, string>>();

            if (groupClaims == null || groupMap == null)
                return;

            foreach (var groupId in groupMap)
            {
                if (groupClaims.Contains(groupId.Value))
                {
                    identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, groupId.Key));
                }
            }

            await Task.CompletedTask;
        }

    };
});



builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    });

builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;
    options.AddPolicy("AdminGroup", policy =>
        policy.RequireClaim("groups", "GUID_DU_GROUPE_ADMIN"));
});

// Journalisation
builder.Services.AddApplicationInsightsTelemetry();

// Key Vault
string keyVaultUrl = "https://ks-ngs-webapp.vault.azure.net/";
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
KeyVaultSecret secret = client.GetSecret("DbConnectionString");
string connectionString = secret.Value;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Auth globale
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
         .RequireAuthenticatedUser()
         .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();


app.Run();
