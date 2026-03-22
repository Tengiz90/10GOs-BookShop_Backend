using Microsoft.OpenApi;
using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class CartItem : BaseEntity
    {

        // many -> 1
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
