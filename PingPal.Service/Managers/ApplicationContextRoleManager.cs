using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PingPal.Service.Stores;

namespace PingPal.Service.Managers;

public class ApplicationContextRoleManager : RoleManager<Role>
{
    public ApplicationContextRoleManager(
        IApplicationContextRoleStore store,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<ApplicationContextRoleManager> logger)
        : base(
            store,
            roleValidators,
            keyNormalizer,
            errors,
            logger)
    {
    }
}