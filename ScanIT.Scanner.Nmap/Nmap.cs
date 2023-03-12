using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using ScanIT.Scanner.Nmap.Model;
using ScanIT.Scanner.Nmap.Options;
using ScanIT.Scanner.Nmap.Options.Target;

namespace ScanIT.Scanner.Nmap;

public partial class Nmap
{
    public event EventHandler<OutputEventArgs>? ProgressHandler;

    public static string GetPathToNmap()
    {
# if Linux 
        return "nmap";
# elif Windows
        return LocateExecutable("nmap.exe");
# else
        throw new PlatformNotSupportedException();
# endif
    }

    public static string BuildTargetArgument(List<ITarget> targets)
    {
        var targetList = new StringBuilder();
        foreach (var target in targets)
        {
            if(targetList.Length > 0)
                targetList.Append(' ');
            targetList.Append(target);
        }

        return targetList.ToString();
    }

    public static string BuildNmapArguments(ITarget targets, ScanOptions scanOptions)
    {
        var builder = new StringBuilder();
        AppendArgument(builder, scanOptions.Output.GetOutputArgument());
        AppendArgument(builder, scanOptions.Arguments);
        AppendArgument(builder, scanOptions.Ports.ToString());
        AppendArgument(builder, targets.ToString());
        return builder.ToString();
    }

    public void Scan(ITarget targets, ScanOptions scanOptions)
    {
        scanOptions.Output.StatsEvery = "1s";
        scanOptions.Output.NonInteractive = false;
        scanOptions.Output.File = $"result-{DateTime.Now}.xml";
        scanOptions.Output.Format = OutputFormat.XML;
        var arguments = BuildNmapArguments(targets, scanOptions);
        RunNmap(arguments);
    }

    public static nmaprun ConvertResult(string fullPath)
    {
        var result = ConvertXml<nmaprun>(fullPath);
        return result;
    }

    public static T ConvertXml<T>(string filename, XmlSerializer? serial = default, ValidationEventHandler? validationCallBack = default)
    {
        var method = typeof(XmlSerializer).GetMethod("set_Mode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        method?.Invoke(null, new object[] { 1 });
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            IgnoreWhitespace = true,
            IgnoreComments = true,
        };
        var cleanXml = RemoveEmptyNodesFromXml(File.ReadAllText(filename));
        var xml = new XmlDocument();
        xml.LoadXml(cleanXml);
        var nodes = xml.SelectNodes("/nmaprun/hosthint");
        if (nodes != null)
            foreach (XmlNode node in nodes)
                node.ParentNode?.RemoveChild(node);
        cleanXml = xml.OuterXml;
        settings.ValidationEventHandler += validationCallBack;
        serial = serial ?? new XmlSerializer(typeof(T));
        using var reader = XmlReader.Create(new StringReader(cleanXml), settings);
        return (T)serial.Deserialize(reader)!;
    }

    private void RunNmap(string arguments)
    {
        var path = GetPathToNmap();
        var process = new Process();
        process.StartInfo.FileName = path;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.OutputDataReceived += OutputHandler;
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
    }

    [GeneratedRegex("(?<name>.*) Timing: About (?<completed>[0-9]{1,3}.[0-9]{1,2})%")]
    private static partial Regex NmapOutputStateRegex();

    private static string LocateExecutable(string filename)
    {
        var path = Environment.GetEnvironmentVariable("path");
        var folders = path?.Split(';');
        if (path == null || folders == null)
            throw new ValidationException("Could not find path to nmap.exe");
        foreach (var folder in folders)
        {
            var combined = Path.Combine(folder, filename);
            if (File.Exists(combined))
            {
                return combined;
            }
        }

        return string.Empty;
    }


    private static void AppendArgument(StringBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value))
            return;
        if(builder.Length > 0)
            builder.Append(' ');
        builder.Append(value);
    }

    private void OutputHandler(object sendingProcess,
        DataReceivedEventArgs outLine)
    {
        if (string.IsNullOrEmpty(outLine.Data)) return;
        var currentStep = "";
        var progress = 0f;
        var match = NmapOutputStateRegex().Match(outLine.Data);
        if(match.Groups.Keys.Contains("name"))
            currentStep = match.Groups["name"].Value;
        if(match.Groups.Keys.Contains("completed"))
            progress = float.Parse(match.Groups["completed"].Value);
        ProgressHandler?.Invoke(this, new OutputEventArgs(currentStep, progress, outLine.Data));
    }

    private static string RemoveEmptyNodesFromXml(string xmlDoc)
    {
 
        var document = XDocument.Parse(xmlDoc);
        document.Descendants()
            .Where(e => e.Attributes().All(a => a.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(a.Value))
                         && string.IsNullOrWhiteSpace(e.Value)
                         && e.Descendants().SelectMany(c => c.Attributes()).All(ca => ca.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(ca.Value)))
            .Remove();
        return document.ToString();
    }
    
    public class OutputEventArgs
    {
        public OutputEventArgs(string? currentStep, float progress, string output)
        {
            CurrentStep = currentStep;
            Progress = progress;
            Output = output;
        }
        public string? CurrentStep { get;  }
        public float Progress { get; }
        public string Output { get; }
    }
}