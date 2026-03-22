using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Responses.Models.Users
{
    public class GetCartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
    }
}
