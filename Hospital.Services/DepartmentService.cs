using Hospital.Models;
using Hospital.Repositories;
using Microsoft.EntityFrameworkCore;

public class DepartmentService
{
    private readonly ApplicationDbContext _context;

    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Department> GetOrCreateDepartmentAsync(string departmentName)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Name == departmentName);

        if (department == null)
        {
            department = new Department { Name = departmentName };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        return department;
    }
}
