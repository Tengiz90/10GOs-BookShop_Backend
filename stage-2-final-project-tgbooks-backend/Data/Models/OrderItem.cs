using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class OrderItem : BaseEntity
    {
        // many -> 1
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
