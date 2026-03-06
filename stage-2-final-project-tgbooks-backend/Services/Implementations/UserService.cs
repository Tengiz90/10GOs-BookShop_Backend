using stage_2_final_project_tgbooks_backend.Data.Implementations;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using System.Net.Security;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IEmailSender _emailSender;

        public UserService(IDatabaseManager databaseManager, IEmailSender emailSender)
        {
            _databaseManager = databaseManager;
            _emailSender = emailSender;
        }
        public async Task<AddUserResult> AddNewUserAsync(AddUser user)
        {
            var emailVerificationCode = GenereateRandom4DigitCode();
            var userToAdd = new User {
                Email = CapitalizeFirstLetter(user.Email),
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                EmailVerificationCode = emailVerificationCode,
                Address = new Address
                {
                    Address1 = user.Address1,
                    Address2 = user.Address2,
                    City = user.City,
                    PostalCode = user.PostalCode,
                }
            };
           var userId = await _databaseManager.AddUserAsync(userToAdd);
           SendCodeToEmail(user.Email, emailVerificationCode);
            return new AddUserResult
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = CapitalizeFirstLetter(user.Email),
            };
        }

        public async Task<ConfirmEmailResult> ConfirmEmailAsync(ConfirmEmail confirmEmail)
        {
            return new ConfirmEmailResult
            {
                UserId = await _databaseManager.ConfirmUserRegistrationAsync(CapitalizeFirstLetter(confirmEmail.Email), confirmEmail.Code)
            };

        }

        public async Task<GetUserByEmailAndPasswordResult> GetUserByEmailAndPasswordAsync(SignInUser signInUser)
        {
            var user = await _databaseManager.GetUserByEmailAndPasswordAsync(CapitalizeFirstLetter(signInUser.Email), signInUser.Password);
            return new GetUserByEmailAndPasswordResult
            {
                Id = user.Id,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Orders = user.Orders.Select(o => new GetOrder
                {
                    Id = o.Id,
                    OrderDate = o.CreatedAt,

                    OrderItems = o.Items.Select(i => new GetOrderItem
                    {
                        BookId = i.BookId,
                        Quantity = i.Quantity,
                        ImageURL = i.Book.ImageURL,
                        Language = i.Book.Language,
                        Title = i.Book.Title,
                    }).ToList()

                }).ToList()
            }; 
        }

        public async Task<PurchaseBooksResponse> AddOrderAsync(PurchaseBooks purchasebooks)
        {
            var order = await _databaseManager.PurchaseBooksByIdsAsync(purchasebooks.BookIds, purchasebooks.QuantitiesToPurchaseEach, purchasebooks.UserId);
            return new PurchaseBooksResponse
            {
                Id = order.Id,
                OrderDate = order.CreatedAt,
                OrderItems = order.Items.Select(i => new GetOrderItem
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    ImageURL = i.Book.ImageURL,
                    Language = i.Book.Language,
                    Title = i.Book.Title,
                }).ToList()
            };
        }

        public async Task<EditUserNameResult> EditUserNameAsync(EditUserName userName)
        {
           var nameResult = await _databaseManager.EditUserFullNameByIdAsync(
               userName.FirstName, userName.LastName, userName.Id);
            return new EditUserNameResult { FirstName = nameResult.FirstName, LastName =nameResult.LastName, UserId = nameResult.Id };
        }

        private string GenereateRandom4DigitCode()
        {
            Random rand = new Random();
            int code = rand.Next(1000, 9999);
            return code.ToString();
        }

        private void SendCodeToEmail(string email, string code)
        {
            _emailSender.Send(email, "Activation Code", $"Your code is {code}");

        }

        string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
