using Microsoft.IdentityModel.Tokens;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Enums;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
using stage_2_final_project_tgbooks_backend.Services.AdditionalModels;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            var email = CapitalizeFirstLetter(user.Email);
            var emailVerificationCode = await GenerateRandom4DigitCode(email);
            var userToAdd = new User {
                Email = email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Role = Role.Customer,
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

        public async Task<GetUserByEmailAndPasswordResultDto> GetUserByEmailAndPasswordAsync(SignInUser signInUser)
        {
            var user = await _databaseManager.GetUserByEmailAndPasswordAsync(CapitalizeFirstLetter(signInUser.Email), signInUser.Password);
            return new GetUserByEmailAndPasswordResultDto
            {
                Id = user.Id,
                Role = user.Role,
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

        public async Task<PurchaseBooksResult> AddOrderAsync(PurchaseBooks purchasebooks)
        {
            var order = await _databaseManager.PurchaseBooksByIdsAsync(purchasebooks.BookIds, purchasebooks.QuantitiesToPurchaseEach, purchasebooks.UserId);
            return new PurchaseBooksResult
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

        public async Task<bool> IsVerificationCodeUniqueForEmailAsync(string code, string email)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(code));

            // Check if any unverified user already has this code
            bool exists = await _databaseManager.IsVerificationCodeUniqueForEmailAsync(code, email);

            return exists; // True = unique, False = already used
        }

        private async Task<string> GenerateRandom4DigitCode(string email)
        {
            string code;

            do
            {
                //.NET 6 introduced Random.Shared, which is:
                //Thread - safe
                //Fast
                //One shared instance
                //No need to manage it yourself
                code = Random.Shared.Next(1000, 10000).ToString();
            }
            while (!await IsVerificationCodeUniqueForEmailAsync(code, email));

            return code;
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

        public string GenerateJwtToken(int userId, string userEmail, Role role, Client client)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JwtSecret");
            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                throw new InvalidOperationException("JWT secret key is not set in environment variables.");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            string roleAsString = role.ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Role, roleAsString)
            };

            string audience = client switch
            {
                Client.Android => "10GO-Android-App",
                Client.iOS => "10GO-iOS-App",
                _ => throw new ArgumentException("Invalid client")
            };

            var token = new JwtSecurityToken(
                issuer: "10GO's books backend",
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      
    }
}
