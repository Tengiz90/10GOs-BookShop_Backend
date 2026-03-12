using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Address : BaseEntity
    {
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public string Country { get; } = "Georgia";

        // Foreign key
        public int UserId { get; set; }

    }
}
