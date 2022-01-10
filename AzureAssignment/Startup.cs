using AzureAssignment;
using AzureAssignment.Repository.Implementation;
using AzureAssignment.Repository.Interfaces;
using AzureAssignment.Services;
using AzureAssignment.Services.Implementation;
using AzureAssignment.Services.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureAssignment
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var keyVaultService = new KeyVaultService();
            string connectionString = keyVaultService.GetSecretValue("SqlConnectionString").GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Please specify a valid SQL DB Connection in the Key vault.");
            }
            builder.Services.AddDbContext<AppDbContext>(
              options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<IEventPublisherOnPurchaseOrderService, EventPublisherOnPurchaseOrderService>();
            builder.Services.AddScoped<IEventTriggerService, EventTriggerService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

    }
}
