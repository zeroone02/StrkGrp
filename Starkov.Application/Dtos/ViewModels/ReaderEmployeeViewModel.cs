using Starkov.Domain;

namespace Starkov.Application.Dtos.ViewModels;
public class ReaderEmployeeViewModel
{
    public string FullName { get; set; }
    public string Login { get; set; }
    public string RawPassword { get; set; }
    public string JobTitleName { get; set; }
    public string Department { get; set; }
}
