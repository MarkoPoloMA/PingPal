#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPal.Models.Users;

public class UserModel
{
    [DisplayName("Id")]
    public Guid Id { get; set; }

    [DisplayName("Логин")]
    public string Login { get; set; }

    [DisplayName("Новый пароль")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля 6 символов")]
    public string? NewPassword { get; set; }

    [DisplayName("Администратор")]
    public bool HasAdminRole { get; set; }

    [DisplayName("Swagger")]
    public bool HasSwaggerRole { get; set; }
}