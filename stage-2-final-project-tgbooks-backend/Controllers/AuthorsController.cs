using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Responses;

using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _aurthorsService;
        public AuthorsController(IAuthorService aurthorsService)
        {
            _aurthorsService = aurthorsService;
        }


        [HttpGet("get-all")]
        public async Task<ActionResult<ApiResponse<ICollection<GetAuthor>?>>> GetAuthors()
        {
            try
            {
                var authors = await _aurthorsService.GetAuthorsAsync();
                var response = new ApiResponse<ICollection<GetAuthor>?> { Message = "Authors retrieval was successful", Data = authors, WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                {
                    var errorResponse = new ApiResponse<ICollection<GetAuthor>?>
                    {
                        WasSuccessful = false,
                        Message = $"Failed to retrieve authors: {ex.Message}",
                        Data = null
                    };

                    return StatusCode(500, errorResponse);
                }
            }
        }
    }
}
