namespace ListaTarefas.Models
{
    public record RegistrarUsuarioModel (string Email, string Password, string ConfirmPassword ){}

    public record LoginUsuarioModel (string Email, string Password ){}
}