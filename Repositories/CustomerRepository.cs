using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class CustomerRepository(DataContext context, IMapper mapper) : Repository<Customer>(context,mapper),ICustomerRepository
{
    
}