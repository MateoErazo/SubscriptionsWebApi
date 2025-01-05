using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionsWebApi.DTOs;
using SubscriptionsWebApi.Entities;

namespace SubscriptionsWebApi.Controllers.V1
{
  [ApiController]
  [Route("api/v1/invoices")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class InvoicesController: CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;

    public InvoicesController(ApplicationDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult> Pay(PayInvoiceDTO payInvoiceDTO)
    {
      Invoice invoice = await dbContext.Invoices
        .Include(x => x.User)
        .FirstOrDefaultAsync(x => x.Id == payInvoiceDTO.InvoiceId);

      if (invoice == null) {
        return NotFound();
      }

      if (invoice.Paid)
      {
        return BadRequest("This invoice already was paid.");
      }

      invoice.Paid = true;
      await dbContext.SaveChangesAsync();

      bool thereAreOverdueInvoices = await dbContext.Invoices
        .AnyAsync(x => x.UserId == invoice.UserId &&
                       !x.Paid &&
                       x.DueDate < DateTime.Today);

      if (!thereAreOverdueInvoices)
      {
        invoice.User.NonPayingUser = false;
        await dbContext.SaveChangesAsync();
      }

      return NoContent();
    }
  }
}
