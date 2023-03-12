namespace ScanIT.Scanner.Nmap.Options.Ports;

public class PortRange: IPort
{
    public PortRange(int start, int end)
    {
        Start = start;
        End = end;
    }
    public int Start { get; set; }
    public int End { get; set; }
    public override string ToString()
    {
        return $"-p {Start}-{End}";
    }
}