using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPal.Models.Users;
public class UserModel
{
	[DisplayName("Id")]
	public Guid Id { get; set; }

	[DisplayName("Логин")]
	public string Name { get; set; }

	public string CurrentPassword { get; set; }
    [DisplayName("Новый пароль")]
	[DataType(DataType.Password)]
	[MinLength(8, ErrorMessage = "Минимальная длина пароля 8 символов")] 
	public string? NewPassword { get; set; }
	[DisplayName("Администратор")]
	public bool HasAdminRole { get; set; }
}

