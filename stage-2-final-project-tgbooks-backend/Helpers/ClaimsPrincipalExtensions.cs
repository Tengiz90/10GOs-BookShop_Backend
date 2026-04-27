using System.Security.Claims;

namespace stage_2_final_project_tgbooks_backend.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("nameid")?.Value;
            return int.Parse(idClaim);
        }
    }
}
