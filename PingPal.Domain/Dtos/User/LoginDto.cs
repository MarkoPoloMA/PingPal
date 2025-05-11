using System.ComponentModel.DataAnnotations;

namespace PingPal.Domain.Dtos.User;
public class LoginDto
{
	public Guid Id { get; set; }

	[Required(ErrorMessage = "Такой логин уже существует.")]
	[StringLength(100, ErrorMessage = "Логин слишком длинный")]
    public string Login { get; set; }

	[StringLength(256, ErrorMessage = "Пароль слишком длинный")] 
	public string Password { get; set; }
}

