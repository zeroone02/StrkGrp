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
            ImportType.JobTitle => await ImportJobTitleAsync(path),
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
            char s = char.ToUpper(item.Name[0]);
            item.Name = item.Name.ToLower().Remove(0, 1);
            item.Name = s + item.Name;

            var data = await _departmentRepository.GetAsync(item.Name, item.ParentDepartment);

            if (data == null)
            {
                data = new Department
                {
                    Name = item.Name,
                    PhoneNumber = item.Phone,
                };
                toAdd.Add(data);
            }
            else
            {
                data.Name = item.Name;
                data.PhoneNumber = item.Phone;
                toUpdate.Add(data);
            }

            if (departments.ContainsKey(item.ParentDepartment))
            {
                data.ParentDepartment = departments[item.ParentDepartment];
            }
            
            if(!departments.ContainsKey(item.Name))
            {
                departments.Add(item.Name, data);
            }

            if (employees.ContainsKey(item.ManagerFullName))
            {
                data.Manager = employees[item.ManagerFullName];
            }
            else
            {
                data.Manager = await _employeeRepository.GetAsync(item.ManagerFullName);
                if (data.Manager != null)
                {
                    employees.Add(data.Manager.FullName, data.Manager);
                }
            }
        }

        if (toAdd.Any())
        {
            await _departmentRepository.InsertRangeAsync(toAdd);
        }
        if (toUpdate.Any())
        {
            await _departmentRepository.UpdateRangeAsync(toAdd);
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
        var departmentsDictionary = new Dictionary<string, Department>();

        await foreach (var item in _tsvReader.ReadTsvAsEmployeeAsync(path))
        {
            item.FullName = string.Join(' ',
                item.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select((x) =>
                {
                    x = x.ToLower();
                    char s = char.ToUpper(x[0]);
                    x = x.Remove(0, 1);
                    return s + x;
                }));
            var data = await _employeeRepository.GetAsync(item.FullName);
            if (data == null)
            {
                data = new Employee
                {
                    FullName = item.FullName,
                    Login = item.Login,
                    PasswordHash = item.RawPassword.ToMd5()
                };
                toAdd.Add(data);
            }
            else
            {
                data.Login = item.Login;
                data.PasswordHash = item.RawPassword.ToMd5();
                toUpdate.Add(data);
            }

            if (jobTitlesMap.ContainsKey(item.JobTitleName))
            {
                data.JobTitle = jobTitlesMap[item.JobTitleName];
            }
            else if (!string.IsNullOrEmpty(item.JobTitleName))
            {
                data.JobTitle = await _titleRepository.GetAsync(item.JobTitleName);
                jobTitlesMap.Add(item.JobTitleName, data.JobTitle);
            }

            if (departmentsDictionary.ContainsKey(item.DepartmentName))
            {
                data.Department = departmentsDictionary[item.DepartmentName];
            }
            else if (!string.IsNullOrEmpty(item.DepartmentName))
            {
                data.Department = await _departmentRepository.GetAsync(item.DepartmentName);
                departmentsDictionary.Add(item.DepartmentName, data.Department);
            }

            if (data.Department == null || data.JobTitle == null)
            {
                toAdd.Remove(data);
                toUpdate.Remove(data);
                Console.WriteLine($"{Path.GetFileName(path)}: сломанные данные, отсутствует отдел или должность");
            }
        }

        if (toAdd.Count > 0)
        {
            await _employeeRepository.InsertRangeAsync(toAdd);
        }
        if (toUpdate.Count > 0)
        {
            await _employeeRepository.UpdateRangeAsync(toUpdate);
        }

        return new TsvImportResult
        {
            AddedCount = toAdd.Count,
            UpdatedCount = toUpdate.Count,
            TotalCount = toAdd.Count + toUpdate.Count,
        };
    }

    private async Task<TsvImportResult> ImportJobTitleAsync(string path)
    {
        var toAdd = new List<JobTitle>();
        await foreach (var item in _tsvReader.ReadTsvAsJobTitleAsync(path))
        {
            if (!await _titleRepository.ContainsAsync(item.Name))
            {
                toAdd.Add(new JobTitle
                {
                    Name = item.Name
                });
            }
        }

        if (toAdd.Any())
        {
            await _titleRepository.InsertRange(toAdd);
        }

        return new TsvImportResult
        {
            AddedCount = toAdd.Count,
            TotalCount = toAdd.Count
        };
    }
}
