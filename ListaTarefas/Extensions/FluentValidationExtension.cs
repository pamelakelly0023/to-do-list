using System.Globalization;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using ListaTarefas.Models;
using Microsoft.Extensions.DependencyInjection; 

namespace ListaTarefas.Extensions;
public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, Type assemblyContainingValidators)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddFluentValidation(typeof(RegistrarUsuarioModel));

        return services;
    }

    public static List<MessageResult>? GetErrors(this ValidationResult result)
    {
        return result.Errors?.Select(error => new MessageResult(error.PropertyName, error.ErrorMessage)).ToList();
    }
}

public class MessageResult
{
    public string Code { get; set; }
    public string Description { get; set; }

    public MessageResult(string code, string description)
    {
        Code = code;
        Description = description;
    }
}