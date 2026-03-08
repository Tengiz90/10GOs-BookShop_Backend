using Microsoft.Data.SqlClient.DataClassification;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Books
{
    public class RemoveBook
    {
        public int Id { get; set; }
        public string imageUrl { get; set; }
    }
}
