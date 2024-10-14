namespace RunGroupWebApp.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IClubRepository Clubs { get; }
        IAppUserClubRepository AppUserClubs { get; }
        IDashboardRepository Dashboard { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
