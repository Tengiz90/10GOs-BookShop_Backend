

using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;

namespace WebApplication2.Services.Interfaces
{
    public interface IBookService
    {

        Task<AddBookResult> AddNewBookAsync(AddNewBook request);
        Task<EditBookResult> EditBookAsync(EditBook request);
        Task<RemoveBookByIdResult> RemoveBookAsync(int id);

        Task<GetBook> GetBookByIdAsync(int id);
        Task<ICollection<GetBook>> GetBooksByCategoryAsync(int categoryId);
        Task<ICollection<GetBook>> GetBooksByCategorySortedAsync(int categoryId);
        Task<ICollection<Book>> GetBooksPageAsync(int pageNumber, int pageSize);

  
    }
}
