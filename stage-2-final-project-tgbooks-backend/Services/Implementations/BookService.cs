

using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
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
            var authors = new List<Author>();
            foreach (var authorName in request.AuthorNames)
            {
                // Check if author already exists in DB
                var existingAuthor = (await _databaseManager.GetAuthorsAsync())
                    .FirstOrDefault(a => a.Name.Trim().ToLower() == authorName.Trim().ToLower());

                if (existingAuthor != null)
                    authors.Add(existingAuthor);
                else
                    authors.Add(new Author { Name = authorName }); // create new author
            }

            var book = new Book
            {
                Title = request.Title.Trim().ToLower(),
                Authors = authors,
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
            // Map author names to Author entities
            var authors = new List<Author>();
            foreach (var authorName in request.AuthorNames)
            {
                // Check if author already exists in DB
                var existingAuthor = (await _databaseManager.GetAuthorsAsync())
                    .FirstOrDefault(a => a.Name.Trim().ToLower() == authorName.Trim().ToLower());

                if (existingAuthor != null)
                    authors.Add(existingAuthor);
                else
                    authors.Add(new Author { Name = authorName }); // create new author
            }

            var book = new Book
            {
                Id = request.Id,
                Title = request.Title.Trim().ToLower(),
                Authors = authors,
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
                Authors = book.Authors.Select(au => new GetAuthor
                {
                    Id = au.Id,
                    Name = au.Name,
                }).ToList(),
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
                Authors = b.Authors.Select(au => new GetAuthor
                {
                    Id = au.Id,
                    Name = au.Name,
                }).ToList(),
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
                Authors = b.Authors.Select(au => new GetAuthor
                {
                    Id = au.Id,
                    Name = au.Name,
                }).ToList(),
                Title = b.Title,
                ImageURL = b.ImageURL,
                Language = b.Language,
                Quantity = b.Quantity,
            }
            ).ToList();
        }
        public async Task<ICollection<GetBook>> GetBooksByAuthorAsync(int authorId)
        {
            var books = await _databaseManager.GetBooksByAuthorIdAsync(authorId);
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                Authors = b.Authors.Select(au => new GetAuthor
                {
                    Id = au.Id,
                    Name = au.Name,
                }).ToList(),
                Title = b.Title,
                ImageURL = b.ImageURL,
                Language = b.Language,
                Quantity = b.Quantity,
            }
            ).ToList();
        }


        public async Task<ICollection<GetBook>> GetBooksPageAsync(string? title, int pageNumber, int pageSize)
        {
            var books = await _databaseManager.GetBooksPageAsync(title, pageNumber, pageSize);
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                ImageURL = b.ImageURL,
                Authors = b.Authors.Select(au => new GetAuthor
                {
                    Id = au.Id,
                    Name = au.Name,
                }).ToList(),
                Language = b.Language,
                Quantity = b.Quantity,
                Title = b.Title,
            }).ToList();
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
