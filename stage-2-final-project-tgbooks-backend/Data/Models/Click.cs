namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Click
    {
        // 1. Primary Key
        // EF Core automatically recognizes 'Id' as the auto-incrementing identity column
        public int Id { get; set; }

        // 2. Foreign Key
        // Naming this 'BookId' tells EF Core this column links directly to the Book table's Id
        public int BookId { get; set; }

        // 3. Navigation Property
        // Allows us to access the specific Book object this click belongs to in C#
        public Book Book { get; set; } = null!;

        // 4. Timestamp
        // Records the exact date and time the book was clicked
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    }
}
