using stage_2_final_project_tgbooks_backend.Enums;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Responses.Models.Categories;

namespace stage_2_final_project_tgbooks_backend.Responses.Models.Books
{
    public class GetBookWithCategories
    {
        public bool AlreadyInCart { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<GetAuthor> Authors { get; set; }
        public ICollection<GetCategory> Categories { get; set; }
        public Language Language { get; set; }
        public bool OnSale { get; set; }
        public int OffPercentage { get; set; }
        public int Quantity { get; set; }

        public string ImageURL { get; set; }
    }
}
