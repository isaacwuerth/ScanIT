using ScanIT.Scanner.Nmap.Options.Ports;

namespace ScanIT.Scanner.Nmap.Options;

public class ScanOptions
{
    public IPort Ports { get; set; } = new FastPortScan();
    public string Arguments { get; set; } = "-sV -sC";
    public Output Output { get; set; } = new Output();
}