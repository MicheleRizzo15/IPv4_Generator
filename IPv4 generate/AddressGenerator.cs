using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

/// <summary>
/// Classe addetta alla generazione di un indirizzo
/// </summary>
namespace IPv4_generate
{
    internal class AddressGenerator : IAddress
    {
        /// <summary>
        /// array di bool usato per ospitare l'indirizzo generato
        /// </summary>
        bool[] bit;
        ///<summary>
        ///variabile intera contenente il cidr
        /// </summary>
        int cidr;
        ///<summary>
        ///oggetto di tipo IPv4
        ///</summary>
        private IPv4 ip;

        #region costruttori

        /// <summary>
        /// Costruttore che accetta un array di bool con all'interno 
        /// </summary>
        /// <param name="bit"></param>
        public AddressGenerator(bool[] bit)
        {
            cidr = -1;
            this.bit = new bool[32];
            if (bit.Length == 32)
            {
                for (int i = 0; i < bit.Length; i++)
                {
                    this.bit[i] = bit[i];
                }
            }
            else
            {
                throw new Exception("Lunghezza indirizzo ip incorretta");
            }

            ip = null;
        }

        internal IPv4 Ip { get => ip; }

        #endregion

        /// <summary>
        /// Metodo implementato dall'interfaccia che mi permette di creare un indirizzo di tipo IPv4
        /// </summary>
        /// <returns></returns>
        public string generateIPv4()
        {
            if (ip == null)
            {
                cidr = IPv4.GetCidrFromSubnet(IPv4.StringToByte(generateSubnet()));
            }
            ip = new IPv4(bit, cidr);
            return ip.ToString();

        }
        /// <summary>
        /// Metodo implementato dall'interfaccia che mi permette di creare una subnet di tipo IPv4
        /// </summary>
        /// <returns></returns>
        public string generateSubnet()
        {
            Random r = new Random();
            int cidr = r.Next(1, 31);
            if (ip == null)
            {
                return IPv4.ByteToString(IPv4.GetSubnetFromCidrByte(cidr));
            }
            else
            {
                return IPv4.ByteToString(IPv4.GetSubnetFromCidrByte(this.ip.Cidr));
            }

        }
    }
}
