﻿using SubscriptionsWebApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SubscriptionsWebApi
{
  public class ApplicationDbContext : IdentityDbContext<User>
  {
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<BookAuthor>()
          .HasKey(ba => new { ba.BookId, ba.AuthorId });
      modelBuilder.Entity<Invoice>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<BookAuthor> BookAuthor { get; set; }

    public DbSet<APIKey> APIKeys { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<DomainRestriction> DomainRestrictions { get; set; }
    public DbSet<IPRestriction> IPRestrictions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<IssuedInvoice> IssuedInvoices { get; set; }

  }
}
