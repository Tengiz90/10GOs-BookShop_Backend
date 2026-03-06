

using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class BookService : IBookService
    {
        private readonly IDatabaseManager _databaseManager;

        public BookService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<AddBookResult> AddNewBookAsync(AddNewBook request)
        {
            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Language = request.Language,
                Quantity = request.Quantity,
                ImageURL = request.ImageURL,
                Categories = _databaseManager.GetCategories()
                          .Where(c => request.CategoryIds.Contains(c.Id)).ToList()
            };

            return new AddBookResult
            {
                BookId = await _databaseManager.AddNewBookAsync(book)
            };
        }

        public async Task<EditBookResult> EditBookAsync(EditBook request)
        {
            var book = new Book
            {
                Id = request.Id,
                Title = request.Title,
                Author = request.Author,
                Language = request.Language,
                Quantity = request.Quantity,
                ImageURL = request.ImageURL,
                Categories = _databaseManager.GetCategories()
                         .Where(c => request.CategoryIds.Contains(c.Id)).ToList()
            };
            return new EditBookResult
            {
               BookId = await _databaseManager.EditBookByIdAsync(book)
            };
        }

        public async Task<GetBook> GetBookByIdAsync(int id)
        {

            var book = await _databaseManager.GetBookByIdAsync(id);
            return new GetBook
            {
                Id = id,
                Author = book.Author,
                Title = book.Title,
                ImageURL = book.ImageURL,
                Language = book.Language,
                Quantity = book.Quantity,
            };
        }

        public async Task<ICollection<GetBook>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _databaseManager.GetBooksByCategoryIdAsync(categoryId);
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                Author = b.Author,
                Title = b.Title,
                ImageURL = b.ImageURL,
                Language = b.Language,
                Quantity = b.Quantity,
            }
            ).ToList();
        }

        public async Task<ICollection<GetBook>> GetBooksByCategorySortedAsync(int categoryId)
        {
            var books = await _databaseManager.GetBooksByCategoryIdSortedByTitleAsync(categoryId);
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                Author = b.Author,
                Title = b.Title,
                ImageURL = b.ImageURL,
                Language = b.Language,
                Quantity = b.Quantity,
            }
            ).ToList();
        }

        public async Task<ICollection<Book>> GetBooksPageAsync(int pageNumber, int pageSize)
        {
            var books = await _databaseManager.GetBooksPageAsync(pageNumber, pageSize);
            return books;
        }

  
        public async Task<RemoveBookByIdResult> RemoveBookAsync(int id)
        {
            return new RemoveBookByIdResult
            {
                BookId = await _databaseManager.RemoveBookByIdAsync(id),
            };
        }

      
    }
}
