using AutoMapper;
using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;

namespace HosterBackend.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles(){
        CreateMap<RegisterDto,Employee>();
        CreateMap<Employee,EmployeeDto>()
            .ForMember(d => d.HasRoles, o => o.MapFrom(s => s.HasRoles.Select(x => x.Role.RoleName)))
            .ForMember(d => d.Status, o => o.MapFrom(s => ((EmployeeStatusEnum)s.Status).ToString()));
        CreateMap<ChangeEmployeeDto,Employee>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
        CreateMap<Role,RoleDto>();
        CreateMap<ChangeRoleDto,Role>();
        CreateMap<Customer,CustomerDto>()
            .ForMember(d => d.HasType, o => o.MapFrom(s => s.HasType.TypeName));
        CreateMap<ChangeCustomerDto,Customer>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
        CreateMap<CustomerType,CustomerTypeDto>()
            .ForMember(d => d.HasCustomers,o => o.MapFrom(s => s.HasCustomers.ToList()))
            .ForMember(d => d.Discounts,o => o.MapFrom(s => s.Discounts.ToList()));
        CreateMap<ChangeCustomerTypeDto,CustomerType>();
        CreateMap<DomainAccount,DomainAccountDto>();
            // .ForMember(d => d.RegisteredDomains, o => o.MapFrom(s => s.RegisteredDomains));
        CreateMap<ChangeDomainAccountDto,DomainAccount>();
        CreateMap<ChangeDomainProductDto,DomainProduct>();
        CreateMap<DomainProduct,DomainProductDto>()
            .ForMember(d => d.DomainType, o => o.MapFrom(s => ((DomainTypeEnum)s.DomainType).ToString()));
            // .ForMember(d => d.RegisteredDomains, o => o.MapFrom(s => s.RegisteredDomains));
        CreateMap<CreateOrderDto,Order>();
        CreateMap<UpdateOrderDto,Order>();
        CreateMap<Order,OrderDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => ((OrderStatusEnum)s.Status).ToString()));
        CreateMap<ChangePaymentMethodDto,PaymentMethod>();
        CreateMap<PaymentMethod,PaymentMethodDto>();
        CreateMap<Discount,DiscountDto>();
        CreateMap<ChangeDiscountDto,Discount>();
        CreateMap<RegisteredDomain,RegisteredDomainDto>();
        CreateMap<Authorize,AuthorizeDto>();
        CreateMap<RegisterCustomerDto,Customer>();
    }
}