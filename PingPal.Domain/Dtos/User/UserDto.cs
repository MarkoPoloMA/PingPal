using System.ComponentModel.DataAnnotations;

namespace PingPal.Domain.Dtos.User;
public class UserDto
{
	public Guid Id { get; set; }

	[StringLength(50, ErrorMessage = "Никнейм слишком длинный")]
	[Required(ErrorMessage = "Такой никнейм существует")]
    public string Name { get; set; }
}

