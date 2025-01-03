using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Entities;

namespace SubscriptionsWebApi.Controllers.V1
{
  [ApiController]
  [Route("api/v1/domainrestrictions")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class DomainRestrictionsController : CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;

    public DomainRestrictionsController(ApplicationDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult> Create(DomainRestrictionCreationDTO domainRestrictionCreationDTO)
    {
      APIKey apiKey = await dbContext.APIKeys
        .FirstOrDefaultAsync(x => x.Id == domainRestrictionCreationDTO.APIKeyId);

      if (apiKey == null) {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != apiKey.UserId)
      {
        return Forbid();
      }

      DomainRestriction domainRestriction = new DomainRestriction()
      {
        APIKeyId = domainRestrictionCreationDTO.APIKeyId,
        Domain = domainRestrictionCreationDTO.Domain,
      };

      dbContext.Add(domainRestriction);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute] int id, 
      DomainRestrictionUpdatingDTO domainRestrictionUpdatingDTO)
    {
      DomainRestriction domainRestriction = await dbContext.DomainRestrictions
        .Include(x => x.APIKey)
        .FirstOrDefaultAsync(x => x.Id == id);

      if (domainRestriction == null) {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != domainRestriction.APIKey.UserId)
      {
        return Forbid();
      }

      domainRestriction.Domain = domainRestrictionUpdatingDTO.Domain;
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
      DomainRestriction domainRestriction = await dbContext.DomainRestrictions
        .Include(x => x.APIKey)
        .FirstOrDefaultAsync(x => x.Id == id);

      if (domainRestriction == null)
      {
        return NotFound();
      }

      string userId = GetUserId();

      if (userId != domainRestriction.APIKey.UserId)
      {
        return Forbid();
      }

      dbContext.Remove(domainRestriction);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
