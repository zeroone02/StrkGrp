namespace Starkov.Application.Dtos.Trees;
public class EmployeeItem
{
    public EmployeeItem(int id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }

    public int Id { get; set; }
    public string FullName { get; set; }
}
