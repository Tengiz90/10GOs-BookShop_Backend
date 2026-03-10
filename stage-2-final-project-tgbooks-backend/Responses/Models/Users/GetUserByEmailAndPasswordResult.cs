namespace stage_2_final_project_tgbooks_backend.Responses.Models.Users
{
    public class GetUserByEmailAndPasswordResult
    {
        public int Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public ICollection<GetUserOrder> Orders {  get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
