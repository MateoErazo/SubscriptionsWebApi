using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SubscriptionsWebApi.Controllers.V1
{
  public class CustomBaseController:ControllerBase
  {
    protected string GetUserId()
    {
      Claim userClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
      return userClaim.Value;
    }
  }
}
