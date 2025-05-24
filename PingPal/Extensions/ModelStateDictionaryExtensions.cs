using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PingPal.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static string JoinErrors(
        this ModelStateDictionary modelState,
        string? errorsSeparator = "\n")
    {
        return string.Join(
            errorsSeparator,
            modelState.Values
                .SelectMany(entry => entry.Errors
                    .Select(error => error.ErrorMessage)));
    }
}