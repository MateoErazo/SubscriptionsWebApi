using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Entities;

namespace SubscriptionsWebApi.Controllers.V1
{
  [ApiController]
  [Route("api/v1/iprestrictions")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class IPRestrictionsController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;

    public IPRestrictionsController(ApplicationDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult> Create(IPRestrictionCreationDTO IPRestrictionCreationDTO)
    {
      APIKey apiKey = await dbContext.APIKeys
        .FirstOrDefaultAsync(x => x.Id == IPRestrictionCreationDTO.APIKeyId);

      if (apiKey == null)
      {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != apiKey.UserId)
      {
        return Forbid();
      }

      IPRestriction IPRestriction = new IPRestriction()
      {
        APIKeyId = IPRestrictionCreationDTO.APIKeyId,
        IP = IPRestrictionCreationDTO.IP,
      };

      dbContext.Add(IPRestriction);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute] int id,
      IPRestrictionUpdatingDTO IPRestrictionUpdatingDTO)
    {
      IPRestriction IPRestriction = await dbContext.IPRestrictions
        .Include(x => x.APIKey)
        .FirstOrDefaultAsync(x => x.Id == id);

      if (IPRestriction == null)
      {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != IPRestriction.APIKey.UserId)
      {
        return Forbid();
      }

      IPRestriction.IP = IPRestrictionUpdatingDTO.IP;
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
      IPRestriction IPRestriction = await dbContext.IPRestrictions
        .Include(x => x.APIKey)
        .FirstOrDefaultAsync(x => x.Id == id);

      if (IPRestriction == null)
      {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != IPRestriction.APIKey.UserId)
      {
        return Forbid();
      }

      dbContext.Remove(IPRestriction);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
