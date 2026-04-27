using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using stage_2_final_project_tgbooks_backend.Responses.Models.Orders;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
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
        public async Task<ActionResult<ApiResponse<ICollection<GetOrderWithDetails>?>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersSortedbyDateFromLatestToOldestAsync();

                var response = new ApiResponse<ICollection<GetOrderWithDetails>?> { Message = "Oders retrieval was successful", Data = orders, WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                {
                    var errorResponse = new ApiResponse<ICollection<GetOrderWithDetails>?>
                    {
                        WasSuccessful = false,
                        Message = $"Failed to retrieve orders: {ex.InnerException?.Message ?? ex.Message}",
                        Data = null
                    };

                    return StatusCode(500, errorResponse);
                }
            }
        }

        [Authorize]
        [HttpGet("get-by-user")]
        public async Task<ActionResult<ApiResponse<ICollection<GetOrderWithDetails>?>>> GetOrdersByUserId()
        {
            try
            {
               
                var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<EditUserNameResult?>
                    {
                        WasSuccessful = false,
                        Message = "Invalid or missing User ID in token."
                    });
                }

                

                var orders = await _orderService.GetOrdersByUserIdAsync(userId);

                var response = new ApiResponse<ICollection<GetOrderWithDetails>?> { Message = "Oders retrieval was successful", Data = orders, WasSuccessful = true };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<ICollection<GetOrderWithDetails>?>
                {
                    WasSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };

                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                {
                    var errorResponse = new ApiResponse<ICollection<GetOrderWithDetails>?>
                    {
                        WasSuccessful = false,
                        Message = $"Failed to retrieve orders: {ex.InnerException?.Message ?? ex.Message}",
                        Data = null
                    };

                    return StatusCode(500, errorResponse);
                }
            }

        }

    }
}
