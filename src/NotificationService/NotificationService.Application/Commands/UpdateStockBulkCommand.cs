using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public class UpdateStockBulkCommand : IRequest<bool>
{
    public List<StockItemDto> Prodcuts { get; set; }
}