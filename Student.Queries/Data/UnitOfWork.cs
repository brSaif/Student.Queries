using StudentQueries.Domain;

namespace StudentQueries.Data;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    
    IStudentRepository StudentRepository { get; }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    
    public IStudentRepository StudentRepository { get; private set; }
    
    public UnitOfWork(
        AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        this.StudentRepository = new StudentRepository(_appDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
        => await _appDbContext.SaveChangesAsync();

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}