using Starkov.Domain;

namespace Starkov.Application.Dtos;
internal class ReaderDepartmentViewModel
{
    public string Name { get; set; }
    public string ParentDepartment { get; set; }
    public string Manager { get; set; }
    public string Phone { get; set; }
}
