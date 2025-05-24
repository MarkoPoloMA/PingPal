using System.ComponentModel;

namespace PingPal.Models.Account;
public class LoginModel
{
    public Guid Id { get; set; }
    [DisplayName("Введите логин:")]
    public string Login { get; set; }
    [DisplayName("Введите пароль:")]
    public string Password { get; set; }
	public string? ReturnUrl { get; set; }
}

