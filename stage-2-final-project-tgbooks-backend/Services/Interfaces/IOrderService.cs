
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Responses.Models.Orders;

namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ICollection<GetOrderWithDetails>> GetAllOrdersSortedbyDateFromLatestToOldestAsync();
        Task<ICollection<GetOrderWithDetails>> GetOrdersByUserIdAsync(int userId);
    }
}
