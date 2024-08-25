using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUser();
        Task<AppUser> GetUserById(string id);

        bool Add(AppUser user);
        bool Delete(AppUser user);
        bool Update(AppUser user);
        bool Save();
    }
}
