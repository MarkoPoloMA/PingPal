using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PingPal.Service.Managers;

public class ApplicationContextSignInManager : SignInManager<User>
{
    public ApplicationContextSignInManager(
        ApplicationContextUserManager applicationContextUserManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<ApplicationContextSignInManager> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation)
        : base(
            applicationContextUserManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            schemes,
            confirmation)
    {
    }

    public override async Task<SignInResult> PasswordSignInAsync(
        string userName,
        string password,
        bool isPersistent,
        bool lockoutOnFailure)
    {
        var user = await ((ApplicationContextUserManager) UserManager)
            .FindByNameAndLoadRolesAsync(userName);

        if (user == null)
            return SignInResult.Failed;

        return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }
}