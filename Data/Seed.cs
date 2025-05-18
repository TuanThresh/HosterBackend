using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HosterBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Data;

public class Seed
{
    public static async Task SeedEmployees(DataContext context)
    {
        if(await context.Employees.AnyAsync()) return;

        var employeeData = await File.ReadAllTextAsync("Data/EmployeeSeedData.json");

        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };

        var employees = JsonSerializer.Deserialize<IEnumerable<Employee>>(employeeData,options) ?? throw new Exception("Không tìm thấy dữ liệu");

        using var hmac = new HMACSHA512();

        foreach (var employee in employees)
        {

            employee.HasRoles.Add(new Authorize {
                RoleId = 1
            });
            employee.Status = Enums.EmployeeStatusEnum.ChoXacThuc;
            employee.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            employee.PasswordSalt = hmac.Key;
            context.Employees.Add(employee);
        }
        var admin = new Employee
        {
            Name = "tuan",
            Email = "tuan92381@st.vimaru.edu.vn",
            Address = "232 Trại Lẻ",
            PhoneNumber = "0373750960",
            Status = Enums.EmployeeStatusEnum.KichHoat,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
            PasswordSalt = hmac.Key
        };

        context.Employees.Add(admin);

        admin.HasRoles.Add(new Authorize
        {
            RoleId = 2
        });

        await context.SaveChangesAsync();
    }
    
    public static async Task SeedData<T>(DataContext context,string dataPath) where T : class
    {
        var dbSet = context.Set<T>();

        if(await dbSet.AnyAsync()) return;

        var entityData = await File.ReadAllTextAsync(dataPath);

        var option = new JsonSerializerOptions{
            PropertyNameCaseInsensitive = true
        };

        var entities = JsonSerializer.Deserialize<IEnumerable<T>>(entityData,option) ?? throw new Exception("Không tìm thấy dữ liệu");

        dbSet.AddRange(entities);

        await context.SaveChangesAsync();
    }
}