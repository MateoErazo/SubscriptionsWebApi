﻿using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SubscriptionsWebApi.Enums;
using SubscriptionsWebApi.Entities;

namespace SubscriptionsWebApi.Controllers.V1
{
  [ApiController]
  [Route("api/v1/accounts")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class AccountsController : ControllerBase
  {
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly IConfiguration configuration;
    private readonly KeysService keysService;

    public AccountsController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        KeysService keysService
        )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.configuration = configuration;
      this.keysService = keysService;
    }

    /// <summary>
    /// Create a new user account.
    /// </summary>
    /// <param name="userCredentials">Credentials for the new account.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("create", Name = "createAccountV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> Create(UserCredentialsDTO userCredentials)
    {
      User user = new User()
      {
        UserName = userCredentials.Email,
        Email = userCredentials.Email
      };

      var creationResult = await userManager.CreateAsync(user, userCredentials.Password);

      if (creationResult.Succeeded)
      {
        await keysService.CreateKeyAsync(user.Id, KeyType.Free);
        return await BuildToken(userCredentials, user.Id);
      }
      else
      {
        return BadRequest(creationResult.Errors);
      }
    }

    /// <summary>
    /// Log in with your user account.
    /// </summary>
    /// <param name="userCredentialsDTO">Your user account credentials.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login", Name = "createLoginV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> Login(UserCredentialsDTO userCredentialsDTO)
    {
      var result = await signInManager.PasswordSignInAsync(
          userName: userCredentialsDTO.Email, password: userCredentialsDTO.Password,
          isPersistent: false, lockoutOnFailure: false
      );

      if (result.Succeeded)
      {
        User user = await userManager.FindByEmailAsync(userCredentialsDTO.Email);
        return await BuildToken(userCredentialsDTO, user.Id);
      }

      return BadRequest("Incorrect login.");
    }

    [AllowAnonymous]
    [HttpGet("refresh-token", Name = "getRefreshTokenV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> RefreshToken()
    {
      Claim emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

      if (emailClaim == null)
      {
        return NotFound("Please log in and try again.");
      }

      string email = emailClaim.Value;
      Claim claimId = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
      var userId = claimId.Value;

      return await BuildToken(new UserCredentialsDTO {Email = email}, userId);

    }

    private async Task<AccountCreationResponseDTO> BuildToken(UserCredentialsDTO userCredentials,
      string userId)
    {
      List<Claim> claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email),
                new Claim("id",userId)
            };

      User user = await userManager.FindByEmailAsync(userCredentials.Email);
      var claimsDb = await userManager.GetClaimsAsync(user);
      claims.AddRange(claimsDb);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      DateTime expiration = DateTime.UtcNow.AddMinutes(30);

      var securityToken = new JwtSecurityToken(
          issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds
      );

      return new AccountCreationResponseDTO()
      {
        Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
        Expiration = expiration,
      };

    }

    [HttpPost("set-admin", Name = "createAdminV1")]
    public async Task<ActionResult> SetAdmin(UserAdminEditDTO userAdminEditDTO)
    {
      User user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

      if (user == null)
      {
        return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
      }

      await userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
      return NoContent();
    }

    [HttpPost("remove-admin", Name = "deleteAdminV1")]
    public async Task<ActionResult> DeleteAdmin(UserAdminEditDTO userAdminEditDTO)
    {
      User user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

      if (user == null)
      {
        return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
      }

      await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
      return NoContent();
    }

  }
}
