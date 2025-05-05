using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HosterBackend.Data.Entities;
using HosterBackend.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HosterBackend.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateEmployeeToken(Employee employee)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Không tìm thấy khóa token trong file config");

        if(tokenKey.Length < 64) throw new Exception("Khóa token cần phải dài hơn");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier,employee.Id.ToString()),
            new(ClaimTypes.Name,employee.Name)
        };

        if(employee.HasRoles.Count > 0)
        {
            foreach(var authorize in employee.HasRoles)
            {
                claims.Add(new (ClaimTypes.Role,authorize.Role.RoleName));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    public string CreateCustomerToken(Customer customer)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Không tìm thấy khóa token trong file config");

        if(tokenKey.Length < 64) throw new Exception("Khóa token cần phải dài hơn");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier,customer.Id.ToString()),
            new(ClaimTypes.Name,customer.Name),
            new(ClaimTypes.Role,"Khách hàng")
        };

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}