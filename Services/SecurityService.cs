using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse1.Data;
using Warehouse1.Models;
using Microsoft.EntityFrameworkCore;

namespace Warehouse1.Services
{
    public class SecurityService
    {
        private readonly AppDbContext _context;

        public SecurityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var hash = ComputeHash(password);

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role) // Важно! Роль здесь называется "Role", а не "UserRole"
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash && u.IsActive);

            if (user != null)
            {
                // Успешный вход
                return new LoginResult { IsSuccess = true, User = user, ErrorMessage = null! };
            }
            else
            {
                // Неудачный вход
                return new LoginResult { IsSuccess = false, User = null, ErrorMessage = "Неверное имя пользователя или пароль." };
            }
        }

        public bool IsAdmin(User user) => user.UserRoles.Any(r => r.Role.Name == "Admin");
        public bool IsManager(User user) => user.UserRoles.Any(r => r.Role.Name == "Manager");

        public string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
