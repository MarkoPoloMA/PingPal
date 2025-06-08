using PingPal.Common.Extensions;
using PingPal.Database.Context;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PingPal.Service.Stores;

public class ApplicationContextRoleStore : IApplicationContextRoleStore, IAsyncDisposable
{
    private readonly ApplicationContext _context;

    public IQueryable<Role> Roles => _context.Roles;

    public ApplicationContextRoleStore(IApplicationContextFactory applicationContextFactory)
    {
        _context = applicationContextFactory.Create();
    }

    public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(role.Name)!;
    }

    public Task SetRoleNameAsync(Role role, string? roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        role.Name = roleName.EmptyIfNull();
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(role.NormalizedName)!;
    }

    public Task SetNormalizedRoleNameAsync(Role role, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        role.NormalizedName = normalizedName.EmptyIfNull();
        return Task.CompletedTask;
    }

    public async Task<Role?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Guid.Parse(roleId);
        return await _context.Roles.FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
    }

    public async Task<Role?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Roles.FirstOrDefaultAsync(role => role.NormalizedName == normalizedRoleName, cancellationToken);
    }

    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Roles.Attach(role);
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
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