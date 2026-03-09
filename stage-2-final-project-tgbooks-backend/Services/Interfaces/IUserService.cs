using stage_2_final_project_tgbooks_backend.Enums;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
using stage_2_final_project_tgbooks_backend.Services.AdditionalModels;

namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<AddUserResult> AddNewUserAsync(AddUser user);
        Task<ConfirmEmailResult> ConfirmEmailAsync(ConfirmEmail confirmEmail);
        Task<GetUserByEmailAndPasswordResultDto> GetUserByEmailAndPasswordAsync(SignInUser signInUser);
        Task<PurchaseBooksResult> AddOrderAsync(PurchaseBooks order);
        Task<EditUserNameResult> EditUserNameAsync(EditUserName userName);
        string GenerateJwtToken(int userId, string userEmail, Role role, Client client);

    }
}
