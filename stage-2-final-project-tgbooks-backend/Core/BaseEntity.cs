namespace stage_2_final_project_tgbooks_backend.Core
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
