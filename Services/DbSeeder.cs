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
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly SecurityService _securityService;

        public DbSeeder(AppDbContext context, SecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
        }

        public async Task SeedAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Admin");
                var adminUser = new User
                {
                    Username = "admin",
                    FullName = "System Administrator",
                    IsActive = true,
                    // Используем ваш сервис для генерации правильного хеша
                    PasswordHash = _securityService.ComputeHash("admin"),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync(); // Сначала сохраняем юзера, чтобы получить ID

                // Связь пользователя и роли
                _context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }

            // 3. Создаем Менеджера (пароль: manager)
            if (!await _context.Users.AnyAsync(u => u.Username == "manager"))
            {
                var managerRole = await _context.Roles.FirstAsync(r => r.Name == "Manager");
                var managerUser = new User
                {
                    Username = "manager",
                    FullName = "Default Manager",
                    IsActive = true,
                    PasswordHash = _securityService.ComputeHash("manager"),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(managerUser);
                await _context.SaveChangesAsync();

                _context.UserRoles.Add(new UserRole
                {
                    UserId = managerUser.Id,
                    RoleId = managerRole.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
