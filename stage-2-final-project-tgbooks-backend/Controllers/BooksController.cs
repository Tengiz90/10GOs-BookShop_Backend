using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Helpers;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using stage_2_final_project_tgbooks_backend.Services.AdditionalModels;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
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
        private readonly IStorageService _storageService;

        public BooksController(
            IBookService bookService,
            IValidator<AddNewBook> addBookValidator,
            IValidator<EditBook> editBookValidator,
            IStorageService storageService

            )
        {
            _bookService = bookService;
            _addBookValidator = addBookValidator;
            _editBookValidator = editBookValidator;
            _storageService = storageService;
        }

        [HttpGet("get-by-category")]
        public async Task<ActionResult<ApiResponse<ICollection<GetBook>?>>> GetBooksByCategory(
            int categoryId,
            [FromQuery] bool sorted = false)
        {
            try
            {
                int? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst("id");

                    if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
                    {
                        userId = parsedId;
                    }
                }


                var books = sorted
                  ? await _bookService.GetBooksByCategorySortedAsync(categoryId, userId)
                  : await _bookService.GetBooksByCategoryAsync(categoryId, userId);

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
                int? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst("id");

                    if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
                    {
                        userId = parsedId;
                    }
                }


                var book = await _bookService.GetBookByIdAsync(id, userId);

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
                int? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst("id");

                    if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
                    {
                        userId = parsedId;
                    }
                }

                var books = await _bookService.GetBooksPageAsync(title, pageNumber, pageSize, userId);
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

        [Authorize (Roles = "Admin")]
        [HttpPut("edit")]
        public async Task<ActionResult<ApiResponse<EditBookResult?>>> EditBookById(EditBook book)
        {
            var validationResult = await _editBookValidator.ValidateAsync(book);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<EditBookResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var imageUrl = book.ImageUrlBefore;
                if (book.Image != null)
                {
                    imageUrl = await _storageService.UploadFileAsync(book.Image);
                    await _storageService.DeleteFileAsync(book.ImageUrlBefore);
                }
                var editiedBook = new EditBookDto
                {
                    Id = book.Id,
                    AuthorNames = book.AuthorNames,
                    Title = book.Title,
                    CategoryIds = book.CategoryIds,
                    ImageURL = imageUrl,
                    Language = book.Language,
                    Quantity = book.Quantity,
                };

                var editBookResult = await _bookService.EditBookAsync(editiedBook);
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
                    Message = $"Failed to edit book: {ex.InnerException?.Message ?? ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<AddBookResult?>>> AddBook(AddNewBook addNewBook)
        {

            var validationResult = await _addBookValidator.ValidateAsync(addNewBook);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<AddBookResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var imageUrl = await _storageService.UploadFileAsync(addNewBook.Image);
                var bookToAddDto = new AddBookDto
                {
                    AuthorNames = addNewBook.AuthorNames,
                    Title = addNewBook.Title,
                    CategoryIds = addNewBook.CategoryIds,
                    ImageUrl = imageUrl,
                    Language = addNewBook.Language,
                    Quantity = addNewBook.Quantity,
                };


                var addBookResult = await _bookService.AddNewBookAsync(bookToAddDto);
                var response = new ApiResponse<AddBookResult?> { Data = addBookResult, Message = "Book was added succesfully", WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
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
            int? userId = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst("id");

                if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
                {
                    userId = parsedId;
                }
            }

            try
            {
                var books = await _bookService.GetBooksByAuthorAsync(authorId, userId);

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

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<RemoveBookByIdResult?>>> DeleteBook(RemoveBook bookInfo)
        {
            try
            {
                var deleteBookResult = await _bookService.RemoveBookAsync(bookInfo.Id);
                await _storageService.DeleteFileAsync(bookInfo.imageUrl);

                var response = new ApiResponse<RemoveBookByIdResult?> { Data = deleteBookResult, Message = "Deletion was successful", WasSuccessful = true };
                return Ok(response);

            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<RemoveBookByIdResult?>
                {
                    WasSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ICollection<RemoveBookByIdResult>?>
                {
                    WasSuccessful = false,
                    Message = $"Could not delete the book: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
