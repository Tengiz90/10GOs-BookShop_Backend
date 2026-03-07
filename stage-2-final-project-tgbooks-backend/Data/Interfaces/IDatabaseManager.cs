using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;

namespace stage_2_final_project_tgbooks_backend.Data.Interfaces
{
    public interface IDatabaseManager
    {
        Task<int> RemoveUserByIdAsync(int id);
        Task<int> RemoveBookByIdAsync(int id);

        Task<User> EditUserFullNameByIdAsync(string firstName, string LastName, int id);
        Task<int> EditBookByIdAsync(Book updatedBook);
        Task<Order> PurchaseBooksByIdsAsync(ICollection<int> bookIds, ICollection<int> quantitiesToPurchaseEach, int userId);

        Task<Book> GetBookByIdAsync(int id);
        Task<ICollection<Book>> GetBooksByCategoryIdAsync(int id);
        // Get books by category, sorted alphabetically by Title
        Task<ICollection<Book>> GetBooksByCategoryIdSortedByTitleAsync(int categoryId);
        Task<ICollection<Book>> GetAllBooksAsync();
        Task<ICollection<Book>> GetBooksPageAsync(string? title, int pageNumber, int pageSize);
        IQueryable<Category> GetCategories();
        Task<User> GetUserByEmailAndPasswordAsync(string email, string password);

        Task<int> AddNewBookAsync(Book book);
        Task<int> AddUserAsync(User user);

        Task<bool> DoesUserWithSuchEmailAlreadyExistAsync(string email);
        Task<bool> IsUserWithSuchEmailAlreadyVerifiedAsync(string email);

        Task<int> ConfirmUserRegistrationAsync(string email, string code);

        Task<ICollection<Author>> GetAuthorsAsync();
        Task<ICollection<Book>> GetBooksByAuthorIdAsync(int authorId);


    }
}
