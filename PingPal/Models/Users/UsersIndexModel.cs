#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel;

namespace PingPal.Models.Users;

public class UsersIndexModel : SortingPaginationModelBase
{
    [DisplayName("Имя или id")]
    public string? SearchString { get; set; }

    public UserModel[] Users { get; set; }
}