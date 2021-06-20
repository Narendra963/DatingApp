using API.Entities;

namespace API.Interfaces
{
    public interface ITokenInterface
    {
        public string CreateToken(AppUser appUser);
    }
}