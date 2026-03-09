using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Users
{
    public class SignInUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Client client { get; set; }
    }
}
