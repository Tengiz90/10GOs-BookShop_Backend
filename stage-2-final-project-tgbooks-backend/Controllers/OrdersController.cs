using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Orders;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<ActionResult<ApiResponse<ICollection<GetOrderWithDetails>?>>> GetAuthors()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersSortedbyDateFromLatestToOldest();

                var response = new ApiResponse<ICollection<GetOrderWithDetails>?> { Message = "Oders retrieval was successful", Data = orders, WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                {
                    var errorResponse = new ApiResponse<ICollection<GetOrderWithDetails>?>
                    {
                        WasSuccessful = false,
                        Message = $"Failed to retrieve orders: {ex.Message}",
                        Data = null
                    };

                    return StatusCode(500, errorResponse);
                }
            }
        }
    }
}
