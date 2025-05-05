using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class OrderRepository(DataContext context, IMapper mapper) : Repository<Order>(context, mapper), IOrderRepository
{
}