namespace stage_2_final_project_tgbooks_backend.Requests.Models.Users
{

    public class ConfirmEmail
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
    }
}
