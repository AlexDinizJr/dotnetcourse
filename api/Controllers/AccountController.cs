using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using api.Models;
using api.DTOs.Account;
using api.Interfaces;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController ]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid email!");

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Email not found and/or password incorrect");

            return Ok(
                new NewUserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    Name = registerDTO.Name,
                    UserName = registerDTO.Username,
                    Email = registerDTO.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDTO.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDTO
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Name = appUser.Name,
                            Token = _tokenService.CreateToken(appUser)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}