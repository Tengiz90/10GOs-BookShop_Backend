using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using WebApplication2.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IValidator<AddNewBook> _addBookValidator;
        private readonly IValidator<EditBook> _editBookValidator;
        public BooksController(
            IBookService bookService,
            IValidator<AddNewBook> addBookValidator,
            IValidator<EditBook> editBookValidator)
        {
            _bookService = bookService;
            _addBookValidator = addBookValidator;
            _editBookValidator = editBookValidator;
        }

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
         [FromQuery] string? title, // optional search
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 20)
        {
            try
            {
                var books = await _bookService.GetBooksPageAsync(title, pageNumber, pageSize);
                var response = new ApiResponse<ICollection<GetBook>?>
                {
                    WasSuccessful = true,
                    Message = "Books retrieved successfully",
                    Data = books
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
            var validationResult = await _editBookValidator.ValidateAsync(book);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var editBookResult = await _bookService.EditBookAsync(book);
                var response = new ApiResponse<EditBookResult?>
                {
                    WasSuccessful = true,
                    Message = "Editing book was successful",
                    Data = editBookResult
                };
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

        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<AddBookResult?>>> AddBook(AddNewBook addNewBook)
        {
            var validationResult = await _addBookValidator.ValidateAsync(addNewBook);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var addBookResult = await _bookService.AddNewBookAsync(addNewBook);
                var response = new ApiResponse<AddBookResult?> { Data = addBookResult, Message = "Book was added succesfully", WasSuccessful = true };
                return Ok(response);
            } catch (Exception ex)
            {
                var errorResponse = new ApiResponse<AddBookResult?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to add book: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("get-by-author")]
        public async Task<ActionResult<ApiResponse<ICollection<GetBook>?>>> GetBooksByAuthor(
            int authorId)
        {
            try
            {
                var books = await _bookService.GetBooksByAuthorAsync(authorId);

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
    }
}
