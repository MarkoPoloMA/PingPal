using System.ComponentModel;

namespace PingPal.Models.Users;

public class UsersIndexModel
{
    [DisplayName("Имя или id")]
    public string? SearchString { get; set; }

    public UserModel[] Users { get; set; }
}