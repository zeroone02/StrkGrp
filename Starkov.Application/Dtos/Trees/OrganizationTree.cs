using System.Collections.ObjectModel;

namespace Starkov.Application.Dtos.Trees;
public class OrganizationTree
{
    public Collection<DepartmentTreeItem> Departments { get; set; }
}