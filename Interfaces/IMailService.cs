using HosterBackend.Data.Entities;

namespace HosterBackend.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(string toEmail, string subject, Order order);
}