using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Interfaces.IReposiotry;

namespace RunGroupWebApp.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IClubRepository Clubs { get; private set; }
        public IAppUserClubRepository AppUserClubs { get; private set; }
        public IDashboardRepository Dashboard { get; private set; }
        public IUserRepository Users { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            Clubs = new ClubRepository(_context);
            AppUserClubs = new AppUserClubRepository(_context, _httpContextAccessor);
            Dashboard = new DashboardRepository(_context, _httpContextAccessor);
            Users = new UserRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
