using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class CustomerTypeRepository(DataContext context,IMapper mapper) : Repository<CustomerType>(context,mapper),ICustomerTypeRepository
{
    
}