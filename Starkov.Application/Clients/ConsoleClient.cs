using Starkov.Application.Dtos.Trees;
using Starkov.Application.Interfaces;
using Starkov.Domain.Repositories;
using System.Collections.ObjectModel;

namespace Starkov.Application.Clients;
public class ConsoleClient : IConsoleClient
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private OrganizationTree _tree;

    private int _maxSize = 10;

    public ConsoleClient(
        ImportService service,
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository)
    {
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
        }
        while (command != "выход");

        await InitTree();
    }

    private (string, KeyValuePair<string, string>[]) ParseCommand(string command)
    {
        var arr = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        KeyValuePair<string, string>[] options = new KeyValuePair<string, string>[arr.Length / 2 - 1];

        for (int i = 1; i < arr.Length; i++)
        {
            arr[i]
        }

        return (arr[0], options);
    }

    private async Task Invoke(string command, KeyValuePair<string, string>[] args)
    {
        if (command == )
    }

    private void Help()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("1) help");
        Console.WriteLine("2) import -p <path> -t <type> - импорт файла в БД" +
            "\n\t <path> - полный путь до tsv файла" +
            "\n\t <type> - тип импорта (d - отделы, e - сотрудники, j - должности)");
        Console.WriteLine("3) output -c <count> - вывод данных на экран" +
            "\n\t <count> - натуральное число, количество элементов для вывода на каждом уровне");
        Console.WriteLine("4) details -id <id> -c <count> - добавление новых подуровней" +
            "\n\t <id> - id отдела для которого надо вывести дополнительные уровни" +
            "\n\t <count> - количество элементов для вывода на уровне");
    }

    private async Task InitTree()
    {
        var queryableDepartment = (await _departmentRepository.GetQueryableAsync())
            .Where(x => x.ParentDepartmentId == null)
            .OrderBy(x => x.Name);

        var employeeQueryable = (await _employeeRepository.GetQueryableAsync())
            .OrderBy(x => x.FullName);

        var list = queryableDepartment.ToList();

        foreach (var item in list)
        {
            int directChildrenCount = queryableDepartment
                .Count(x => x.ParentDepartmentId == item.Id);

            var employees = employeeQueryable.Where(x => x.DepartmentId == item.Id)
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

            _tree.Departments.Add(child);
        }
    }
}
