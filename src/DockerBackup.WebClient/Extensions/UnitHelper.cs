namespace DockerBackup.WebClient.Extensions;

public static class UnitHelper
{
    private static readonly string[] Sizes = ["B", "KB", "MB", "GB", "TB"];

    public static string HumanReadableSize(long? sizeInBytes, int decimalPlaces = 2)
    {
        double len = sizeInBytes ?? 0;
        int order = 0;
        while (len >= 1024 && order < Sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
        // show a single decimal place, and no space.
        var decimals = new string('#', decimalPlaces);
        return string.Format($$"""{0:0.{{decimals}}} {1}""", len, Sizes[order]);
    }
}
