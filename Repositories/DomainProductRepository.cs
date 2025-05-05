using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class DomainProductRepository(DataContext context,IMapper mapper) : Repository<DomainProduct>(context,mapper),IDomainProductRepository
{
    
}