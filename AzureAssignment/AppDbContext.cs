using System;
using System.Collections.Generic;
using System.Text;
using AzureAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Services;

namespace AzureAssignment
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var keyVaultService = new KeyVaultService();
            optionsBuilder.UseSqlServer(keyVaultService.GetSecretValue("SqlConnectionString").GetAwaiter().GetResult(), builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
