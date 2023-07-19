using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StudentQueries.Domain;
using StudentQueries.QueryServices.Filter;
using StudentQueries.QueryServices.Find;

namespace StudentQueries.Data;

public interface IStudentRepository
{
    Task<Domain.Student?> GetByIdAsync(Guid studentId);
    Task AddAsync(Domain.Student student);
    Task<FilterResult> FilterAsync(FilterQuery filter, CancellationToken cancellationToken = default);
    Task<Domain.Student?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _appDbContext;

    public StudentRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Domain.Student?> GetByIdAsync(Guid studentId)
        => await _appDbContext.Students.FindAsync(studentId);

    public async Task AddAsync(Domain.Student student)
    => await _appDbContext.Students.AddAsync(student);

    public async Task<FilterResult> FilterAsync(FilterQuery filter, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Students.AsQueryable();

        if (filter.CreatedAfter != null)
            query.Where(s => s.CreatedAt >= filter.CreatedAfter);

        var total = await query.CountAsync(cancellationToken);

        var result = await query.Skip(filter.Skip)
            .Take(filter.Size)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return new FilterResult(
            filter.Page,
            filter.Size,
            total,
            result);
    }

    public async Task<Domain.Student?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        => await _appDbContext.Students.FindAsync(new object[] { id }, cancellationToken);
}