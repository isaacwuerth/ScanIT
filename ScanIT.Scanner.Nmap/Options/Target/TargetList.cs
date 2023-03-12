using System.Text;
using ScanIT.Scanner.Nmap.Options.Target;

namespace ScanIT.Scanner.Nmap;

public class TargetList: List<ITarget>, ITarget
{
    public override string ToString()
    {
        var targetList = new StringBuilder();
        foreach (var target in this)
        {
            if(targetList.Length > 0)
                targetList.Append(' ');
            targetList.Append(target);
        }

        return targetList.ToString();
    }
}