using System.Collections.ObjectModel;

namespace Starkov.Application.Dtos.Trees;
public class DepartmentTreeItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EmployeeItem Manager { get; set; }
    public IEnumerable<DepartmentTreeItem> Departments { get; set; }
    public IEnumerable<EmployeeItem> Employees { get; set; }
    public int? ParentDepartmentId { get; set; }
}
