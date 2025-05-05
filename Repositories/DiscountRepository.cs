using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class DiscountRepository(DataContext context,IMapper mapper) : Repository<Discount>(context,mapper),IDiscountRepository
{
    
}