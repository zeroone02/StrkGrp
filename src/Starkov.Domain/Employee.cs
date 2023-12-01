namespace Starkov.Domain;
public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public int JobTitleId { get; set; }
    public JobTitle JobTitle { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}