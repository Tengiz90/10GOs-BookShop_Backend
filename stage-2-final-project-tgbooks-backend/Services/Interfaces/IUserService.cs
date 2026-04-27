using stage_2_final_project_tgbooks_backend.Data.Models;
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
        Task<GetUserBillingInfoResult> GetUserBillingInfoByIdAsync(int id);
        Task<PurchaseBooksResult> AddOrderAsync(PurchaseBooksDto order);
        Task<EditUserNameResult> EditUserNameAsync(EditUserNameDto userName);
        Task UpdateBillingAddressByUserIdAsync(UpdateBillingAddressDto updatedAddress);

        Task<ICollection<GetCartItem>> GetUserCartByUserIdAsync(int userId);
        Task<GetCartItem> AddItemToCartAsync(AddCartItemDto addCartItem);
        Task<int> RemoveItemFromCartAsync(int cartItemId, int userId);
        Task<GetCartItem> ChangeCartItemQuantityAsync(int cartItemId, int quantity, int userId);

        string GenerateJwtToken(int userId, string userEmail, Role role, Client client);


    }
}
