namespace RunGroupWebApp.Interfaces.IReposiotry
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
