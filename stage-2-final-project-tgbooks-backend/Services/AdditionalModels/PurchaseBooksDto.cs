namespace stage_2_final_project_tgbooks_backend.Services.AdditionalModels
{
    public class PurchaseBooksDto
    {
        public int UserId { get; set; }
        public ICollection<int> BookIds { get; set; } = new List<int>();
        public ICollection<int> QuantitiesToPurchaseEach { get; set; } = new List<int>();
    }
}
