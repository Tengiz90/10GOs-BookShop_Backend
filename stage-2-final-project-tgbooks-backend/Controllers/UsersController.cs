using FluentValidation;
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
                    Message = $"Failed to register an user: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }



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
                var response = new ApiResponse<GetUserByEmailAndPasswordResult?> { Data = userInfo, WasSuccessful = true, Message = "Login successful" };
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
                var errorResponse = new ApiResponse<EditUserNameResult?> { Data = null, WasSuccessful = false, Message = "Coukd not edit user name: " + ex.Message };
                return StatusCode(500, errorResponse);
            }
        }

    }
}
