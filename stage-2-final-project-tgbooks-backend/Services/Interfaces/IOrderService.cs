
using stage_2_final_project_tgbooks_backend.Responses.Orders;

namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ICollection<GetOrderWithDetails>> GetAllOrdersSortedbyDateFromLatestToOldest();
    }
}
