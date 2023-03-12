using System.Text;

namespace ScanIT.Scanner.Nmap.Options.Ports;

public class PortList: List<int>, IPort
{
    public override string ToString()
    {
        var portList = new StringBuilder();
        foreach (var port in this)
        {
            if(portList.Length > 0)
                portList.Append(',');
            portList.Append(port);
        }

        return $"-p {portList}";
    }
}