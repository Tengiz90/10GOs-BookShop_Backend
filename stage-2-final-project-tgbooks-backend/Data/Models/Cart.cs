using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Cart : BaseEntity
    {
        // 1 -> many
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        // FK (1 → 1)
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
