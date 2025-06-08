#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPal.Models.Account;

public class RegisterModel
{
    [DisplayName("Логин")]
    [Required(ErrorMessage = "Не указан логин")]
    public string Login { get; set; }

    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Не указан пароль")]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля 6 символов")]
    public string Password { get; set; }

    [DisplayName("Подтверждение пароля")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string? ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }
}