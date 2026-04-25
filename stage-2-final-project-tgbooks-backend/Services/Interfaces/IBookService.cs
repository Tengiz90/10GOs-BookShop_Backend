

using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using stage_2_final_project_tgbooks_backend.Services.AdditionalModels;

namespace WebApplication2.Services.Interfaces
{
    public interface IBookService
    {

        Task<AddBookResult> AddNewBookAsync(AddBookDto request);
        Task<EditBookResult> EditBookAsync(EditBookDto request);
        Task<RemoveBookByIdResult> RemoveBookAsync(int id);

        Task<GetBook> GetBookByIdAsync(int id, int? userId);
        Task<ICollection<GetBook>> GetBooksByCategoryAsync(int categoryId, int? userId);
        Task<ICollection<GetBook>> GetBooksByCategorySortedAsync(int categoryId, int? userId);
        Task<ICollection<GetBook>> GetBooksByAuthorAsync(int authorId, int? userId);
        Task<ICollection<GetBook>> GetBooksPageAsync(string? title, int? categoryId, int pageNumber, int pageSize, int? userId);

        Task<ICollection<GetBook>> GetAllBooksOnSaleAsync(int? userId);

        Task<ICollection<GetBook>> GetAllDeletedBooksAsync();
        Task<int> UnDeleteBookAsync(int bookId);

    }
}
