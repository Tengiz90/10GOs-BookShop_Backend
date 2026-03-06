using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Responses.Models.Books
{
    public class GetBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Language Language { get; set; }
        public int Quantity { get; set; }

        public string ImageURL { get; set; }
    }
}
