using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;

namespace ListaTarefas.Models
{
    public record class UsuarioModel (string Email, string Password ){}
}