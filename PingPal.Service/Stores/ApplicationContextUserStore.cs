using PingPal.Common.Extensions;
using PingPal.Database.Context;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PingPal.Service.Stores;

public class ApplicationContextUserStore : IApplicationContextUserStore, IAsyncDisposable
{
    private readonly ApplicationContext _context;
    private readonly IApplicationContextRoleStore _applicationContextRoleStore;

    public IQueryable<User> Users => _context.Users;

    public ApplicationContextUserStore(
        IApplicationContextFactory applicationContextFactory,
        IApplicationContextRoleStore applicationContextRoleStore)
    {
        _context = applicationContextFactory.Create();
        _applicationContextRoleStore = applicationContextRoleStore;
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.Name)!;
    }

    public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        user.Name = userName.EmptyIfNull();
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.NormalizedName)!;
    }

    public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        user.NormalizedName = normalizedName.EmptyIfNull();
        return Task.CompletedTask;
    }

    public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Guid.Parse(userId);
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> FindByIdAndLoadRolesAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Guid.Parse(userId);
        return await _context.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Users.FirstOrDefaultAsync(user => user.NormalizedName == normalizedUserName, cancellationToken);
    }

    public async Task<User?> FindByNameAndLoadRolesAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.NormalizedName == normalizedUserName, cancellationToken);
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Users.Attach(user);
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        user.PasswordHash = passwordHash.EmptyIfNull();
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.PasswordHash.NullIfEmpty());
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.PasswordHash.IsSignificant());
    }

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await _applicationContextRoleStore.FindByNameAsync(roleName, cancellationToken);
        if (role == null)
            throw new InvalidOperationException($"Role with name {roleName} not found");

        _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await _applicationContextRoleStore.FindByNameAsync(roleName, cancellationToken);
        if (role == null)
            throw new InvalidOperationException($"Role with name {roleName} not found");

        await _context.UserRoles
            .Where(userRole => userRole.UserId == user.Id && userRole.RoleId == role.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var isUserRolesLoaded = user.UserRoles != null &&
            (!user.UserRoles.Any() || user.UserRoles.First().Role != null);
        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

        return isUserRolesLoaded
            ? user.UserRoles!
                .Select(userRole => userRole.Role.Name)
                .ToList()
            : await _context.UserRoles
                .Where(userRole => userRole.UserId == user.Id)
                .Select(userRole => userRole.Role.Name)
                .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var isUserRolesLoaded = user.UserRoles != null &&
            (!user.UserRoles.Any() || user.UserRoles.First().Role != null);
        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

        return isUserRolesLoaded
            ? user.UserRoles!
                .Any(userRole => userRole.Role.NormalizedName == roleName)
            : await _context.UserRoles
                .AnyAsync(userRole => userRole.UserId == user.Id && userRole.Role.NormalizedName == roleName, cancellationToken);
    }

    public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Users
            .Where(user => user.UserRoles.Any(userRole => userRole.Role.NormalizedName == roleName))
            .ToListAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}