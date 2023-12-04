using Starkov.Application.Dtos;
using Starkov.Application.Dtos.Trees;
using Starkov.Application.Interfaces;
using Starkov.Domain;
using Starkov.Domain.Repositories;
using System.Collections.ObjectModel;

namespace Starkov.Application.Clients;
public class ConsoleClient : IConsoleClient
{
    private readonly ImportService _importService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    private OrganizationTree _tree;
    private string[] _availableCommands = { "import", "output" };
    public ConsoleClient(
        ImportService service,
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository)
    {
        _importService = service;
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;

        _tree = new OrganizationTree
        {
            Departments = new Collection<DepartmentTreeItem>(),
        };
    }

    public async Task RunAsync(string[] args)
    {
        if (args.Length == 0 || !_availableCommands.Contains(args.FirstOrDefault()))
        {
            WriteLine("Отсутствует команда import или output", ConsoleColor.Red);
            return;
        }

        if (args[0] == "import")
        {
            var prepeare = args.Skip(1).Chunk(2);
            if (prepeare.Count() != 2)
            {
                WriteLine("Неверные аргументы для команды import", ConsoleColor.Red);
                return;
            }

            var parameters = prepeare.Select(x => new KeyValuePair<string, string>(x[0], x[1]));
            if (!CheckImportParameters(parameters, out string type, out string path))
            {
                WriteLine("Неверные аргументы для команды import", ConsoleColor.Red);
                return;
            }

            var result = await ExecuteImportCommandAsync(type, path);
            Console.WriteLine($"Импортировано: {result.AddedCount}, обновлено: {result.UpdatedCount}");
        }
        else if (args[0] == "output")
        {
            var prepeare = args.Skip(1).Chunk(2);
            if (prepeare.Count() == 0)
            {
                await CreateTreeAsync(null);
                await DrawTreeAsync(_tree.Departments, 1);
            }
            else if(prepeare.Count() == 1)
            {
                var parameters = prepeare.Select(x => new KeyValuePair<string, string>(x[0], x[1]));

                if (CheckOutputParameters(parameters, out int? id))
                {
                    await CreateTreeAsync(id);
                    await DrawTreeAsync(_tree.Departments, 1);
                }
            }
            else
            {
                WriteLine("Неверные аргументы для команды output", ConsoleColor.Red);
            }

        }
    }

    private async Task<TsvImportResult> ExecuteImportCommandAsync(string typeStr, string path)
    {
        var type = typeStr switch
        {
            "d" => ImportType.Department,
            "j" => ImportType.JobTitle,
            "e" => ImportType.Employee
        };

        return await _importService.ImportTsvAsync(path, type);
    }

    private bool CheckImportParameters(IEnumerable<KeyValuePair<string, string>> parameters, out string type, out string path)
    {
        const string availableTParamValues = "dej";
        
        type = parameters.FirstOrDefault(x => x.Key == "-t").Value;
        path = parameters.FirstOrDefault(x => x.Key == "-p").Value;

        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (!availableTParamValues.Contains(type[0])) //что бы нельзя было передать dej, а только 1 символ
        {
            return false;
        }

        return true;
    }

    private bool CheckOutputParameters(IEnumerable<KeyValuePair<string, string>> parameters, out int? id)
    {
        var val = parameters.FirstOrDefault(x => x.Key == "-id").Value;
        if (int.TryParse(val, out int parseId))
        {
            id = parseId;
            return true;
        }

        id = null;
        return false;
    }

    private async Task DrawTreeAsync(IEnumerable<DepartmentTreeItem> items, int depth)
    {
        foreach (var item in items)
        {
            WriteLine($"{new string('=', depth)}{item.Name} ({item.Id}), подразделов: {item.DirectChildrenCount}", ConsoleColor.DarkGreen);

            if (item.Manager != null)
            {
                WriteLine($"{new string(' ', depth)}*{item.Manager.FullName}", ConsoleColor.Magenta);
            }
            foreach (var employee in item.Employees)
            {
                WriteLine($"{new string(' ', depth)}-{employee.FullName}", ConsoleColor.Yellow);
            }
            await DrawTreeAsync(item.Departments, depth + 1);
        }
    }

    private async Task CreateTreeAsync(int? id)
    {
        _tree.Departments = new Collection<DepartmentTreeItem>(await CreateTreeAsync(id, false));
    }

    //nested - костыль для того чтобы не уйти в беск цикл
    //если true - то надо доставать по parentId, иначе по id.
    private async Task<List<DepartmentTreeItem>> CreateTreeAsync(int? id, bool nested)
    {
        var queryableDepartment = (await _departmentRepository.GetQueryableAsync())
            .OrderBy(x => x.Name);

        var employeeQueryable = (await _employeeRepository.GetQueryableAsync())
            .OrderBy(x => x.FullName);

        List<Department> list = new List<Department>();

        if (id != null && !nested)
        {
            list = queryableDepartment
                .Where(x => x.Id == id)
                .ToList();
        }
        else
        {
            list = queryableDepartment
                .Where(x => x.ParentDepartmentId == id)
                .ToList();
        }

        List<DepartmentTreeItem> result = new List<DepartmentTreeItem>();
        foreach (var item in list)
        {
            int directChildrenCount = queryableDepartment
                .Count(x => x.ParentDepartmentId == item.Id);

            List<EmployeeItem> employees = new List<EmployeeItem>();


            employees = employeeQueryable.Where(x => x.DepartmentId == item.Id)
                .Select(x => new EmployeeItem(x.Id, x.FullName))
                .ToList();


            var child = new DepartmentTreeItem
            {
                Id = item.Id,
                Name = item.Name,
                DirectChildrenCount = directChildrenCount,
                Employees = new Collection<EmployeeItem>(employees)
            };
            var manager = child.Employees.FirstOrDefault(x => x.Id == item.ManagerId);
            if (manager != null)
            {
                child.Employees.Remove(manager);
                child.Manager = manager;
            }
            var items = await CreateTreeAsync(child.Id, true);
            result.Add(child);
            child.Departments = new Collection<DepartmentTreeItem>(items);
        }

        return result;
    }

    private void WriteLine(string text, ConsoleColor textColor, ConsoleColor initColor = ConsoleColor.White)
    {
        Console.ForegroundColor = textColor;
        Console.WriteLine(text);
        Console.ForegroundColor = initColor;
    }
}
