using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4_generate
{
    /// <summary>
    /// Interfaccia che mi permette di implementare i metodi generateIPv4 e generateSubnet
    /// </summary>
    internal interface IAddress
    {
        string generateIPv4();
        string generateSubnet();
    }
}
