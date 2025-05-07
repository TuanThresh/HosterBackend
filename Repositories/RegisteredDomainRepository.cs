using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class RegisteredDomainRepository(DataContext context,IMapper mapper) : Repository<RegisteredDomain>(context,mapper),IRegisteredDomainRepository
{
    
}