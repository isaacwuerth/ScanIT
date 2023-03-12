using ScanIT.Scanner.Nmap;
using ScanIT.Scanner.Nmap.Options;
using ScanIT.Scanner.Nmap.Options.Ports;
using ScanIT.Scanner.Nmap.Options.Target;
using Xunit.Abstractions;

namespace ScanITTest.NmapTest;

public class NmapTests
{
    private readonly ITestOutputHelper output;

    public NmapTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    private static string NmapScanResultFile { get; } = "./Resources/NmapScanResult.xml";
    
    [Fact]
    public void SerializeNmapScanResult()
    {
        var nmapScanResult = Nmap.ConvertResult(NmapScanResultFile);
        Assert.NotNull(nmapScanResult);
    }
    
    [Fact]
    public void LocateNmap()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var path = Nmap.GetPathToNmap();
        Assert.NotEmpty(path);
        output.WriteLine(path);
    }

    [Fact]
    public void BuildPortArgumentWithPortList_3Ports_ReturnsValidArugument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var targets = new List<ITarget>
        {
            new IpAddress("127.0.0.1"),
            new IpSubnet("127.0.0.0", 24),
            new DnsAddress("localhost")
        };
        var targetList = Nmap.BuildTargetArgument(targets);
        Assert.Equal("127.0.0.1 127.0.0.0/24 localhost", targetList);
    }

    [Fact]
    public void BuildPortListTest()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var ports = new PortList { 80, 443, 8080 };
        var port = ports.ToString();
        Assert.Equal("-p 80,443,8080", port.ToString());
    }

    [Fact]
    public void BuildPortArgumentWithTopPorts_Valid_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var port = new TopPorts(1000);
        Assert.Equal("–top-ports 1000", port.ToString());
    }

    [Fact]
    public void BuildPortArgumentWithAllPorts_Valid_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var port = new AllPorts();
        Assert.Equal("-p-", port.ToString());
    }

    [Fact]
    public void BuildPortArgumentWithPortRange_Valid_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var port = new PortRange(1, 1000);
        Assert.Equal("-p 1-1000", port.ToString());
    }

    [Fact]
    public void BuildPortArgumentWithFastPortScan_Valid_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var port = new FastPortScan();
        Assert.Equal("-F", port.ToString());
    }

    [Fact]
    public void BuildNmapArgumentLocalHostPort80_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var scan = new ScanOptions();
        scan.Ports = new SinglePort(80);
        var target = new IpAddress("127.0.0.1");
        var argument = Nmap.BuildNmapArguments(target, scan);
        Assert.Equal("-sV -sC -p 80 127.0.0.1", argument);
    }

    [Fact]
    public void RunScanLocalHostPort80_ReturnsValidOutput()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var scan = new ScanOptions();
        scan.Ports = new SinglePort(80);
        scan.Output = new Output();
        scan.Output.Format = OutputFormat.XML;
        scan.Output.File = "test";
        var target = new IpAddress("127.0.0.1");
        nmap.Scan(target, scan);
    }

    [Fact]
    public void OutputXMLBuild_ReturnsValidOutput()
    {
        var output = new Output();
        output.Format = OutputFormat.XML;
        output.File = "test.xml";
        var argument = output.GetOutputArgument();
        Assert.Equal("--no-stylesheet -oX test.xml", argument);
    }
    
    [Fact]
    public void OutputGrepBuild_ReturnsValidOutput()
    {
        var output = new Output();
        output.Format = OutputFormat.Grepable;
        output.File = "test.grep";
        var argument = output.GetOutputArgument();
        Assert.Equal("-oG test.grep", argument);
    }
    
    [Fact]
    public void OutputNmapBuild_ReturnsValidOutput()
    {
        var output = new Output();
        output.Format = OutputFormat.Normal;
        output.File = "test.nmap";
        var argument = output.GetOutputArgument();
        Assert.Equal("-oN test.nmap", argument);
    }
    
    [Fact]
    public void ArgumentWithXMLOutputOnLocalhostPort80_ReturnsValidArgument()
    {
        var nmap = new ScanIT.Scanner.Nmap.Nmap();
        var scan = new ScanOptions();
        scan.Ports = new SinglePort(80);
        scan.Output = new Output();
        scan.Output.Format = OutputFormat.XML;
        scan.Output.File = "test.xml";
        var target = new IpAddress("127.0.0.1");
        var argument = Nmap.BuildNmapArguments(target, scan);
        Assert.Equal("--no-stylesheet -oX test.xml -sV -sC -p 80 127.0.0.1", argument);
    }
    
    [Fact]
    public void RunScanWithXMLOutputOnLocalhostPort80_ReturnsValidArgument()
    {
        var nmap = new Nmap();
        var scan = new ScanOptions
        {
            Ports = new PortList{80, 443, 22},
            Arguments = "-O --osscan-guess -sS -sV -sC -T4",
            Output = new Output
            {
                Format = OutputFormat.XML,
                File = "test.xml"
            }
        };
        var target = new TargetList { new DnsAddress("localhost") };
        var argument = Nmap.BuildNmapArguments(target, scan);
        Assert.Equal("--no-stylesheet -oX test.xml -O --osscan-guess -sS -sV -sC -T4 -p 80,443,22 localhost", argument);
        output.WriteLine(argument);
        nmap.ProgressHandler += (_, args) => output.WriteLine($"{args.Output}");
        nmap.Scan(target, scan);
    }
}