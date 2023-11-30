using Starkov.Application.Common;
using Starkov.Application.Dtos;
using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.Application;
public class ImportService
{
    private readonly TsvReader _tsvReader;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJobTitleRepository _titleRepository;

    public ImportService(
        IDepartmentRepository repository,
        IEmployeeRepository employeeRepository,
        IJobTitleRepository titleRepository)
    {
        _tsvReader = new TsvReader();
        _departmentRepository = repository;
        _employeeRepository = employeeRepository;
        _titleRepository = titleRepository;
    }
    public async Task<TsvImportResult> ImportTsvAsync(string path, ImportType type)
    {
        return type switch
        {
            ImportType.Department => await ImportDepartmentsAsync(path),
            ImportType.Employee => await ImportEmployeeAsync(path),
            _ => throw new ArgumentException("Неизвестный тип импорта")
        };
    }

    private async Task<TsvImportResult> ImportDepartmentsAsync(string path)
    {
        List<Department> toAdd = new List<Department>();
        List<Department> toUpdate = new List<Department>();

        var departments = new Dictionary<string, Department>();
        var employees = new Dictionary<string, Employee>();

        await foreach (var item in _tsvReader.ReadTsvAsDepartmentAsync(path))
        {
            var department = await _departmentRepository.GetAsync(item.Name, item.ParentDepartment);
            if (department != null)
            {
                var manager = await _employeeRepository.GetAsync(item.ManagerFullName);
                var data = new Department
                {
                    Name = item.Name,
                    PhoneNumber = item.Phone,
                };

                if(departments.ContainsKey(item.ParentDepartment))
                {
                    data.ParentDepartment = departments[item.ParentDepartment];
                }

                if (employees.ContainsKey(item.ManagerFullName))
                {
                    data.Manager = employees[item.ManagerFullName];
                }
                else
                {
                    data.Manager = await _employeeRepository.GetAsync(item.ManagerFullName);
                }

                toAdd.Add(data);
                departments.Add(item.Name, data);
            }
        }

        return new TsvImportResult
        {
            AddedCount = toAdd.Count,
            UpdatedCount = toUpdate.Count,
            TotalCount = toAdd.Count + toUpdate.Count,
        };
    }

    private async Task<TsvImportResult> ImportEmployeeAsync(string path)
    {
        List<Employee> toAdd = new List<Employee>();
        List<Employee> toUpdate = new List<Employee>();

        var jobTitlesMap = new Dictionary<string, JobTitle>();

        await foreach (var item in _tsvReader.ReadTsvAsEmployeeAsync(path))
        {
            var department = await _employeeRepository.GetAsync(item.FullName);
            if (department != null)
            {
                var data = new Employee
                {
                    FullName = item.FullName,
                    Login = item.Login,
                    PasswordHash = item.RawPassword.GenerateSHA256Hash()
                };

                if (jobTitlesMap.ContainsKey(item.JobTitle))
                {
                    data.JobTitle = jobTitlesMap[item.JobTitle];
                }
                else
                {
                    data.JobTitle = await _titleRepository.GetAsync(item.JobTitle);
                }

                toAdd.Add(data);
                jobTitlesMap.Add(item.Name, data);
            }
        }

        return new TsvImportResult
        {
            AddedCount = toAdd.Count,
            UpdatedCount = toUpdate.Count,
            TotalCount = toAdd.Count + toUpdate.Count,
        };
    }
}
