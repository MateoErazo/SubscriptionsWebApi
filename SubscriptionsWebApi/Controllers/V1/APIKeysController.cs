using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Entities;
using SubscriptionsWebApi.Enums;
using SubscriptionsWebApi.Services;

namespace SubscriptionsWebApi.Controllers.V1
{
  [ApiController]
  [Route("api/v1/apikeys")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class APIKeysController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly KeysService keysService;

    public APIKeysController(ApplicationDbContext dbContext, 
      IMapper mapper,
      KeysService keysService)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.keysService = keysService;
    }

    [HttpGet]
    public async Task<List<APIKeyDTO>> GetAll()
    {
      string userId = GetUserId();
      List<APIKey> keys = await dbContext.APIKeys.Where(x => x.UserId == userId).ToListAsync();
      return mapper.Map<List<APIKeyDTO>>(keys);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateAPIKeyDTO createAPIKeyDTO)
    {
      string userId = GetUserId();
      if (createAPIKeyDTO.KeyType == KeyType.Free)
      {
        bool userAlreadyHaveFreeKey = await dbContext.APIKeys
          .AnyAsync(x => x.UserId == userId && x.KeyType == KeyType.Free);

        if (userAlreadyHaveFreeKey) {
          return BadRequest("The user already has a free key and only one free key is allowed per user.");
        }
      }

      await keysService.CreateKeyAsync(userId, KeyType.Professional);
      return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> Update(UpdateAPIKeyDTO updateAPIKeyDTO)
    {
      string userId = GetUserId();
      APIKey apiKey = await dbContext.APIKeys.FirstOrDefaultAsync(x => x.Id == updateAPIKeyDTO.KeyId);

      if (apiKey == null) {
        return NotFound("Don't was found the key.");
      }

      if (userId != apiKey.UserId)
      {
        return Forbid();
      }

      if (updateAPIKeyDTO.UpdateKey)
      {
        apiKey.Key = keysService.GenerateKey();
      }

      apiKey.IsActive = updateAPIKeyDTO.IsActive;

      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
