namespace stage_2_final_project_tgbooks_backend.Responses.Models.Users
{
    public class GetUserBillingInfoResult
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public string Country { get; } = "Georgia";

    }
}
