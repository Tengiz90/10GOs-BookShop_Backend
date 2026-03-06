namespace stage_2_final_project_tgbooks_backend.Requests.Models.Users
{
    public class PurchaseBooks
    {
        public int UserId { get; set; }
        public ICollection<int> BookIds { get; set; } = new List<int>();
        public ICollection<int> QuantitiesToPurchaseEach { get; set; } = new List<int>();
    }
}
