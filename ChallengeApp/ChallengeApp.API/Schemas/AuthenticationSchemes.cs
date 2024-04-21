using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChallengeApp.API.Schemas;

public static class AuthenticationSchemes
{
    public const string JwtBearer = JwtBearerDefaults.AuthenticationScheme;
    public const string AzureAd = "AzureAd";
    public const string JwtAndAzureAd = "JwtAndAzureAd";

}