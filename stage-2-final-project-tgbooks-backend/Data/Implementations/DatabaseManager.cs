using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Data;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using System.Globalization;
using System.Text;

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

            // Update basic properties
            foundBook.Title = updatedBook.Title;
            foundBook.OnSale = updatedBook.OnSale;
            foundBook.OriginalPrice = updatedBook.OriginalPrice;
            foundBook.Description= updatedBook.Description; 
            foundBook.Language = updatedBook.Language;
            foundBook.OffPercentage = updatedBook.OffPercentage;
            if (updatedBook.Quantity < 0) throw new ArgumentOutOfRangeException(nameof(updatedBook.Quantity), "Book quantity can't be negative");
            foundBook.Quantity = updatedBook.Quantity;
            foundBook.ImageURL = updatedBook.ImageURL;

            // --- SYNC AUTHORS (Removes and Adds) ---

            // 1. Identify which authors to remove (those currently in DB but not in the update request)
            var incomingNames = updatedBook.Authors.Select(a => a.Name.Trim().ToLower()).ToList();
            var authorsToRemove = foundBook.Authors
                .Where(a => !incomingNames.Contains(a.Name.Trim().ToLower()))
                .ToList();

            foreach (var author in authorsToRemove)
            {
                foundBook.Authors.Remove(author); // Removes the link in the join table
            }

            // 2. Identify and add new/existing authors
            foreach (var author in updatedBook.Authors)
            {
                var normalizedName = author.Name.Trim().ToLower();

                // Skip if they are already linked to this book
                if (foundBook.Authors.Any(a => a.Name.Trim().ToLower() == normalizedName))
                    continue;

                // Check if the author exists in the global Authors table
                var existingAuthor = await _db.Authors
                    .FirstOrDefaultAsync(a => a.Name.Trim().ToLower() == normalizedName);

                if (existingAuthor != null)
                {
                    foundBook.Authors.Add(existingAuthor); // Link existing record
                }
                else
                {
                    foundBook.Authors.Add(author); // Insert and link new author record
                }
            }

            // --- SYNC CATEGORIES ---
            foundBook.Categories.Clear();
            foreach (var category in updatedBook.Categories)
            {
                var trackedCategory = await _db.Categories.FindAsync(category.Id);
                if (trackedCategory != null)
                    foundBook.Categories.Add(trackedCategory);
            }

            await _db.SaveChangesAsync();
            return foundBook.Id;
        }

        public async Task<Book> GetBookByIdAsync(int id, bool isUserViewing)
        {
            // 1. Fetch the book and its details normally
            var book = await _db.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new EntityNotFoundException(nameof(Book), id);

            // 2. Track the click by adding a new row to the Clicks table
            if (isUserViewing)
            {
                var newClick = new Click
                {
                    BookId = book.Id,
                    ClickedAt = DateTime.UtcNow
                };

                _db.Clicks.Add(newClick);

                // This is a pure INSERT. SQL Server handles heavy traffic seamlessly here!
                await _db.SaveChangesAsync();
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

        public async Task<ICollection<Book>> GetBooksPageAsync(string? title, int? categoryId, bool? onSale, int pageNumber, int pageSize)
        {
            var query = _db.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            if (categoryId != null && categoryId > 0)
            {
                query = query.Where(b => b.Categories.Any(c => c.Id == categoryId.Value));
            }

            if (onSale == true)
            {
                query = query.Where(b => b.OnSale == true);
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
            return await _db.Books.Where(b => b.OnSale == true && !b.IsDeleted).Include(b => b.Authors).ToListAsync();
        }

  
        public async Task<ICollection<Book>> GetAllDeletedBooksAsync()
        {
            return await _db.Books.Where(b => b.IsDeleted == true).ToListAsync();
        }

        public IQueryable<Category> GetCategories()
        {
            return _db.Categories;
        }

        public string GetCategoryNameByCategoryId(int id)
        {
            var foundCategoryName = _db.Categories
            .Where(c => c.Id == id)
            .Select(c => c.Type)
            .FirstOrDefault();

            if (foundCategoryName == null)
            {
                throw new EntityNotFoundException(nameof(Category), id);
            }
            else
            {
                return foundCategoryName;
            }
        }        

        public async Task<int> RemoveBookByIdAsync(int id)
        {
            //  Find the book
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                throw new EntityNotFoundException(nameof(Book), id);

            //  Access the Carts "indirectly" using _db.Set<Cart>()
            // This works even if we don't have public DbSet<Cart> Carts { get; set; }
            var cartsWithBook = await _db.Set<Cart>()
                .Include(c => c.Items)
                .Where(c => c.Items.Any(ci => ci.BookId == id))
                .ToListAsync();

            //  Remove the specific item from each cart
            foreach (var cart in cartsWithBook)
            {
                var itemToRemove = cart.Items.FirstOrDefault(ci => ci.BookId == id);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                }
            }

            //  Perform Soft Delete
            book.IsDeleted = true;

            //  Save Changes
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

        public async Task<User> GetUserBillingInfoByIdAsync(int id)
        {
            var user = await _db.Users
               .Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new EntityNotFoundException(nameof(User), id);
            
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

                if (book.IsDeleted == true)
                {
                    throw new BookDeletedException(book.Title, book.Language.ToString());
                }

                if (book.Quantity < qtyToPurchase)
                    throw new NotEnoughStockException(
                          book.Title,
                          book.Language.ToString(),
                          book.Quantity,
                          book.Quantity == 0 // true if out of stock
                      );
                // Decrement the stock
                book.Quantity -= qtyToPurchase;

                var pricePerUnit = book.OnSale
                    ? book.OriginalPrice * (100 - book.OffPercentage) / 100
                    : book.OriginalPrice;

                var totalPriceAtOrderTime = qtyToPurchase * pricePerUnit;
                var orderItem = new OrderItem { BookId = bookId, Quantity = qtyToPurchase, TotalPriceAtOrderTime = totalPriceAtOrderTime };
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
                .Include(o => o.Items.OrderByDescending(ci => ci.CreatedAt))
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
            .ThenInclude(c => c.Items.OrderByDescending(ci => ci.CreatedAt))
            .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            return user.Cart;
        }

        public async Task<CartItem> AddItemToCartAsync(int bookId, int userId, int bookQuantity)
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

            //if requested quantity exceeds available stock
            if (bookQuantity > book.Quantity)
                throw new NotEnoughStockException(
                    book.Title,
                    book.Language.ToString(),
                    book.Quantity
                );

            if (bookQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

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

            if (user.Cart.Items.Count >= 100)
                throw new CartIsFullException("User cart is full.");

            var cartItem = new CartItem
            {
                BookId = bookId,
                Cart = user.Cart, // EF will handle CartId automatically
                Quantity = bookQuantity
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

            // Check if Cart is null. If it is, return an empty list immediately.
            if (user.Cart == null || user.Cart.Items == null)
            {
                return new List<int>();
            }

            return user.Cart.Items.Select(ci => ci.BookId).ToList();
        }
        public async Task<int> UnDeleteBookAsync(int bookId)
        {
           var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
           if(book == null) throw new EntityNotFoundException(nameof(book), bookId);
           book.IsDeleted = false; 
           await _db.SaveChangesAsync();
           return book.Id;
        }


        /// <summary>
        /// Generates a pure, flat UTF-8 encoded CSV dataset stream.
        /// OPTIMIZED FOR: Python Pandas ingestion loops (pd.read_csv).
        /// </summary>
        public async Task<byte[]> GenerateBooksMlDataCsvAsync()
        {
            var books = await _db.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .Include(b => b.Clicks) // <-- This ensures the collection isn't null
                .AsNoTracking()
                .ToListAsync();

            var cartCounts = await _db.Set<CartItem>()
                .AsNoTracking()
                .GroupBy(ci => ci.BookId)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BookId, x => x.Count);

            var orderMetrics = await _db.Set<OrderItem>()
                .AsNoTracking()
                .GroupBy(oi => oi.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    OrderAppearances = g.Count(),
                    TotalUnitsSold = g.Sum(oi => oi.Quantity)
                })
                .ToDictionaryAsync(x => x.BookId, x => x);

            var allSystemCategories = await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => c.Type)
                .ToListAsync();

            var csvBuilder = new StringBuilder();

            // Build CSV Header Row
            var baseHeader = "BookId,Title,Language,OriginalPrice,OnSale,OffPercentage,CurrentPrice,StockQuantity,TimesClicked,CartCount,OrderCount,TotalUnitsSold,CategoryCount,AuthorCount";

            foreach (var categoryName in allSystemCategories)
            {
                string cleanColumnName = categoryName.Replace(" ", "_").Replace("-", "_");
                baseHeader += $",Category_{cleanColumnName}";
            }
            csvBuilder.AppendLine(baseHeader);

            // Populate CSV Rows
            foreach (var book in books)
            {
                cartCounts.TryGetValue(book.Id, out var cartCount);

                int orderCount = 0;
                int totalUnitsSold = 0;
                if (orderMetrics.TryGetValue(book.Id, out var metrics))
                {
                    orderCount = metrics.OrderAppearances;
                    totalUnitsSold = metrics.TotalUnitsSold;
                }

                decimal currentPrice = book.OnSale
                    ? book.OriginalPrice * (1 - ((decimal)book.OffPercentage / 100))
                    : book.OriginalPrice;

                // Handle CSV quote escaping rules cleanly
                string escapedTitle = book.Title.Contains(",") || book.Title.Contains("\"")
                    ? $"\"{book.Title.Replace("\"", "\"\"")}\""
                    : book.Title;

                var line = $"{book.Id}," +
                           $"{escapedTitle}," +
                           $"{(int)book.Language}," +
                           $"{book.OriginalPrice.ToString("F2", CultureInfo.InvariantCulture)}," +
                           $"{(book.OnSale ? 1 : 0)}," +
                           $"{book.OffPercentage}," +
                           $"{currentPrice.ToString("F2", CultureInfo.InvariantCulture)}," +
                           $"{book.Quantity}," +
                           $"{book.TotalClicks}," +
                           $"{cartCount}," +
                           $"{orderCount}," +
                           $"{totalUnitsSold}," +
                           $"{book.Categories.Count}," +
                           $"{book.Authors.Count}";

                foreach (var categoryName in allSystemCategories)
                {
                    int isAssociated = book.Categories.Any(c => c.Type == categoryName) ? 1 : 0;
                    line += $",{isAssociated}";
                }

                csvBuilder.AppendLine(line);
            }

            // Fallback strategy: Prepend raw byte signatures to help local text readers parse Unicode layouts
            byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            byte[] bomBytes = { 0xEF, 0xBB, 0xBF };
            byte[] combinedResult = new byte[bomBytes.Length + csvBytes.Length];
            Buffer.BlockCopy(bomBytes, 0, combinedResult, 0, bomBytes.Length);
            Buffer.BlockCopy(csvBytes, 0, combinedResult, bomBytes.Length, csvBytes.Length);

            return combinedResult;
        }

        /// <summary>
        /// Alternative layout engine that converts the dataset matrix into a Tab-Separated Unicode format.
        /// OPTIMIZED FOR: Flawless double-click execution directly in Excel 2019 without code conversion.
        /// </summary>
        public async   Task<byte[]> GenerateBooksMlDataExcelFriendlyAsync()
        {
            // Call our underlying data logic to fetch the clean CSV text string array
            byte[] rawCsvBytes = await GenerateBooksMlDataCsvAsync();
            string csvContent = Encoding.UTF8.GetString(rawCsvBytes);

            // Excel instantly recognizes UTF-16 Little Endian when combined with traditional tab indexing
            string tabDelimitedContent = csvContent
                .Replace("sep=\n", "") // Clean out header artifacts if present
                .Replace(",", "\t");   // Convert row elements to clean sheet tabs

            return Encoding.Unicode.GetBytes(tabDelimitedContent);
        }


        public async Task<byte[]> GenerateAuthorMlDatasetAsync()
        {
            var csvBuilder = new StringBuilder();

            // 1. Write the tabular matrix headers based on your models
            csvBuilder.AppendLine("AuthorId,TotalBooksCount,TotalBookClicks,TotalAddedToCart,TotalOrderedCount");

            try
            {
                // 2. Query and aggregate metrics navigating through your model navigation collections
                var authorsMetrics = await _db.Authors
                    .Select(author => new
                    {
                        AuthorId = author.Id,

                        // Count of books tied to this author
                        TotalBooksCount = author.Books.Count,

                        // Aggregate popularity via total times their books were clicked
                        TotalBookClicks = author.Books.Any()
                            ? author.Books.Sum(b => b.TotalClicks)
                            : 0,

                        // Query CartItem table via Book relationship to find how many times added to a cart
                        TotalAddedToCart = _db.Set<Data.Models.CartItem>()
                            .Where(ci => author.Books.Select(b => b.Id).Contains(ci.BookId))
                            .Sum(ci => (int?)ci.Quantity) ?? 0,

                        // Query OrderItem table via Book relationship to find total verified completed sales
                        TotalOrderedCount = _db.Set<Data.Models.OrderItem>()
                            .Where(oi => author.Books.Select(b => b.Id).Contains(oi.BookId))
                            .Sum(oi => (int?)oi.Quantity) ?? 0
                    })
                    .ToListAsync();

                // 3. Build the CSV lines string rows
                foreach (var metric in authorsMetrics)
                {
                    string line = $"{metric.AuthorId}," +
                                 $"{metric.TotalBooksCount}," +
                                 $"{metric.TotalBookClicks}," +
                                 $"{metric.TotalAddedToCart}," +
                                 $"{metric.TotalOrderedCount}";

                    csvBuilder.AppendLine(line);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed compiling dataset matrix due to internal contextual querying faults.", ex);
            }

            // 4. Return as raw UTF-8 byte stream matching your service pipeline mechanics
            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> GenerateClickMlDatasetAsync()
        {
            // 1. Fetch data efficiently and load the nested collections through the Book property
            var clickData = await _db.Clicks
                .AsNoTracking()
                .Select(c => new
                {
                    ClickId = c.Id,
                    c.BookId,
                    BookTitle = c.Book.Title,
                    BookLanguage = c.Book.Language.ToString(),
                    OriginalPrice = c.Book.OriginalPrice,
                    OffPercentage = c.Book.OffPercentage,
                    OnSale = c.Book.OnSale,

                    // Flatten the Many-to-Many lists into Excel/ML friendly text formats
                    Authors = c.Book.Authors.Select(a => a.Name),
                    Categories = c.Book.Categories.Select(cat => cat.Type),

                    // Track timestamp breakdowns
                    ClickHour = c.ClickedAt.AddHours(4).Hour,
                    ClickDayOfWeek = c.ClickedAt.AddHours(4).DayOfWeek.ToString(),
                    Timestamp = c.ClickedAt.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            // 2. Stream content directly into a StringBuilder
            var csvBuilder = new StringBuilder();

            // Write Header Row (Optimized with rich features for ML target modeling)
            csvBuilder.AppendLine("ClickId,BookId,BookTitle,Language,OriginalPrice,OffPercentage,OnSale,Authors,Categories,ClickHour,ClickDayOfWeek,Timestamp");

            // Write Data Rows
            foreach (var click in clickData)
            {
                // Join multiple authors/categories with a semicolon inside the quote boundaries
                string formattedAuthors = string.Join("; ", click.Authors).Replace("\"", "\"\"");
                string formattedCategories = string.Join("; ", click.Categories).Replace("\"", "\"\"");
                string safeTitle = click.BookTitle.Replace("\"", "\"\"");

                csvBuilder.AppendLine($"{click.ClickId}," +
                                     $"{click.BookId}," +
                                     $"\"{safeTitle}\"," +
                                     $"\"{click.BookLanguage}\"," +
                                     $"{click.OriginalPrice}," +
                                     $"{click.OffPercentage}," +
                                     $"{click.OnSale.ToString().ToLower()}," +
                                     $"\"{formattedAuthors}\"," +
                                     $"\"{formattedCategories}\"," +
                                     $"{click.ClickHour}," +
                                     $"{click.ClickDayOfWeek}," +
                                     $"{click.Timestamp}");
            }

            // 3. Return as a native UTF-8 CSV Byte Array
            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

    }



}
