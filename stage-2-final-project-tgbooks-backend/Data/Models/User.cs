using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PasswordHash { get; set; }

        public bool IsVerified { get; set; } = false;
        public string? EmailVerificationCode { get; set; } = null;
        // 1 → many
        public ICollection<Order> Orders { get; set; } = new List<Order>();


        // 1 → 1
        public Address Address { get; set; } = null!;
    }
}
