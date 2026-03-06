using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.DaEditBookByIdEditBookByIdAsyncta.Implementations;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using WebApplication2.Services.Interfaces;
using static System.Reflection.Metadata.BlobBuilder;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [HttpGet("get-by-category")]
        public async Task<ActionResult<ApiResponse<ICollection<GetBook>?>>> GetBooksByCategory(
            int categoryId,
            [FromQuery] bool sorted = false)
        {
            try
            {
                var books = sorted
                  ? await _bookService.GetBooksByCategorySortedAsync(categoryId)
                  : await _bookService.GetBooksByCategoryAsync(categoryId);

                var response = new ApiResponse<ICollection<GetBook>?>
                {
                    Data = books,
                    Message = "Books retrieved successfully",
                    WasSuccessful = true
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ICollection<GetBook>?>
                {
                    WasSuccessful = false,
                    Message = $"Could not get the books: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetBook?>>> GetBookById(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);

                var response = new ApiResponse<GetBook?>
                {
                    WasSuccessful = true,
                    Message = "Book retrieved successfully",
                    Data = book
                };

                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<GetBook?>
                {
                    WasSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };

                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<GetBook?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to retrieve book: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }

        }
        [HttpGet("page")]
        public async Task<ActionResult<ApiResponse<ICollection<GetBook>?>>> GetBooksPage(
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 20)
         {
            try
            {
                var books = await _bookService.GetBooksPageAsync(pageNumber, pageSize);
                var response = new ApiResponse<ICollection<GetBook>?>
                {
                    WasSuccessful = true,
                    Message = "Books retrieved successfully",
                    Data = books.Select(b => new GetBook
                    {
                        Id = b.Id,
                        ImageURL = b.ImageURL,
                        Author  = b.Author,
                        Language = b.Language,
                        Quantity = b.Quantity,
                        Title = b.Title,
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ICollection<GetBook>?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to retrieve books: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse); 
            }
        }


        [HttpPut("edit")]
        public async Task<ActionResult<ApiResponse<EditBookResult?>>> EditBookById(EditBook book)
        {
            try
            {
              var editBookResult = await _bookService.EditBookAsync(book);
              var response = new ApiResponse<EditBookResult?> { 
                  WasSuccessful = true,
                  Message = "Editing book was successfull",
                  Data = editBookResult };
                return Ok(response);
            }
            catch (EntryPointNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<EditBookResult?>
                {
                    WasSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<EditBookResult?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to edit book: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }

    }
}
