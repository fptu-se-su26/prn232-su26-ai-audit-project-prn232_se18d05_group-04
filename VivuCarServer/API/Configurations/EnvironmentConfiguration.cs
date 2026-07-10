namespace API.Configurations;

public static class EnvironmentConfiguration
{
    public static void LoadEnvFile()
    {
        var envPath = FindEnvFile();

        if (envPath is null)
        {
            return;
        }

        foreach (var line in File.ReadAllLines(envPath))
        {
            LoadEnvironmentVariable(line);
        }
    }

    private static void LoadEnvironmentVariable(string line)
    {
        var trimmedLine = line.Trim();

        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
        {
            return;
        }

        var separatorIndex = trimmedLine.IndexOf('=');

        if (separatorIndex <= 0)
        {
            return;
        }

        var key = trimmedLine[..separatorIndex].Trim();
        var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

        if (string.IsNullOrWhiteSpace(key) || Environment.GetEnvironmentVariable(key) is not null)
        {
            return;
        }

        Environment.SetEnvironmentVariable(key, value);
    }

    private static string? FindEnvFile()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            var envPath = Path.Combine(directory.FullName, ".env");

            if (File.Exists(envPath))
            {
                return envPath;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
