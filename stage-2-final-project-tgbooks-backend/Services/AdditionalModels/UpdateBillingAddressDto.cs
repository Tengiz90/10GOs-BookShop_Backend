namespace stage_2_final_project_tgbooks_backend.Services.AdditionalModels
{
    public class UpdateBillingAddressDto
    {
        public int UserId { get; set; }
        public string Address1 { get; set; } = string.Empty;

        public string Address2 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;
    }
}
