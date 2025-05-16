using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;

namespace HosterBackend.Interfaces;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    
}