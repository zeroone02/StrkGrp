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
    private int _maxCount = 10;
    private string[] _availableCommands = { "help", "import", "output", "expand" };
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
            Console.Write("> ");
            command = Console.ReadLine();
            var parsedCommand = ParseCommand(command);
            await ExecuteCommand(parsedCommand.Item1, parsedCommand.Item2);
        }
        while (command != "выход");
    }

    private (string, KeyValuePair<string, string>[]) ParseCommand(string command)
    {
        var arr = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

        var options = arr
            .Skip(1)
            .Chunk(2);

        foreach (var option in options)
        {
            if (option.Length != 2)
            {
                continue;
            }
            result.Add(new KeyValuePair<string, string>(option[0], option[1]));
        }

        return (arr[0], result.ToArray());
    }

    private async Task ExecuteCommand(string command, KeyValuePair<string, string>[] args)
    {
        if (!_availableCommands.Contains(command))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Неизвестная команда");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Неверный синтаксис команды или значение параметров");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            try
            {
                var result = await _service.ImportTsvAsync(path, type);
                if (result.TotalCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Успешно импортировано ({result.TotalCount}): " +
                        $"\n\tдобавлено: {result.AddedCount}" +
                        $"\n\tобновлено: {result.UpdatedCount}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Записи не были обновлены и не были добавлены (возможно такие данные уже существуют)");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файл не найден");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось выполнить команду ({ex.GetType()}: {ex.Message})");
            }

        }
        else if (command == "help")
        {
            Help();
        }
        else if (command == "expand")
        {
            var expand = args.FirstOrDefault(x => x.Key == "-c").Value;
            if (expand == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Неверный синтаксис команды");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            _maxCount = Convert.ToInt32(expand);
            if(_maxCount < 0 || _maxCount > 40)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: вывод может быть медленным!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        else if (command == "output")
        {
            if (args.Any())
            {
                var option = args.FirstOrDefault(x => x.Key == "-id").Value;
                if (int.TryParse(option, out int id))
                {
                    await CreateTree(id);
                }
                else
                {
                    Console.WriteLine("Переданный идентификатор не число");
                    return;
                }
            }
            else
            {
                await CreateTree(null);
            }
            if (_tree.Departments.Any())
            {
                Console.WriteLine();
                await DrawTreeAsync(_tree.Departments, 1);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Данные отсутствуют");
            }
        }
    }

    private void Help()
    {
        Console.WriteLine();
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("1) help");
        Console.WriteLine("2) import -p <path> -t <type> - импорт файла в БД" +
            "\n\t -p <path> - полный путь до tsv файла" +
            "\n\t -t <type> - тип импорта (d - отделы, e - сотрудники, j - должности)");
        Console.WriteLine("3) output -id <id> - вывод данных на экран" +
            "\n\t-id <id> - опциональный параметр, идентификатор отдела");
        Console.WriteLine("4) expand -c <count> - установка максимального количества элементов для вывода на уровне" +
            "\n\t -c <count> - число, макс. количество отделов и работников для уровняы, по умолчанию 10, значение -1 - вывод всех.");
        Console.WriteLine();
    }

    private async Task DrawTreeAsync(IEnumerable<DepartmentTreeItem> items, int depth)
    {
        foreach (var item in items)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{new string('=', depth)}{item.Name} ({item.Id})");
            Console.ForegroundColor = ConsoleColor.White;
            if (item.Manager != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{new string(' ', depth)}*{item.Manager.FullName}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            foreach (var employee in item.Employees)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{new string(' ', depth)}-{employee.FullName}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            await DrawTreeAsync(item.Departments, depth + 1);
        }
    }

    private async Task CreateTree(int? id)
    {
        _tree.Departments = new Collection<DepartmentTreeItem>(await CreateTree(id, _maxCount, false));
    }

    //nested - костыль для того чтобы не уйти в беск цикл
    //если true - то надо доставать по parentId, иначе по id.
    private async Task<List<DepartmentTreeItem>> CreateTree(int? id, int employeesCount, bool nested)
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
            var items = await CreateTree(child.Id, employeesCount, true);
            result.Add(child);
            child.Departments = new Collection<DepartmentTreeItem>(items);
        }

        return result;
    }
}
