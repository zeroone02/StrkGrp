using System.Collections.ObjectModel;

namespace Starkov.Application.Dtos.Trees;
public class OrganizationTree
{
    public Collection<DepartmentTreeItem> Departments { get; set; }
    public DepartmentTreeItem GetDepartment(int id)
    {
        var queue = new Queue<DepartmentTreeItem>();

        foreach (var item in Departments)
        {
            queue.Enqueue(item);
        }

        while (queue.Any())
        {
            var department = queue.Dequeue();
            if (department.Id == id)
            {
                return department;
            }
            foreach (var item in department.Departments)
            {
                queue.Enqueue(item);
            }
        }

        return default;
    }
}