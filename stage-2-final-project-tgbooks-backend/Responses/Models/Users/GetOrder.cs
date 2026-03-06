namespace stage_2_final_project_tgbooks_backend.Responses.Models.Users
{
    public class GetOrder
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public ICollection<GetOrderItem> OrderItems { get; set; }
            = new List<GetOrderItem>();
    }
}
