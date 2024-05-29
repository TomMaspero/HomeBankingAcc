﻿using Microsoft.EntityFrameworkCore;

namespace HomeBankingAcc.Models
{
    public class HomeBankingContext : DbContext
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
