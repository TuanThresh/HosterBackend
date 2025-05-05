using AutoMapper;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Interfaces;

namespace HosterBackend.Repositories;

public class PaymentMethodRepository(DataContext context,IMapper mapper) : Repository<PaymentMethod>(context,mapper),IPaymentMethodRepository
{
    
}