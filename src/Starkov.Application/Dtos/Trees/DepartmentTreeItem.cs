using System.Collections.ObjectModel;

namespace Starkov.Application.Dtos.Trees;
public class DepartmentTreeItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EmployeeItem Manager { get; set; }
    public Collection<DepartmentTreeItem> Departments { get; set; }
    public Collection<EmployeeItem> Employees { get; set; }
    public int DirectChildrenCount { get; set; }
}
