using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Categories;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-categories")]
        public async Task<ActionResult<ApiResponse<ICollection<GetCategory>?>>> GetCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategoriesAsync().Select(
                    c => new GetCategory
                    {
                        Id = c.Id,
                        Type = c.Type,
                    }).ToList();
                var response = new ApiResponse<ICollection<GetCategory>?> { Message = "Categories retrieval was successful", Data = categories, WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                {
                    var errorResponse = new ApiResponse<GetCategory?>
                    {
                        WasSuccessful = false,
                        Message = $"Failed to retrieve categories: {ex.Message}",
                        Data = null
                    };

                    return StatusCode(500, errorResponse);
                }
            }
        }
    }
}
