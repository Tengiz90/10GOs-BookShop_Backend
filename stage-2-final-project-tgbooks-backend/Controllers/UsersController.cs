using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Core.Exceptions;
using stage_2_final_project_tgbooks_backend.Helpers;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;
using stage_2_final_project_tgbooks_backend.Responses;
using stage_2_final_project_tgbooks_backend.Responses.Models.Users;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<AddUser> _addUserValidator;
        private readonly IValidator<EditUserName> _editUserNameValidator;
        private readonly IValidator<ConfirmEmail> _confirmEmailValidator;
        private readonly IValidator<SignInUser> _signInUserValidator;
        public UsersController(IUserService userService,
            IValidator<AddUser> addUserValidator,
            IValidator<EditUserName> editUserNameValidator,
            IValidator<ConfirmEmail> confirmEmailValidator,
            IValidator<SignInUser> signInUserValidator)
        {
            _userService = userService;
            _addUserValidator = addUserValidator;
            _editUserNameValidator = editUserNameValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _signInUserValidator = signInUserValidator;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("buy-books")]
        public async Task<ActionResult<ApiResponse<PurchaseBooksResult?>>> PurchaseBooksByIds(PurchaseBooks purchaseBooks)
        {
            try
            {
                var orderInfo = await _userService.AddOrderAsync(purchaseBooks);
                var response = new ApiResponse<PurchaseBooksResult?> { Data = orderInfo, WasSuccessful = true, Message = "Ordering was sucessfull" };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<PurchaseBooksResult?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(notFoundResponse);
            }
            catch (NotEnoughStockException ex)
            {
                var notEnoughStockResponse = new ApiResponse<PurchaseBooksResult?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(notEnoughStockResponse);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                var notValidAmounrProvidedResponse = new ApiResponse<PurchaseBooksResult?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(notValidAmounrProvidedResponse);
            }
            catch (ArgumentException ex)
            {
                var badArgumentResponse = new ApiResponse<PurchaseBooksResult?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(badArgumentResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<PurchaseBooksResult?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to place an order: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }

        }


        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AddUserResult?>>> RegisterUser(AddUser userToAdd)
        {
            var validationResult = await _addUserValidator.ValidateAsync(userToAdd);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<AddUserResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var userInfo = await _userService.AddNewUserAsync(userToAdd);
                var response = new ApiResponse<AddUserResult?> { Data = userInfo, Message = "Registration was successful", WasSuccessful = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<AddUserResult?>
                {
                    WasSuccessful = false,
                    Message = $"Failed to register user: {ex.InnerException?.Message ?? ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }


        [Authorize]
        [HttpPost("confirm-email")]
        public async Task<ActionResult<ApiResponse<ConfirmEmailResult?>>> VerifyEmail(ConfirmEmail confirmEmail)
        {
            var validationResult = await _confirmEmailValidator.ValidateAsync(confirmEmail);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<ConfirmEmailResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var userId = await _userService.ConfirmEmailAsync(confirmEmail);
                var response = new ApiResponse<ConfirmEmailResult?> { Data = userId, WasSuccessful = true, Message = "Email confirmed" };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<ConfirmEmailResult?> { Data = null, WasSuccessful = true, Message = ex.Message };
                return NotFound(notFoundResponse);
            }
            catch (EmailConfirmationException ex)
            {
                var wrongCodeResponse = new ApiResponse<ConfirmEmailResult?> { Data = null, WasSuccessful = true, Message = ex.Message };
                return BadRequest(wrongCodeResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ConfirmEmailResult?> { Data = null, WasSuccessful = false, Message = ex.Message };
                return StatusCode(500, errorResponse);
            }

        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<ApiResponse<GetUserByEmailAndPasswordResult?>>> SignIn(SignInUser signInUser)
        {
            var validationResult = await _signInUserValidator.ValidateAsync(signInUser);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<GetUserByEmailAndPasswordResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var userInfo = await _userService.GetUserByEmailAndPasswordAsync(signInUser);
                var jwtToken = _userService.GenerateJwtToken(userInfo.Id, userInfo.Email, userInfo.Role, signInUser.client);
                var responseData = new GetUserByEmailAndPasswordResult
                {
                    DateOfBirth = userInfo.DateOfBirth,
                    Email = userInfo.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    role = userInfo.Role,
                    Id = userInfo.Id,
                    JwtToken = jwtToken,
                    Orders = userInfo.Orders,

                };
                var response = new ApiResponse<GetUserByEmailAndPasswordResult?> { Data = responseData, WasSuccessful = true, Message = "Login successful" };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var userNotFoundResponse = new ApiResponse<GetUserByEmailAndPasswordResult?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(userNotFoundResponse);
            }
            catch (AuthenticationException ex)
            {
                var wrongCredentialsResponse = new ApiResponse<GetUserByEmailAndPasswordResult?> { Data = null, WasSuccessful = false, Message = ex.Message };
                return Unauthorized(wrongCredentialsResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<GetUserByEmailAndPasswordResult?> { Data = null, Message = $"Could not login the user: {ex.Message}", WasSuccessful = false };
                return StatusCode(500, errorResponse);
            }

        }

        [Authorize]
        [HttpPut("edit-name")]
        public async Task<ActionResult<ApiResponse<EditUserNameResult?>>> EditNameOfUser(EditUserName editUserName)
        {
            var validationResult = await _editUserNameValidator.ValidateAsync(editUserName);

            if (!validationResult.IsValid)
            {
                var response = ValidationHelper.CreateValidationFailedResponse<EditUserNameResult?>(validationResult.Errors);
                return BadRequest(response);
            }

            try
            {
                var userEditiedInfo = await _userService.EditUserNameAsync(editUserName);
                var response = new ApiResponse<EditUserNameResult?> { Data = userEditiedInfo, Message = "Name changed successfully", WasSuccessful = true };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var userNotFoundResponse = new ApiResponse<EditUserNameResult?> { Data = null, WasSuccessful = false, Message = ex.Message };
                return NotFound(userNotFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<EditUserNameResult?> { Data = null, WasSuccessful = false, Message = "Could not edit user name: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }

        [Authorize]
        [HttpPut("edit-billing-adress")]
        public async Task<ActionResult<ApiResponse>> EditBillingAddressOfUser(UpdateBillingAddress requestInfo)
        {
            try
            {
                await _userService.UpdateBillingAddressByUserIdAsync(requestInfo);
                var response = new ApiResponse { Message = "Billing address updated successfully", WasSuccessful = true };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse { WasSuccessful = false, Message = ex.Message };
                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse {  WasSuccessful = false, Message = "Could not update billing address: " + ex.Message };
                return StatusCode(500, errorResponse);
            }

        }


        [HttpGet("cart")]
        public async Task<ActionResult<ApiResponse<ICollection<GetCartItem>?>>> GetAllCartItems(int userId)
        {
            try
            {
                var cart = (await _userService.GetUserCartByUserIdAsync(userId)).ToList();
                var response = new ApiResponse<ICollection<GetCartItem>?> { WasSuccessful = true, Message = "Cart retrieved successfully", Data = cart };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<ICollection<GetCartItem>?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ICollection<GetCartItem>?> { Data = null, WasSuccessful = false, Message = "Could not get cart: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("cart")]
        public async Task<ActionResult<GetCartItem?>> AddToCart(AddCartItem addCartItem)
        {
            try
            {
                var cartItem = await _userService.AddItemToCartAsync(addCartItem);
                var response = new ApiResponse<GetCartItem?> { WasSuccessful = true, Data = cartItem, Message = "Item added succesfully" };
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(notFoundResponse);
            }
            catch (NotEnoughStockException ex)
            {
                var notEnoughStockResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(notEnoughStockResponse);
            }
            catch (AlreadyInCartException ex)
            {
                var alreadyInCartExceptionResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(alreadyInCartExceptionResponse);
            }
            catch (CartIsFullException ex)
            {
                var cartIsFullExceptionResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(cartIsFullExceptionResponse);
            } catch (Exception ex)
            {
                var errorResponse = new ApiResponse<GetCartItem?> { Data = null, WasSuccessful = false, Message = "Could not add item to cart: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("cart")]
        public async Task<ActionResult<ApiResponse<int?>>> RemoveCartItem(RemoveCartItem removeCartItem)
        {
            try
            {
                var removedCartItemId = await _userService.RemoveItemFromCartAsync(removeCartItem.CartItemId, removeCartItem.UserId);
                var response = new ApiResponse<int?> { WasSuccessful = true, Message = "Item was removed successfully"};
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<int?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(notFoundResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<int?> { Data = null, WasSuccessful = false, Message = "Could not remove cart item: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }
        [HttpPut("cart")]
        public async Task<ActionResult<GetCartItem?>> ChangeCartItemAmount(ChangeCartItemQuantity changeCartItemQuantity)
        {
            try
            {
                var updatedCartItem = await _userService.ChangeCartItemQuantityAsync(changeCartItemQuantity.CartItemId, changeCartItemQuantity.Quantity, changeCartItemQuantity.UserId);
                var reponse = new ApiResponse<GetCartItem?> { Data = updatedCartItem, Message = "Cart item updated successfully", WasSuccessful = true };
                return Ok(reponse);
            }
            catch (EntityNotFoundException ex)
            {
                var notFoundResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return NotFound(notFoundResponse);
            }
            catch (NotEnoughStockException ex)
            {
                var notEnoughStockResponse = new ApiResponse<GetCartItem?> { Data = null, Message = ex.Message, WasSuccessful = false };
                return BadRequest(notEnoughStockResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<GetCartItem?> { Data = null, WasSuccessful = false, Message = "Could not remove cart item: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }
    }
}
