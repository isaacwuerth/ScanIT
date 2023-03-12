using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScanIT.Nmap;

public class Output
{
    public string Directory { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public string FullPath => Path.Combine(Directory, File);
    public OutputFormat Format { get; set; } = OutputFormat.None;
    public string StatsEvery { get; set; } = string.Empty;
    public bool NonInteractive { get; set; }
    public bool NoStylesheet { get; set; } = true;
    public string GetOutputArgument()
    {
        var output = new StringBuilder(string.Empty);
        var extension = string.Empty;
        if(!string.IsNullOrEmpty(StatsEvery))
            output.Append($"--stats-every {StatsEvery} ");
        if(NonInteractive)
            output.Append("--noninteractive ");
        if(Format == OutputFormat.None)
            return output.ToString().Trim();
        if(string.IsNullOrEmpty(File))
            throw new ValidationException("File must be set if Format is not None");
        if(!string.IsNullOrEmpty(Directory))
            Directory = Directory.TrimEnd('/') + "/";
        switch(Format)
        {
            case OutputFormat.XML:
                if(NoStylesheet)
                    output.Append("--no-stylesheet ");
                output.Append("-oX");
                break;
            case OutputFormat.JSON:
                output.Append("-oJ");
                break;
            case OutputFormat.Grepable:
                output.Append("-oG");
                break;
            case OutputFormat.Normal:
                output.Append("-oN");
                break;
            case OutputFormat.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return $"{output} {FullPath}";
    }
}