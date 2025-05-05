using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class UpdateOrderDto
{
    public OrderStatusEnum Status { get; set; }
}