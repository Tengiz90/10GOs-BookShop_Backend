namespace stage_2_final_project_tgbooks_backend.Responses
{
    public class ApiResponse
    {
        public bool WasSuccessful { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
}
