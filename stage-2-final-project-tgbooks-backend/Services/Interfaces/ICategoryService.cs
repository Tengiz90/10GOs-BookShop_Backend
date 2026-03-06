using stage_2_final_project_tgbooks_backend.Responses.Models.Categories;

namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface ICategoryService
    {
        ICollection<GetCategory> GetAllCategoriesAsync();
    }
}
