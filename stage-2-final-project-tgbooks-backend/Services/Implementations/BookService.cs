

using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Responses.Models.Books;
using stage_2_final_project_tgbooks_backend.Services.AdditionalModels;
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

        public async Task<AddBookResult> AddNewBookAsync(AddBookDto request)
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
                Title = request.Title,
                Authors = authors,
                Language = request.Language,
                Quantity = request.Quantity,
                ImageURL = request.ImageUrl,
                Categories = _databaseManager.GetCategories()
                          .Where(c => request.CategoryIds.Contains(c.Id)).ToList()
            };

            return new AddBookResult
            {
                BookId = await _databaseManager.AddNewBookAsync(book)
            };
        }

        public async Task<EditBookResult> EditBookAsync(EditBookDto request)
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
                Title = request.Title.Trim(),
                OnSale =  request.OnSale,
                OffPercentage = request.OffPercentage,
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

        public async Task<GetBook> GetBookByIdAsync(int id, int? userId)
        {

            var book = await _databaseManager.GetBookByIdAsync(id);
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
                return new GetBook
            {
                Id = id,
                OnSale = book.OnSale,
                OffPercentage = book.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(book.Id),
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

        public async Task<ICollection<GetBook>> GetBooksByCategoryAsync(int categoryId, int? userId)
        {
            var books = await _databaseManager.GetBooksByCategoryIdAsync(categoryId);
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
                return books.Select(b => new GetBook
            {
                Id = b.Id,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(b.Id),
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


        public async Task<ICollection<GetBook>> GetBooksByCategorySortedAsync(int categoryId, int? userId)
        {
            var books = await _databaseManager.GetBooksByCategoryIdSortedByTitleAsync(categoryId);
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
                return books.Select(b => new GetBook
            {
                Id = b.Id,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(b.Id),
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
        public async Task<ICollection<GetBook>> GetBooksByAuthorAsync(int authorId, int? userId)
        {
            var books = await _databaseManager.GetBooksByAuthorIdAsync(authorId);
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
                return books.Select(b => new GetBook
            {
                Id = b.Id,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(b.Id),
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


        public async Task<ICollection<GetBook>> GetBooksPageAsync(string? title, int pageNumber, int pageSize, int? userId)
        {
            var books = await _databaseManager.GetBooksPageAsync(title, pageNumber, pageSize);
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
                return books.Select(b => new GetBook
            {
                Id = b.Id,
                ImageURL = b.ImageURL,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(b.Id),
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

        public async Task<ICollection<GetBook>> GetAllBooksOnSaleAsync(int? userId)
        {
            var books = await _databaseManager.GetAllBooksOnSaleAsync();
            ICollection<int> cartBookIds = new List<int> { };
            if (userId != null)
            {
                cartBookIds = await _databaseManager.GetUserCartBookIdsAsync(userId.Value);
            }
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                ImageURL = b.ImageURL,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
                AlreadyInCart = cartBookIds.Contains(b.Id),
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

        public async Task<ICollection<GetBook>> GetAllDeletedBooksAsync()
        {
            var books = await _databaseManager.GetAllDeletedBooksAsync();
           
            return books.Select(b => new GetBook
            {
                Id = b.Id,
                ImageURL = b.ImageURL,
                OnSale = b.OnSale,
                OffPercentage = b.OffPercentage,
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

        public async Task<int> UnDeleteBookAsync(int bookId)
        {
          return await _databaseManager.UnDeleteBookAsync(bookId);
        }


    }
}
