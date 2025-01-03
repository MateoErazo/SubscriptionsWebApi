using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Entities;
using System.Data;
using static System.Net.WebRequestMethods;

namespace SubscriptionsWebApi.Middlewares
{
  public static class LimitRequestsMiddlewareExtensions
  {
    public static IApplicationBuilder UseLimitRequests(this IApplicationBuilder app)
    {
      return app.UseMiddleware<LimitRequestsMiddleware>();
    }
  }

  public class LimitRequestsMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IConfiguration configuration;

    public LimitRequestsMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      this.next = next;
      this.configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext dbContext)
    {
      var limitRequestsConfig = new LimitRequestsConfigurationDTO();
      configuration.GetRequiredSection("LimitRequests").Bind(limitRequestsConfig);

      var path = httpContext.Request.Path.ToString();
      bool pathIsInWhitelist = limitRequestsConfig.WhitelistRoutes.Any(x => path.Contains(x));
      if (pathIsInWhitelist) {
        await next(httpContext);
        return;
      }

      var keyStringValues = httpContext.Request.Headers["X-Api-Key"];
      if (keyStringValues.Count == 0)
      {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("You must provide the key in the header X-Api-Key");
        return;
      }

      if (keyStringValues.Count > 1)
      {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("Only one key must be present.");
        return;
      }

      string apiKey = keyStringValues[0];

      APIKey apiKeyDb = await dbContext.APIKeys
        .Include(x => x.DomainRestrictions)
        .Include(x => x.IPRestrictions)
        .FirstOrDefaultAsync(x => x.Key == apiKey);
      
      if (apiKeyDb == null) {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("The key don't exist.");
        return;
      }

      if (!apiKeyDb.IsActive)
      {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("The key is inactive.");
        return;
      }

      if (apiKeyDb.KeyType == Enums.KeyType.Free)
      {
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);

        int requestsTodayAmount = await dbContext.Requests
          .CountAsync(x => x.APIKeyId == apiKeyDb.Id &&
          x.RequestDate >= today &&
          x.RequestDate < tomorrow);

        if (requestsTodayAmount >= limitRequestsConfig.FreeRequestsPerDay)
        {
          httpContext.Response.StatusCode = 429;
          await httpContext.Response.WriteAsync("You have exceeded the limit of requests per day. If you want to make more requests today, update you subscription to professional account.");
          return;
        }
      }

      bool approveRestrictions = RequestApprovesAnyOfTheRestrictions(apiKeyDb,httpContext);
      if (!approveRestrictions)
      {
        httpContext.Response.StatusCode = 403;
        return;
      }


      var request = new Request() { APIKeyId = apiKeyDb.Id, RequestDate = DateTime.UtcNow };
      dbContext.Add(request);
      await dbContext.SaveChangesAsync();
      await next(httpContext);
    }

    private bool RequestApprovesAnyOfTheRestrictions(APIKey apiKey, HttpContext httpContext)
    {
      bool existAnyRestriction = apiKey.DomainRestrictions.Any() || apiKey.IPRestrictions.Any();

      if (!existAnyRestriction) {
        return true;
      }

      return RequestApprovesDomainRestrictions(apiKey.DomainRestrictions, httpContext);
    }

    private bool RequestApprovesDomainRestrictions(List<DomainRestriction> domainRestrictions,
      HttpContext httpContext)
    {
      if (domainRestrictions == null || domainRestrictions.Count == 0)
      {
        return false;
      }

      string referer = httpContext.Request.Headers["Referer"].ToString();

      if (referer == string.Empty) {
        return false;
      }

      Uri myUri = new Uri(referer);
      string host = myUri.Host;

      return domainRestrictions.Any(x => x.Domain == host);
    }
  }
}