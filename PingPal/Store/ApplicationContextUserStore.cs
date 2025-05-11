using PingPal.Database.Context.Factory;
using PingPal.Database.Context;
using PingPal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Resources;
using UserRole = PingPal.Domain.Entities.UserRole;
using PingPal.Common.Extensions;

namespace AspNetExample.Store
{
    public class ApplicationContextUserStore : IUserStore<User>, IQueryableUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        public readonly IRoleStore<Role> _role;
        public readonly ApplicationContext _context;

        public ApplicationContextUserStore(IRoleStore<Role> role , IApplicationContextFactory applicationContextFactory)
        {
            _role = role;
            _context = applicationContextFactory.Create();
        }

        public IQueryable<User> Users => throw new NotImplementedException();

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

            var roleEntity = await _role.FindByNameAsync(roleName, cancellationToken);
            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, roleName));
            }
            _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleEntity.Id });
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Users.Add(user);
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

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var id = Guid.Parse(userId);

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedName == normalizedUserName);
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedName);
        }

        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            return await _context.UserRoles
                 .Where(userRole => userRole.UserId == user.Id)
                 .Select(userRole => userRole.Role.Name)
                 .ToListAsync(cancellationToken);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Name);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            ArgumentException.ThrowIfNullOrEmpty(roleName);

            var role = await _role.FindByNameAsync(roleName, cancellationToken);

            if (role != null)
            {
                var query = from userrole in _context.UserRoles
                            join user in Users on userrole.UserId equals user.Id
                            where userrole.RoleId.Equals(role.Id)
                            select user;

                return await query.ToListAsync(cancellationToken);
            }
            return new List<User>();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash.IsSignificant());
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

            var role = await _role.FindByNameAsync(roleName, cancellationToken);
            if (role != null)
            {
                var userRole = await _context.UserRoles.FindAsync(user.Id, role.Id, cancellationToken);
                return userRole != null;
            }
            return false;
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

            var roleEntity = await _role.FindByIdAsync(roleName, cancellationToken);
            if (roleEntity != null)
            {
                var userRole = await _context.UserRoles.FindAsync(user.Id, roleEntity.Id, cancellationToken);
                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                }
            }
        }

        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedName = normalizedName ?? string.Empty;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash ?? string.Empty;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Name = userName ?? string.Empty;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Attach(user);
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }
    }
}
