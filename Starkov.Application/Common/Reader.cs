using Starkov.Application.Dtos.ViewModels;

namespace Starkov.Application.Common;
public sealed class Reader
{
    public async Task<IEnumerable<ReaderDepartmentViewModel>> ReadTsvAsDepartmentAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<ReaderDepartmentViewModel>();
        const int dataLength = 4;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsLengthIncorrectOrEmpty(data, dataLength))
            {
                continue;
            }

            TrimEmptyData(data);

            result.Add(new ReaderDepartmentViewModel
            {
                Name = data[0],
                ParentDepartment = data[1],
                Manager = data[2],
                Phone = data[3].NormalizePhoneNumber()
            });
        }

        return result;
    }

    public async Task<IEnumerable<ReaderEmployeeViewModel>> ReadTsvAsEmployeeAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<ReaderEmployeeViewModel>();
        const int dataLength = 5;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsLengthIncorrectOrEmpty(data, dataLength))
            {
                continue;
            }

            TrimEmptyData(data);

            result.Add(new ReaderEmployeeViewModel
            {
                Department = data[0],
                FullName = data[1],
                Login = data[2],
                RawPassword = data[3],
                JobTitle = data[4],
            });
        }

        return result;
    }

    public async Task<IEnumerable<ReaderJobTitleViewModel>> ReadTsvAsJobTitleAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<ReaderJobTitleViewModel>();
        const int dataLength = 2;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsLengthIncorrectOrEmpty(data, dataLength))
            {
                continue;
            }

            TrimEmptyData(data);

            result.Add(new ReaderJobTitleViewModel
            {
                Name = data[0]
            });
        }

        return result;
    }

    private void ThrowIfFileIncorrect(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }
        if (Path.GetExtension(path).ToLower() != ".tsv")
        {
            throw new InvalidOperationException("Invalid file type, expected *.tsv");
        }
    }

    private bool IsLengthIncorrectOrEmpty(string[] data, int length)
    {
        return data.Length == length && !data.All(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
    }

    private void TrimEmptyData(string[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = data[i].TrimEmptyEntries();
        }
    }
}
