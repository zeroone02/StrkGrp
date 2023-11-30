namespace Starkov.Application.Common;
public sealed class Reader
{
    public async Task<IEnumerable<string[]>> ReadTsvAsDepartmentAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<string[]>();
        const int dataLength = 4;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsFileLineCorrect(data, dataLength))
            {
                continue;
            }

            result.Add(data);
        }

        return result;
    }

    public async Task<IEnumerable<string[]>> ReadTsvAsEmployeeAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<string[]>();
        const int dataLength = 5;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsFileLineCorrect(data, dataLength))
            {
                continue;
            }

            result.Add(data);
        }

        return result;
    }

    public async Task<IEnumerable<string[]>> ReadTsvAsJobTitleAsync(string path)
    {
        ThrowIfFileIncorrect(path);

        var result = new List<string[]>();
        const int dataLength = 2;

        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); //скипаем заголовок файла

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            string[] data = line.Split('\t');
            if (IsFileLineCorrect(data, dataLength))
            {
                continue;
            }

            result.Add(data);
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

    private bool IsFileLineCorrect(string[] data, int length)
    {
        return data.Length == length && !data.All(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
    }
}
