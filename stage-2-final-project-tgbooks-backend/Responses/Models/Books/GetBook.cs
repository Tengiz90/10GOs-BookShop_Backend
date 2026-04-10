using stage_2_final_project_tgbooks_backend.Enums;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;

namespace stage_2_final_project_tgbooks_backend.Responses.Models.Books
{
    public class GetBook 
    {
        public bool AlreadyInCart { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<GetAuthor> Authors { get; set; }
        public Language Language { get; set; }
        public bool OnSale { get; set; }
        public int OffPercentage { get; set; }
        public int Quantity { get; set; }

        public string ImageURL { get; set; }
    }
}
