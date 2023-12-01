using Starkov.Application.Common;
using Starkov.Application.Dtos.ViewModels;

namespace Starkov.Application;
public sealed class TsvReader
{
    public async IAsyncEnumerable<ReaderDepartmentViewModel> ReadTsvAsDepartmentAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        const int dataLength = 4;
        int count = 1;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла
        while (!reader.EndOfStream)
        {
            count++;
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (!IsLengthIncorrectOrEmpty(data, dataLength))
            {
                Console.WriteLine($"{Path.GetFileName(path)}: сломанные данные, строка: {count}");
                continue;
            }

            TrimEmptyData(data);

            yield return new ReaderDepartmentViewModel
            {
                Name = data[0],
                ParentDepartment = data[1],
                ManagerFullName = data[2],
                Phone = data[3].NormalizePhoneNumber()
            };
        }
    }

    public async IAsyncEnumerable<ReaderEmployeeViewModel> ReadTsvAsEmployeeAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        const int dataLength = 5;
        int count = 1;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла
        while (!reader.EndOfStream)
        {
            count++;
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (!IsLengthIncorrectOrEmpty(data, dataLength))
            {
                Console.WriteLine($"{Path.GetFileName(path)}: сломанные данные, строка: {count}");
                continue;
            }

            TrimEmptyData(data);

            yield return new ReaderEmployeeViewModel
            {
                DepartmentName = data[0],
                FullName = data[1],
                Login = data[2],
                RawPassword = data[3],
                JobTitleName = data[4],
            };
        }
    }

    public async IAsyncEnumerable<ReaderJobTitleViewModel> ReadTsvAsJobTitleAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        const int dataLength = 1;
        int count = 1;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            count++;
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (!IsLengthIncorrectOrEmpty(data, dataLength))
            {
                Console.WriteLine($"{Path.GetFileName(path)}: сломанные данные, строка: {count}");
                continue;
            }

            TrimEmptyData(data);

            yield return new ReaderJobTitleViewModel
            {
                Name = data[0]
            };
        }
    }

    private void ThrowIfFileIncorrect(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }
        if (Path.GetExtension(path).ToLower() != ".tsv")
        {
            throw new InvalidOperationException("Неправильный формат файла, ожидалось: tsv");
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
