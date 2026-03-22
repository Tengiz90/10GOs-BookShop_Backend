namespace stage_2_final_project_tgbooks_backend.Requests.Models.Users
{
    public class ChangeCartItemQuantity
    {
        public int CartItemId {  get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
}
