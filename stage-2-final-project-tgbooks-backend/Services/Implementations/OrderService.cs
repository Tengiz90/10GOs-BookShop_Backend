using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Orders;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using System.Linq;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IDatabaseManager _databaseManager;

        public OrderService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<ICollection<GetOrderWithDetails>> GetAllOrdersSortedbyDateFromLatestToOldest()
        {

            var orders = await _databaseManager.GetAllOrdersSortedbyDateFromLatestToOldest();
            return orders.Select(
                 or => new GetOrderWithDetails
                 {
                     OrderId = or.Id,
                     OrderDate = or.CreatedAt,
                     OrderUserEmail = or.User.Email,
                     OrderUserFullName = $"{or.User.FirstName} {or.User.LastName}",
                     OrderUserId = or.UserId,
                     Address1 = or.User.Address.Address1,
                     Address2 = or.User.Address.Address2,
                     City = or.User.Address.City,
                     PostalCode = or.User.Address.PostalCode,
                     OrderItems = or.Items.Select(
                         orIt => new GetOrderItem
                         {
                             BookId = orIt.BookId,
                             ImageURL = orIt.Book.ImageURL,
                             Language = orIt.Book.Language,
                             Quantity = orIt.Quantity,
                             Title = orIt.Book.Title,
                         }
                     ).ToList()
                 }
                ).ToList();
               
        }

        public async Task<ICollection<GetOrderWithDetails>> GetOrdersByUserId(int userId)
        {
            var orders = await _databaseManager.GetOrdersByUserId(userId);
            return orders.Select(
              or => new GetOrderWithDetails
              {
                  OrderId = or.Id,
                  OrderDate = or.CreatedAt,
                  OrderUserEmail = or.User.Email,
                  OrderUserFullName = $"{or.User.FirstName} {or.User.LastName}",
                  OrderUserId = or.UserId,
                  Address1 = or.User.Address.Address1,
                  Address2 = or.User.Address.Address2,
                  City = or.User.Address.City,
                  PostalCode = or.User.Address.PostalCode,
                  OrderItems = or.Items.Select(
                      orIt => new GetOrderItem
                      {
                          BookId = orIt.BookId,
                          ImageURL = orIt.Book.ImageURL,
                          Language = orIt.Book.Language,
                          Quantity = orIt.Quantity,
                          Title = orIt.Book.Title,
                      }
                  ).ToList()
              }
             ).ToList();
        }
    }
}
