using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPal.Models.Account;
public class RegisterModel
{
    public Guid Id { get; set; }

    [DisplayName("Введите логин:")]
    [Required(ErrorMessage = "Неверный логин")]
    public string Login { get; set; }

    [DisplayName("Введите пароль:")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Неверный пароль")]
    [MinLength(5, ErrorMessage = "Минимальный пароль 5 символов")]
    public string Password { get; set; }
    public string Name { get; set; }

    [DisplayName("Подтверждение пароля")]
    [Required(ErrorMessage = "Подтверждение обязательно.")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string? ConfirmPassword { get; set; }
}

