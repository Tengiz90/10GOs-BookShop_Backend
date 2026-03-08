using FluentValidation.Results;
using stage_2_final_project_tgbooks_backend.Responses;

namespace stage_2_final_project_tgbooks_backend.Helpers
{
    public static class ValidationHelper
    {
        public static ApiResponse<T?> CreateValidationFailedResponse<T>(IList<ValidationFailure> errors)
        {
            var messages = errors.Select(e => e.ErrorMessage);
            return new ApiResponse<T?>
            {
                WasSuccessful = false,
                Message = string.Join($";{Environment.NewLine}", messages),
                Data = default
            };
        }
    }
}
