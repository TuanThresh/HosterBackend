using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using HosterBackend.Data;
using HosterBackend.Services;
using HosterBackend.Interfaces;

public class DomainExpiryNotifierService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // mỗi 24h

    public DomainExpiryNotifierService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var registeredDomainRepository = scope.ServiceProvider.GetRequiredService<IRegisteredDomainRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                var today = DateTime.UtcNow.Date;

                var soonToExpireDomains = await dbContext.RegisteredDomains
                    .Include(x => x.DomainAccount)
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Customer)
                    .Where(d =>
                        d.ExpiredAt.Date == today.AddDays(7) ||
                        d.ExpiredAt.Date == today.AddDays(1) ||
                        d.ExpiredAt.Date == today
                )
                    .ToListAsync();

                foreach (var domain in soonToExpireDomains)
                {
                    var email = domain.Order.Customer.Email;
                    var domainName = domain.FullDomainName;
                    var expiredAt = domain.ExpiredAt;

                    await emailService.SendExpiredDomainEmailAsync(email,$"Tên miền {domainName} sắp hết hạn",domain);
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}