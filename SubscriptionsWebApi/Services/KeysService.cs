using SubscriptionsWebApi.Entities;
using SubscriptionsWebApi.Enums;

namespace SubscriptionsWebApi.Services
{
  public class KeysService
  {
    private readonly ApplicationDbContext dbContext;

    public KeysService(ApplicationDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task CreateKey(string userId, KeyType keyType)
    {
      string key = Guid.NewGuid().ToString().Replace("-","");

      APIKey apiKey = new APIKey()
      {
        Key = key,
        KeyType = keyType,
        IsActive = true,
        UserId = userId,
      };

      dbContext.Add(apiKey);
      await dbContext.SaveChangesAsync();
    }
  }
}
