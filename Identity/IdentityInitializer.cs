using BlankAPI.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace BlankAPI.Identity
{
    public class IdentityInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public IdentityInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                if (!_roleManager.RoleExistsAsync(Roles.ROLE_API_LOGIN).Result)
                {
                    var resultado = _roleManager.CreateAsync(new IdentityRole(Roles.ROLE_API_LOGIN)).Result;
                    if (!resultado.Succeeded)
                    {
                        throw new Exception($"Erro durante a criação da role {Roles.ROLE_API_LOGIN}.");
                    }
                }

                CreateUser(
                    new ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "admin@exemplo.com.br",
                        EmailConfirmed = true
                    }, "Abc_12345", Roles.ROLE_API_LOGIN);

                CreateUser(
                    new ApplicationUser()
                    {
                        UserName = "zemane",
                        Email = "carasemacesso@exemplo.com.br",
                        EmailConfirmed = true
                    }, "Abc_12345");
            }
        }

        private void CreateUser( ApplicationUser user, string password, string initialRole = null)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager.CreateAsync(user, password).Result;

                if(!resultado.Succeeded)
                {
                    throw new Exception("Ocorreu um erro ao criar usuários iniciais. \n" + resultado.ToString());
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(initialRole))
                        _userManager.AddToRoleAsync(user, initialRole).Wait();
                }
            }
        }
    }
}
