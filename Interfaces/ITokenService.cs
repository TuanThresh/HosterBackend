using HosterBackend.Data.Entities;

namespace HosterBackend.Interfaces;

public interface ITokenService
{
    public string CreateEmployeeToken(Employee employee);
    public string CreateCustomerToken(Customer customer);


}