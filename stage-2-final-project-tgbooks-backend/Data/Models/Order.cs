using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Order : BaseEntity
    {
        // 1 -> many
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        // FK (many → 1)
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
