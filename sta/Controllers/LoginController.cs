﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using sta.Models;

namespace sta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(); // Utilizador não encontrado
            }

            return user;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Verifica as credenciais do utilizador
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                if (user.Password == password)
                {
                    // Crie uma identidade para o utilizador autenticado
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Type == 0 ? "Admin" : "User"), // Use o campo "Type" para definir a função/role do utilizador
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Faz a autenticação do utilizador
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = false
                        });

                    return Ok(new { Id = user.Id, Type = user.Type }); // Retorne o ID do utilizador na resposta
                }
                else
                {
                    // Password incorreta
                    return Unauthorized("Senha incorreta");
                }
            }

            // Utilizador não encontrado
            return Unauthorized("Utilizador não encontrado");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Efetua o logout do utilizador
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string password, [FromQuery] string email)
        {
            if (UserExists(username))
            {
                return BadRequest("Utilizador já existente.");
            }

            if (EmailExists(email))
            {
                return BadRequest("Email já existente.");
            }

            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                Type = 1 // Definir o valor padrão para 1 (utilizador normal)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Retorna o ID do novo utilizador na resposta
            return Ok(new { Id = user.Id });
        }






        [HttpGet("getusername")]
        [Authorize]
        public IActionResult GetUsername()
        {
            var username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            return Ok(new { username });
        }


        [HttpGet("{id}/Type")]
        public async Task<IActionResult> GetUserType(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Type);
        }


        [HttpPost("ChangeUserType/{id}")]
        public async Task<IActionResult> ChangeUserType(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(); // Utilizador não encontrado
            }

            // Alterar o valor do campo 'type'
            user.Type = user.Type == 0 ? 1 : 0;

            await _context.SaveChangesAsync();

            return Ok(); // Sucesso na alteração do tipo do utilizador
        }





        [HttpPost("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(); // Utilizador não encontrado
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(); // Sucesso na exclusão do utilizador
        }




        private bool UserExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        private bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

    }
}
