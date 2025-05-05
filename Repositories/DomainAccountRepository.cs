using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class DomainAccountRepository(DataContext context,IMapper mapper) : Repository<DomainAccount>(context,mapper),IDomainAccountRepository
{
    

}