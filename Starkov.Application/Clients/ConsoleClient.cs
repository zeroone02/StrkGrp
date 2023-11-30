using Starkov.Application.Dtos.Trees;
using Starkov.Application.Interfaces;
using Starkov.Domain;
using Starkov.Domain.Repositories;
using System.Collections.ObjectModel;

namespace Starkov.Application.Clients;
public class ConsoleClient : IConsoleClient
{
    private readonly ImportService _service;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private OrganizationTree _tree;

    private int _employeesCount = 10;
    private string[] _availableCommands = { "help", "import", "output", "details" };
    public ConsoleClient(
        ImportService service,
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository)
    {
        _service = service;
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;

        _tree = new OrganizationTree
        {
            Departments = new Collection<DepartmentTreeItem>(),
        };
    }

    public async Task RunAsync()
    {
        string command = string.Empty;
        Help();
        do
        {
            command = Console.ReadLine();
            var parsedCommand = ParseCommand(command);
            await ExecuteCommand(parsedCommand.Item1, parsedCommand.Item2);
        }
        while (command != "выход");
    }

    private (string, KeyValuePair<string, string>[]) ParseCommand(string command)
    {
        var arr = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var options = arr
            .Skip(1)
            .Chunk(2)
            .Select(x => new KeyValuePair<string, string>(x[0], x[1]))
            .ToArray();

        return (arr[0], options);
    }

    private async Task ExecuteCommand(string command, KeyValuePair<string, string>[] args)
    {
        if (!_availableCommands.Contains(command))
        {
            Console.WriteLine("Неизвестная команда");
            return;
        }

        if (command == "import")
        {
            var path = args.FirstOrDefault(x => x.Key == "-p").Value;
            var typeStr = args.FirstOrDefault(x => x.Key == "-t").Value;
            ImportType type = ImportType.Department;
            if (typeStr == "d")
            {
                type = ImportType.Department;
            }
            else if (typeStr == "e")
            {
                type = ImportType.Employee;
            }
            else if (typeStr == "j")
            {
                type = ImportType.JobTitle;
            }
            else
            {
                Console.WriteLine("Неизвестное значение для аргумента -t");
            }

            await _service.ImportTsvAsync(path, type);
        }
        else if (command == "help")
        {
            Help();
        }
        else if(command == "expand")
        {
            _employeesCount = Convert.ToInt32(args[0]);
        }
        else if (command == "output")
        {
            await CreateTree();
            Console.WriteLine();
            await DrawTreeAsync(_tree.Departments, 0);
            Console.WriteLine();
        }
    }

    private void Help()
    {
        Console.WriteLine();
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("1) help");
        Console.WriteLine("2) import -p <path> -t <type> - импорт файла в БД" +
            "\n\t <path> - полный путь до tsv файла" +
            "\n\t <type> - тип импорта (d - отделы, e - сотрудники, j - должности)");
        Console.WriteLine("3) output -c <count> - вывод данных на экран" +
            "\n\t <count> - натуральное число, количество элементов для вывода на каждом уровне");
        Console.WriteLine("4) expand <count> - установка максимального количества сотрудников для вывода" +
            "\n\t <count> - число, количество сотрудников (-1 для вывода всех, по умолчанию 10)");
        Console.WriteLine();
    }

    private async Task DrawTreeAsync(IEnumerable<DepartmentTreeItem> items, int depth)
    {
        foreach (var item in items)
        {
            Console.WriteLine($"{new string('=', depth)}{item.Name} ({item.Id})");
            if (item.Manager != null)
            {
                Console.WriteLine($"{new string(' ', depth)}*{item.Manager.FullName}");
            }
            foreach (var employee in item.Employees)
            {
                Console.WriteLine($"{new string(' ', depth)}-{employee.FullName}");
            }
            await DrawTreeAsync(item.Departments, depth + 1);
        }
    }

    private async Task CreateTree()
    {
        _tree.Departments = new Collection<DepartmentTreeItem>(await CreateTree(null, _employeesCount));
    }

    private async Task<List<DepartmentTreeItem>> CreateTree(int? id, int employeesCount)
    {
        var queryableDepartment = (await _departmentRepository.GetQueryableAsync())
            .OrderBy(x => x.Name);

        var employeeQueryable = (await _employeeRepository.GetQueryableAsync())
            .OrderBy(x => x.FullName);

        var list = queryableDepartment
            .Where(x => x.ParentDepartmentId == id)
            .ToList();

        List<DepartmentTreeItem> result = new List<DepartmentTreeItem>();
        foreach (var item in list)
        {
            int directChildrenCount = queryableDepartment
                .Count(x => x.ParentDepartmentId == item.Id);

            List<EmployeeItem> employees = new List<EmployeeItem>();

            if (employeesCount < 0)
            {
                employees = employeeQueryable.Where(x => x.DepartmentId == item.Id)
                    .Select(x => new EmployeeItem(x.Id, x.FullName))
                    .ToList();
            }
            else
            {
                employees = employeeQueryable.Where(x => x.DepartmentId == item.Id)
                    .Select(x => new EmployeeItem(x.Id, x.FullName))
                    .Take(employeesCount)
                    .ToList();
            }

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
            var items = await CreateTree(child.Id, employeesCount);
            result.Add(child);
            child.Departments = new Collection<DepartmentTreeItem>(items);
        }

        return result;
    }
}
