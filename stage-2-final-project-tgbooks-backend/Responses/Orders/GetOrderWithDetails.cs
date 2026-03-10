using stage_2_final_project_tgbooks_backend.Responses.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Responses.Orders
{
    public class GetOrderWithDetails
    {

        public int OrderId { get; set; }
        public int OrderUserId { get; set; }
        public string OrderUserFullName { get; set; }
        public string OrderUserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; } = "Georgia";

        public ICollection<GetOrderItem> OrderItems { get; set; }

    }
}
