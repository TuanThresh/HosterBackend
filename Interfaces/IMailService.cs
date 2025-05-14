using HosterBackend.Data.Entities;

namespace HosterBackend.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(string toEmail, string subject, Order order);
    public Task SendCreatedEmployee( string subject, Employee employee,string password);
    public Task SendExpiredDomainEmailAsync(string toEmail, string subject, RegisteredDomain registeredDomain);
    public Task SendDiscountCodeEmailAsync(string toEmail, string subject, Discount discount);
}