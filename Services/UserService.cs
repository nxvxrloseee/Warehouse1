using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse1.Data;
using Warehouse1.Models;

namespace Warehouse1.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly SecurityService _securityService;

        public UserService(AppDbContext context, SecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task CreateUserAsync(string username, string rawPassword, string roleName)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new System.Exception("Пользователь с этим логином уже существует.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) throw new System.Exception("Роль не найдена.");

            var newUser = new User
            {
                Username = username,
                PasswordHash = _securityService.ComputeHash(rawPassword),
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _context.UserRoles.Add(new UserRole { UserId = newUser.Id, RoleId = role.Id });
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user, string? newPassword, string roleName)
        {
            var existingUser = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null) return;

            existingUser.Username = user.Username;

            if (!string.IsNullOrEmpty(newPassword))
            {
                existingUser.PasswordHash = _securityService.ComputeHash(newPassword);
            }

            var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (newRole != null)
            {
                _context.UserRoles.RemoveRange(existingUser.UserRoles);
                _context.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = newRole.Id });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
