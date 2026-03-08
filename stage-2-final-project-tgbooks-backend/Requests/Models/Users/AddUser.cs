using stage_2_final_project_tgbooks_backend.Data.Models;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Users
{
    public class AddUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Password { get; set; }

        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public string Country { get; } = "Georgia";

    }
}
