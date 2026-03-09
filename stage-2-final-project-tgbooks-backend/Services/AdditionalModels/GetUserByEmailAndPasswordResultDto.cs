using stage_2_final_project_tgbooks_backend.Enums;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Services.AdditionalModels
{
    public class GetUserByEmailAndPasswordResultDto
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public ICollection<GetOrder> Orders { get; set; }
    }
}
