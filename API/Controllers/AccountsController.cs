using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenInterface _tokenInterface;

        public AccountsController(DataContext context, ITokenInterface tokenInterface)
        {
            _context = context;
            _tokenInterface = tokenInterface;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){
            if(await UserNameExists(registerDto.Username)) return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();

            var user = new AppUser{
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                UserName = registerDto.Username.ToLower()
            };

           _context.Users.Add(user);
           await _context.SaveChangesAsync();

           return new UserDto{
                Username= user.UserName,
                Token = _tokenInterface.CreateToken(user)
            };;
        }
    
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            var user =await _context.Users.SingleOrDefaultAsync(x=>x.UserName==loginDto.Username);

            if(user==null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0;i<computedHash.Length;i++){
                if(computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto{
                Username= user.UserName,
                Token = _tokenInterface.CreateToken(user)
            };
        }

        private async Task<bool> UserNameExists(string userName){
            return await _context.Users.AnyAsync(x=>x.UserName==userName.ToLower());
        }

    }
}