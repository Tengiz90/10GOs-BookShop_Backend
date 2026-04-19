using Microsoft.EntityFrameworkCore;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Data;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;

namespace stage_2_final_project_tgbooks_backend.DaEditBookByIdEditBookByIdAsyncta.Implementations
{

    public class DatabaseManager : IDatabaseManager
    {
        private readonly DatabaseContext _db;
        public DatabaseManager(DatabaseContext db)
        {
            _db = db;
        }


        public async Task<int> AddNewBookAsync(Book book)
        {

            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return book.Id;


        }

        public async Task<int> EditBookByIdAsync(Book updatedBook)
        {
            var foundBook = await _db.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == updatedBook.Id);

            if (foundBook == null)
                throw new EntityNotFoundException(nameof(Book), updatedBook.Id);

            foundBook.Title = updatedBook.Title;
            foundBook.OnSale = updatedBook.OnSale;
            foundBook.OffPercentage = updatedBook.OffPercentage;
            if (updatedBook.Quantity < 0) throw new ArgumentOutOfRangeException(nameof(updatedBook.Quantity), "Book quantity can't be negative");
            foundBook.Quantity = updatedBook.Quantity;
            foundBook.ImageURL = updatedBook.ImageURL;

            foreach (var author in updatedBook.Authors)
            {
                // Check if this author already exists in the database
                var existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
                if (existingAuthor != null)
                {
                    // Attach the tracked entity
                    foundBook.Authors.Add(existingAuthor);
                }
                else
                {
                    // New author, EF will insert it
                    foundBook.Authors.Add(author);
                }
            }


            foundBook.Categories.Clear();
            foreach (var category in updatedBook.Categories)
            {
                var trackedCategory = await _db.Categories.FindAsync(category.Id);
                if (trackedCategory != null)
                    foundBook.Categories.Add(trackedCategory);
            }

            await _db.SaveChangesAsync();
            return updatedBook.Id;

        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            var book = await _db.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                throw new EntityNotFoundException(nameof(Book), id);
            }
            return book;


        }

        public async Task<ICollection<Book>> GetBooksByCategoryIdAsync(int id)
        {
            return await _db.Books
                            .Where(b => (b.Categories.Any(c => c.Id == id)) && !b.IsDeleted)
                            .Include(b => b.Authors)
                            .ToListAsync();
        }

        public async Task<ICollection<Book>> GetBooksByCategoryIdSortedByTitleAsync(int categoryId)
        {
            return await _db.Books
                .Where(b => (b.Categories.Any(c => c.Id == categoryId)) && !b.IsDeleted)
                .Include(b => b.Authors)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            return await _db.Books.Include(b => b.Authors).Where(b => !b.IsDeleted).ToListAsync();
        }

        public async Task<ICollection<Book>> GetBooksPageAsync(string? title, int pageNumber, int pageSize)
        {
            var query = _db.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .Where(b => !b.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Include(b => b.Authors)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<ICollection<Book>> GetAllBooksOnSaleAsync()
        {
            return await _db.Books.Where (b => b.OnSale == true).ToListAsync();
        }

  
        public async Task<ICollection<Book>> GetAllDeletedBooksAsync()
        {
            return await _db.Books.Where(b => b.IsDeleted == true).ToListAsync();
        }

        public IQueryable<Category> GetCategories()
        {
            return _db.Categories;
        }

        public async Task<int> RemoveBookByIdAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                throw new EntityNotFoundException(nameof(Book), id);

            //_db.Books.Remove(book);
            book.IsDeleted = true;
            await _db.SaveChangesAsync();
            return book.Id;
        }


        public async Task<int> AddUserAsync(User user)
        {

            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email && u.IsVerified);

            if (existingUser != null)
            {
                throw new Exception("A verified user with this email already exists.");
            }


            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user.Id;
        }


        public async Task<User> EditUserFullNameByIdAsync(string firstName, string lastName, int id)
        {
            var dbUser = await _db.Users.FindAsync(id);
            if (dbUser == null)
                throw new EntityNotFoundException(nameof(User), id);

            dbUser.FirstName = firstName;
            dbUser.LastName = lastName;

            await _db.SaveChangesAsync();
            return dbUser;
        }

        public async Task<bool> DoesUserWithSuchEmailAlreadyExistAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email && u.IsVerified == true);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new EntityNotFoundException(nameof(User), email);

            else return user;
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var users = await _db.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(oi => oi.Book)
                .Where(u => u.Email == email)
                .ToListAsync();

            if (!users.Any())
                throw new EntityNotFoundException(nameof(User), email);

            var user = users.FirstOrDefault(u =>
                BCrypt.Net.BCrypt.Verify(password, u.PasswordHash));

            if (user == null)
                throw new AuthenticationException("Invalid email or password.");

            return user;
        }

        public async Task<int> RemoveUserByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) throw new EntityNotFoundException(nameof(User), id);

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return user.Id;
        }

        public async Task<int> ConfirmUserRegistrationAsync(string email, int userId, string code)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Email == email);
            if (user == null) throw new EntityNotFoundException(nameof(User), email + " and " + userId);

            if (user.EmailVerificationCode == code)
            {
                user.IsVerified = true;
                user.EmailVerificationCode = null; // clear code after verification
                await _db.SaveChangesAsync();
                return user.Id;
            }

            throw new EmailConfirmationException("Entered code was incorrect");
        }

        public async Task<bool> IsUserWithSuchEmailAlreadyVerifiedAsync(string email)
        {
            var isVerified = await _db.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => (bool)u.IsVerified)
                .FirstOrDefaultAsync();


            return isVerified;
        }
        public async Task<Order> PurchaseBooksByIdsAsync(ICollection<int> bookIds, ICollection<int> quantitiesToPurchaseEach, int userId)
        {
            if (bookIds.Count != quantitiesToPurchaseEach.Count)
                throw new ArgumentException("Book IDs and quantities must match.");

            if (!bookIds.Any())
                throw new ArgumentException("No books selected.");

            var orderItems = new List<OrderItem>();
            // Loop through each book and attempt purchase
            for (int i = 0; i < bookIds.Count; i++)
            {
                int bookId = bookIds.ElementAt(i);
                int qtyToPurchase = quantitiesToPurchaseEach.ElementAt(i);

                if (qtyToPurchase <= 0)
                    throw new ArgumentOutOfRangeException("You can't purchase Negative or Zero amount"); // invalid quantity

                // Find the book by primary key
                var book = await _db.Books.FindAsync(bookId);
                if (book == null)
                    throw new EntityNotFoundException(nameof(Book), bookId);

                if (book.Quantity < qtyToPurchase)
                    throw new NotEnoughStockException(
                          book.Title,
                          book.Language.ToString(),
                          book.Quantity,
                          book.Quantity == 0 // true if out of stock
                      );
                // Decrement the stock
                book.Quantity -= qtyToPurchase;


                var orderItem = new OrderItem { BookId = bookId, Quantity = qtyToPurchase };
                orderItems.Add(orderItem);
            }

            var order = new Order { CreatedAt = DateTime.Now, Items = orderItems, UserId = userId };
            _db.Orders.Add(order);
            // Persist both stock updates and the new order
            await _db.SaveChangesAsync();
            return order;

        }

        public async Task<ICollection<Author>> GetAuthorsAsync()
        {
            return await _db.Authors.ToListAsync();
        }

        public async Task<ICollection<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var books = await _db.Books
                .Include(b => b.Authors)    // include authors so we can filter
                .Where(b => (b.Authors.Any(a => a.Id == authorId)) && !b.IsDeleted) // filter by author
                .ToListAsync();

            return books;
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<bool> IsVerificationCodeUniqueForEmailAsync(string code, string email)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(code));

            bool exists = await _db.Users
                .AnyAsync(u => !u.IsVerified && u.EmailVerificationCode == code && u.Email == email);

            return !exists;
        }

        public async Task<ICollection<Order>> GetAllOrdersSortedbyDateFromLatestToOldestAsync()
        {
            return await _db.Orders
                 .OrderByDescending(o => o.CreatedAt)
                 .Include(o => o.User)
                 .ThenInclude(u => u.Address)
                 .Include(o => o.Items)
                 .ThenInclude(oi => oi.Book)
                 .ToListAsync();
        }

        public async Task<ICollection<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                throw new EntityNotFoundException("User", userId);
            }

            return await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .ThenInclude(u => u.Address)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();
        }

        public async Task UpdateBillingAddressByUserIdAsync(int userId, Address updatedAddress)
        {

            var user = await _db.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new EntityNotFoundException("User", userId);
            }
            user.Address.Address1 = updatedAddress.Address1;
            user.Address.Address2 = updatedAddress.Address2;
            user.Address.City = updatedAddress.City;
            user.Address.PostalCode = updatedAddress.PostalCode;

            _db.Users.Update(user);



            await _db.SaveChangesAsync();
        }

        public async Task<Cart> GetUserCartByUserIdAsync(int userId)
        {
            var user = await _db.Users
            .Include(u => u.Cart)
            .ThenInclude(c => c.Items)
            .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            return user.Cart;
        }

        public async Task<CartItem> AddItemToCartAsync(int bookId, int userId)
        {
            var user = await _db.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

           
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                throw new EntityNotFoundException(nameof(Book), bookId);

            if (book.Quantity <= 0)
                throw new NotEnoughStockException(book.Title, book.Language.ToString(), 0, true);

            if (user.Cart == null)
            {
                user.Cart = new Cart
                {
                    User = user // EF will track UserId automatically
                                // Items is already initialized to empty list
                };
               
            }

            var existingItem = user.Cart.Items
                .FirstOrDefault(ci => ci.BookId == bookId);

            if (existingItem != null)
                throw new AlreadyInCartException("Book is already in your cart");

            if (user.Cart.Items.Count >= 200)
                throw new CartIsFullException("User cart is full.");

            var cartItem = new CartItem
            {
                BookId = bookId,
                Cart = user.Cart, // EF will handle CartId automatically
                Quantity = 1
            };

            user.Cart.Items.Add(cartItem);

            await _db.SaveChangesAsync();

            return cartItem;
        }
        public async Task<int> RemoveItemFromCartAsync(int cartItemId, int userId)
        {
            var user = await _db.Users
             .Include(u => u.Cart)
             .ThenInclude(c => c.Items)
             .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            var item = user.Cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new EntityNotFoundException(nameof(CartItem), cartItemId);

            user.Cart.Items.Remove(item);

            await _db.SaveChangesAsync();

            return item.Id;

        }

        public async Task<CartItem> ChangeCartItemQuantityAsync(int cartItemId, int quantity, int userId)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

            var user = await _db.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            var item = user.Cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new EntityNotFoundException(nameof(CartItem), cartItemId);

            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
            if (book == null)
                throw new EntityNotFoundException(nameof(Book), item.BookId);

            if (quantity > book.Quantity)
            {
                throw new NotEnoughStockException(book.Title, book.Language.ToString(), book.Quantity, book.Quantity == 0);
            }

            item.Quantity = quantity;

            await _db.SaveChangesAsync();

            return item;
        }

        public async Task<ICollection<int>> GetUserCartBookIdsAsync(int userId)
        {
            var user = await _db.Users
              .Include(u => u.Cart)
              .ThenInclude(c => c.Items)
              .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), userId);
            }
            var userCart = user.Cart;
            return userCart.Items.Select(ci => ci.BookId)
                .ToList();
        }

        public async Task<int> UnDeleteBookAsync(int bookId)
        {
           var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
           if(book == null) throw new EntityNotFoundException(nameof(book), bookId);
           book.IsDeleted = false; 
           await _db.SaveChangesAsync();
           return book.Id;
        }
    }
}
