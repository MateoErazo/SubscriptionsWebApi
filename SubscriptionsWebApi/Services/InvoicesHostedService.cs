
using Microsoft.EntityFrameworkCore;

namespace SubscriptionsWebApi.Services
{
  public class InvoicesHostedService : IHostedService
  {
    private readonly IServiceProvider serviceProvider;
    private Timer timer;

    public InvoicesHostedService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
      timer = new Timer(ProcessInvoices,null,TimeSpan.Zero,TimeSpan.FromDays(1));
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      timer.Dispose();
      return Task.CompletedTask;
    }

    private void ProcessInvoices(object state)
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        SetNonPayingUsers(dbContext);
        IssueInvoices(dbContext);
      }
    }

    private static void SetNonPayingUsers(ApplicationDbContext dbContext)
    {
      dbContext.Database.ExecuteSqlInterpolated($"exec SetNonPayingUser");
    }

    private static void IssueInvoices(ApplicationDbContext dbContext)
    {
      DateTime today = DateTime.Today;
      DateTime comparisonDate = today.AddMonths(-1);
      bool invoicesForTheMonthHaveBeenIssued = dbContext.IssuedInvoices
        .Any(x => x.Year == comparisonDate.Year && x.Month == comparisonDate.Month);

      if (!invoicesForTheMonthHaveBeenIssued)
      {
        DateTime startDate = new DateTime(comparisonDate.Year, comparisonDate.Month, 1);
        DateTime endDate = startDate.AddMonths(1);
        dbContext.Database.ExecuteSqlInterpolated($"exec InvoicesCreation {startDate.ToString("yyyy-MM-dd")}, {endDate.ToString("yyyy-MM-dd")}");
      }
    }
  }
}
