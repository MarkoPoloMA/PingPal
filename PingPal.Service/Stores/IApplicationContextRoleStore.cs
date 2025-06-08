using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace PingPal.Service.Stores;

public interface IApplicationContextRoleStore : IQueryableRoleStore<Role>
{
}