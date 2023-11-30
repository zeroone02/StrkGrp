namespace Starkov.Domain;
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentDepartmentId { get; set; }
    public Department ParentDepartment { get; set; }
    public int? ManagerId { get; set; }
    public Employee Manager { get; set; }
    public string PhoneNumber { get; set; }
}

