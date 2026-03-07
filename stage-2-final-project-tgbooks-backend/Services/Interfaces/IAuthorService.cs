using stage_2_final_project_tgbooks_backend.Data.Models;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;

namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<ICollection<GetAuthor>> GetAuthorsAsync();
    }
}
