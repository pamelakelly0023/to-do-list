using FluentValidation;
using ListaTarefas.Extensions;

namespace ListaTarefas.Models
{
    
    public sealed class UsuarioModelValidator : AbstractValidator<UsuarioModel>
    {
        public UsuarioModelValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email é um campo obrigatório");
                    
            RuleFor(p => p.Password).NotEmpty().WithMessage("Senha é um campo requerido")
                .MinimumLength(8).WithMessage("O tamanho da sua senha deve conter pelo menos 8 caracteres")
                .MaximumLength(16).WithMessage("O tamanho da sua senha não deve exceder 16 caracteres");
                    
        }
    }
}