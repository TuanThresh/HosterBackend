using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class NewRepository(DataContext context, IMapper mapper) : Repository<New>(context, mapper), INewRepository
{
}