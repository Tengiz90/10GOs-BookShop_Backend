using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Services.AdditionalModels
{
    public class EditBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> AuthorNames { get; set; } = new List<string>();
        public Language Language { get; set; }
        public bool OnSale { get; set; }
        public int OffPercentage { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
