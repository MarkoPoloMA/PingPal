#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPal.Models.Account;

public class LoginModel
{
    [DisplayName("Логин")]
    [Required(ErrorMessage = "Не указан логин")]
    public string Login { get; set; }

    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Не указан пароль")]
    public string Password { get; set; }

    public string? ReturnUrl { get; set; }
}