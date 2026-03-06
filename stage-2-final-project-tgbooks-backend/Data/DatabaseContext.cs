using Microsoft.EntityFrameworkCore;
using stage_2_final_project_tgbooks_backend.Data.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace stage_2_final_project_tgbooks_backend.Data
{

    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
          : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
 
            builder.Entity<Category>().HasData
                (
                    new Category { Id = 1, Type = "Fiction" },
                    new Category { Id = 2, Type = "Non-Fiction" },
                    new Category { Id = 3, Type = "Science Fiction" },
                    new Category { Id = 4, Type = "Fantasy" },
                    new Category { Id = 5, Type = "Horror" },
                    new Category { Id = 6, Type = "Dystopian" },
                    new Category { Id = 7, Type = "Mystery" },
                    new Category { Id = 8, Type = "Thriller" },
                    new Category { Id = 9, Type = "Romance" },
                    new Category { Id = 10, Type = "Biography" },
                    new Category { Id = 11, Type = "Autobiography" },
                    new Category { Id = 12, Type = "History" },
                    new Category { Id = 13, Type = "Philosophy" },
                    new Category { Id = 14, Type = "Science" },
                    new Category { Id = 15, Type = "Technology" },
                    new Category { Id = 16, Type = "Psychology" },
                    new Category { Id = 17, Type = "Self-Help" },
                    new Category { Id = 18, Type = "Business" },
                    new Category { Id = 19, Type = "Economics" },
                    new Category { Id = 20, Type = "Politics" },
                    new Category { Id = 21, Type = "Religion" },
                    new Category { Id = 22, Type = "Spirituality" },
                    new Category { Id = 23, Type = "Education" },
                    new Category { Id = 24, Type = "Children" },
                    new Category { Id = 25, Type = "Young Adult" },
                    new Category { Id = 26, Type = "Adventure" },
                    new Category { Id = 27, Type = "Crime" },
                    new Category { Id = 28, Type = "True Crime" },
                    new Category { Id = 29, Type = "Classic" },
                    new Category { Id = 30, Type = "Drama" },
                    new Category { Id = 31, Type = "Comedy" },
                    new Category { Id = 32, Type = "Poetry" },
                    new Category { Id = 33, Type = "Short Stories" },
                    new Category { Id = 34, Type = "Graphic Novel" },
                    new Category { Id = 35, Type = "Manga" },
                    new Category { Id = 36, Type = "Mythology" },
                    new Category { Id = 37, Type = "Fairy Tales" },
                    new Category { Id = 38, Type = "Anthology" },
                    new Category { Id = 39, Type = "Western" },
                    new Category { Id = 40, Type = "Detective" },
                    new Category { Id = 41, Type = "Historical Fiction" },
                    new Category { Id = 42, Type = "Political Fiction" },
                    new Category { Id = 43, Type = "Science Popular" },
                    new Category { Id = 44, Type = "Health" },
                    new Category { Id = 45, Type = "Parenting" },
                    new Category { Id = 46, Type = "Environmental" },
                    new Category { Id = 47, Type = "Travel" },
                    new Category { Id = 48, Type = "Cooking" }
                );

            builder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique()
               .HasFilter("[IsVerified] = 1");
        }



    }
}
