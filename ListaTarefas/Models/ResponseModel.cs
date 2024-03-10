namespace ListaTarefas.Models
{
    public class ResponseModel
    {
        public ResponseModel(object result = null)
        {
            
        }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}