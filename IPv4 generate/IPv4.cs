using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

/// <summary>
/// Classe usata per effettuare i controlli su un indirizzo ip passato
/// </summary>
namespace IPv4_generate
{
    internal class IPv4
    {
        /// <summary>
        /// Array di bool privato rappresentate i bit dell'indirizzo: true corrisponde 1, false corrisponde 0.
        /// Gli ottetti sono rappresentati in un unico array (otteto 1 da 0 a 7, otteto 2 da 8 a 15, otteto 3 da 16 a 23, otteto 4 da 24 a 31).
        /// Ogni ottetto ha il bit più significativo in posizione più bassa, quindi la rappresentazione è Big-Endian
        /// </summary>
        bool[] ip;
        /// <summary>
        /// Array di bool privato rappresentate i bit della subnetmask: true corrisponde 1, false corrisponde 0.
        /// Gli ottetti sono rappresentati in un unico array (otteto 1 da 0 a 7, otteto 2 da 8 a 15, otteto 3 da 16 a 23, otteto 4 da 24 a 31).
        /// Ogni ottetto ha il bit più significativo in posizione più bassa, quindi la rappresentazione è Big-Endian.
        /// </summary>
        bool[] subnet;

        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo ip in notazione decimale come array di 4 byte.
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public byte[] Ip { get { return BoolToByte(ip); } }
        /// <summary>
        /// Proprietà solo in get che ritorna la subnetmask in notazione decimale come array di 4 byte.
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public byte[] Subnet { get { return BoolToByte(subnet); } }
        /// <summary>
        /// Proprietà solo in get che ritorna il cidr riferito alla subnetmask dell'indirizzo.
        /// Il cidr rappresenta il numero di 1 consecutivi all'interno della subnetmask
        /// </summary>
        public int Cidr { get { return GetCidrFromSubnet(subnet); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo di broadcast (indirizzo+relativa subnet) come IPv4.
        /// L'indirizzo di broadcast consente di raggiungere tutti gli host appartenenti alla rete corrente
        /// L'indirizzo di broadcast si calcola ponendo ad 1 tutti i bit riservati agli host dell'indirizzo ip data la subnet
        /// </summary>
        public IPv4 BroadcastIp { get { return GetBroadcast(ip, subnet); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo di broadcast (solo indirizzo) in notazione decimale come array di 4 byte.
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// L'indirizzo di broadcast consente di raggiungere tutti gli host appartenenti alla rete corrente
        /// L'indirizzo di broadcast si calcola ponendo ad 1 tutti i bit riservati agli host dell'indirizzo ip data la subnet
        /// </summary>
        public byte[] Broadcast { get { return BoolToByte(GetBroadcast(ip, subnet).ip); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo di netId (indirizzo+relativa subnet) come IPv4.
        /// L'indirizzo di netId consente di identificare la rete di appartenenza dell'host corrente
        /// L'indirizzo di netId si calcola facendo un and bit a bit tra la subnetmask e l'indirizzo ip richiesto
        /// </summary>
        public IPv4 NetIdIp { get { return GetNetId(ip, subnet); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo di netId (indirizzo+relativa subnet) come IPv4.
        /// L'indirizzo di netId consente di identificare la rete di appartenenza dell'host corrente
        /// L'indirizzo di netId si calcola facendo un and bit a bit tra la subnetmask e l'indirizzo ip richiesto
        /// </summary>
        public byte[] NetId { get { return BoolToByte(GetNetId(ip, subnet).ip); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo ip in notazione decimale come stringa 
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public string IpString { get { return ByteToString(BoolToByte(ip)); } }
        /// <summary>
        /// Proprietà solo in get che ritorna la subnetmask in notazione decimale come stringa
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public string SubnetString { get { return ByteToString(BoolToByte(subnet)); } }
        /// <summary>
        /// Proprietà solo in get che ritorna l'indirizzo ip in notazione binaria come stringa 
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public string BinaryIpString { get { return BitToString(ip); } }
        /// <summary>
        /// Proprietà solo in get che ritorna la subnetmask in notazione binararia come stringa
        /// Ogni posizione rappresenta un ottetto, rispetto alla notazione Big-Endian quindi, l'ottetto in posizione più bassa è quello con il peso maggiore
        /// </summary>
        public string BinarySubnetString { get { return BitToString(subnet); } }



        #region costruttori
        private IPv4()
        {
            ip = new bool[32];
            subnet = new bool[32];
        }
        /// <summary>
        /// Costruttore a cui viene passato un array di bool rappresentate un indirizzo ip, un array di bool rappresentante 
        /// la sua relativa subnetmask che verranno poi passate agli array di classe
        /// </summary>
        /// <param name="ip">array contenente l'indirizzo ip</param>
        /// <param name="sb">array contenente la subnet passata</param>
        /// <param name="isHost">bool che definisce se un indirizzo utilizzabile</param>
        public IPv4(bool[] ip, bool[] sb, bool isHost = false) : this()
        {
            if (ip.Length == 32)
            {

                bool test = false;
                int err;
                if (ControlloSubnet(sb, out err))
                {
                    if (isHost == true)
                    {
                        if (!(GetNetId(ip, sb).Equals(new IPv4(ip, sb))))
                        {
                            if (!(GetBroadcast(ip, sb).Equals(new IPv4(ip, sb))))
                            {
                                test = true;
                            }
                            else
                            {
                                SegnalazioneErrori(3);
                            }
                        }
                        else
                        {
                            SegnalazioneErrori(4);
                        }
                    }
                    else
                    {
                        test = true;
                    }
                    if (test == true)
                    {
                        for (int i = 0; i < ip.Length; i++)
                        {
                            this.ip[i] = ip[i];
                            subnet[i] = sb[i];
                        }
                    }
                }
                else
                {
                    SegnalazioneErrori(err);
                }

            }
            else
            {
                SegnalazioneErrori(5);
            }
        }
        /// <summary>
        /// Costruttore a cui viene passato un array di byte rappresentate un indirizzo ip, un array di byte rappresentante 
        /// la sua relativa subnetmask che verranno poi passate agli array di classe
        /// </summary>
        /// <param name="ip">array contenente l'indirizzo ip</param>
        /// <param name="sb">array contenente la subnetmask</param>
        /// <param name="isHost">bool che definisce se un indirizzo utilizzabile</param>
        public IPv4(byte[] ip, byte[] sb, bool isHost = false) : this(ByteToBool(ip), ByteToBool(sb), isHost)
        {

        }
        /// <summary>
        /// Costruttore a cui viene passato un array di byte rappresentate un indirizzo ip e il cidr dell'indirizzo
        /// </summary>
        /// <param name="ip">array contenente l'indirizzo ip</param>
        /// <param name="cidr">cidr dell'indirizzo</param>
        /// <param name="isHost">bool che definisce se un indirizzo utilizzabile</param>
        public IPv4(byte[] ip, int cidr, bool isHost = false) : this(ByteToBool(ip), GetSubnetFromCidrBool(cidr), isHost)
        {

        }
        /// <summary>
        /// Costruttore a cui viene passato un array di bool rappresentate un indirizzo ip e il cidr dell'indirizzo
        /// </summary>
        /// <param name="ip">array contenente l'indirizzo ip</param>
        /// <param name="cidr">cidr dell'indirizzo</param>
        /// <param name="isHost">bool che definisce se un indirizzo utilizzabile</param>
        public IPv4(bool[] ip, int cidr, bool isHost = false) : this(ip, GetSubnetFromCidrBool(cidr), isHost)
        {

        }
        #endregion

        #region override

        public override bool Equals(object obj)
        {
            for (int i = 0; i < 32; i++)
            {
                if (!(this.ip[i] == (obj as IPv4).ip[i] && this.subnet[i] == (obj as IPv4).subnet[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Override del ToString 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"{Ip[0]}.{Ip[1]}.{Ip[2]}.{Ip[3]}/{Cidr}");
        }
        #endregion


        #region metodi statici
        /// <summary>
        /// Metodo che ritorna un array di byte dato un array di bool
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static byte[] BoolToByte(bool[] arr)
        {
            if (arr != null && arr.Length != 32)
            {
                SegnalazioneErrori(5);
            }
            byte[] byteConvertito = new byte[] { 0, 0, 0, 0 };
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == true)
                {
                    byteConvertito[i / 8] += (byte)Math.Pow(2, 7 - (i % 8));
                }
            }
            return byteConvertito;
        }
        /// <summary>
        /// Metodo che ritorna un array di bool dato un array di byte per ottenere l'indirizzo passato 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool[] ByteToBool(byte[] arr)
        {
            if (arr != null && arr.Length != 4)
            {
                SegnalazioneErrori(6);
            }
            bool[] boolConvertito = new bool[32];
            byte valore;
            for (int i = 0; i < arr.Length; i++)
            {
                valore = arr[i];
                for (int j = 7; j >= 0; j--)
                {
                    //controllare se pari o dispari partendo da 7 fino a 0 (>=0)
                    boolConvertito[i * 8 + j] = valore % 2 == 1 ? true : false;
                    valore = (byte)((valore - (byte)(valore % 2)) / 2);
                }

            }
            return boolConvertito;
        }
        /// <summary>
        /// Metodo che restituisce un indirizzo da binario in stringa
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string BitToString(bool[] arr)
        {
            if (arr != null && arr.Length != 32)
            {
                SegnalazioneErrori(6);
            }
            string s = string.Empty;
            for (int i = 0; i < arr.Length; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    s += '.';
                }
                s += arr[i] == true ? "1" : "0";
            }
            return s;
        }
        /// <summary>
        /// Metodo che restituisce un indirizzo da stringa in binario
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool[] StringToBit(string s)
        {
            if (s == null && s.Trim().Length == 35)
            {
                SegnalazioneErrori(6);
            }
            bool[] arr = new bool[32];
            int pos=0;
            for (int i = 0; i < s.Trim().Length; i++)
            {
                if (s.Substring(i, 1) == "1")
                {
                    arr[pos] = true;
                    pos++;
                }
                else if (s.Substring(i, 1) == "0")
                {
                    arr[pos] = false;
                    pos++;
                }
            }

            return arr;
        }

        /// <summary>
        /// Metodo privato che controlla laa subnet passata, se non è corretta restituisce alla variabile err un valore intero
        /// usato come riferimento per il tipo di errore da segnalare
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool ControlloSubnet(bool[] sb, out int err)
        {
            err = -2;
            if (sb.Length == 32)
            {
                for (int i = 0; i < sb.Length - 1; i++)
                {
                    if (sb[i] == false && sb[i + 1] == true)
                    {
                        err = 1;
                        return false;
                    }

                }
                return true;
            }
            else
            {
                err = 5;
                return false;
            }

        }

        public static bool ControlloSubnet(bool[] sb)
        {
            int i;
            if (ControlloSubnet(sb, out i))
            {
                return true;
            }
            else
            {
                SegnalazioneErrori(i);
            }
            return false;
        }
        /// <summary>
        /// Metodo che restituisce una subnetmask dato un cidr in ingresso
        /// </summary>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool[] GetSubnetFromCidrBool(int cidr)
        {
            if (!(cidr > 0 && cidr < 31))
            {
                SegnalazioneErrori(2);
            }
            bool[] sbTmp = new bool[32];

            for (int i = 0; i < sbTmp.Length; i++)
            {
                if (i < cidr)
                {
                    sbTmp[i] = true;
                }
                else
                {
                    sbTmp[i] = false;
                }

            }

            return sbTmp;
        }

        public static byte[] GetSubnetFromCidrByte(int cidr)
        {
            return BoolToByte(GetSubnetFromCidrBool(cidr));
        }
        /// <summary>
        /// Metodo che restituisce un cidr data una subnet in ingresso
        /// </summary>
        /// <param name="sbTmp"></param>
        /// <returns></returns>
        public static int GetCidrFromSubnet(bool[] sbTmp)
        {
            int err;
            if (!ControlloSubnet(sbTmp, out err))
            {
                SegnalazioneErrori(err);
            }
            int cidr = 0;
            for (int i = 0; i < sbTmp.Length; i++)
            {
                if (sbTmp[i] == true)
                {
                    cidr++;
                }

            }
            return cidr;
        }

        public static int GetCidrFromSubnet(byte[] sbTmp)
        {
            return GetCidrFromSubnet(ByteToBool(sbTmp));
        }
        /// <summary>
        /// Metodo che ritorna il Broadcast di un indirizzo dato il suo ip e la sua subnet
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="subnet"></param>
        /// <returns></returns>
        public static IPv4 GetBroadcast(bool[] ip, bool[] subnet)
        {
            int err;
            if (!(ip.Length == 32) || !(subnet.Length == 32))
            {
                SegnalazioneErrori(5);
            }
            else if (ControlloSubnet(subnet, out err))
            {
                SegnalazioneErrori(err);
            }
            bool[] ipSupporto = new bool[32];
            for (int i = 0; i < subnet.Length; i++)
            {
                ipSupporto[i] = ((ip[i] == true && subnet[i] == true) || (subnet[i] == false));
            }
            return new IPv4(ipSupporto, subnet);
        }
        /// <summary>
        /// Metodo che ritorna il NetId di un indirizzo dato il suo ip e subnet
        /// </summary>
        /// <param name="ip">indirizzo passato</param>
        /// <param name="subnet">subnet passata</param>
        /// <returns></returns>
        public static IPv4 GetNetId(bool[] ip, bool[] subnet)
        {
            int err;
            if (!(ip.Length == 32) || !(subnet.Length == 32))
            {
                SegnalazioneErrori(5);
            }
            else if (ControlloSubnet(subnet, out err))
            {
                SegnalazioneErrori(err);
            }
            bool[] ipSupporto = new bool[32];
            for (int i = 0; i < subnet.Length; i++)
            {
                ipSupporto[i] = (ip[i] == true && subnet[i] == true);
            }
            return new IPv4(ipSupporto, subnet);
        }
        /// <summary>
        /// Metodo che esegue determinate eccezioni dato un determinato intero in ingresso
        /// </summary>
        /// <param name="cdErrore"></param>
        private static void SegnalazioneErrori(int cdErrore)
        {
            switch (cdErrore)
            {
                case 1: throw new Exception("Subnet non corretta"); break;
                case 2: throw new Exception("Cidr non corretto"); break;
                case 3: throw new Exception("Indirizzo uguale a broadcast"); break;
                case 4: throw new Exception("Indirizzo uguale a NetId"); break;
                case 5: throw new Exception("Lunghezza indirizzo non corretta"); break;
                case 6: throw new Exception("Lunghezza indirizzo in byte non corretta"); break;
                case 7: throw new Exception("Valore impossibile da convertire"); break;
            }
        }
        /// <summary>
        /// Metodo che ritorna l'array di byte suddiviso opportunamente dal carattere '.'
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string s)
        {
            string[] arr = s.Split('.');
            if (arr.Length != 4)
            {
                SegnalazioneErrori(5);
            }
            byte tmp;
            byte[] bTmp = new byte[4];
            for (int i = 0; i < arr.Length; i++)
            {
                if (byte.TryParse(arr[i], out tmp))
                {
                    bTmp[i] = tmp;
                }
                else
                {
                    SegnalazioneErrori(7);
                }
            }
            return bTmp;
        }
        /// <summary>
        /// Metodo che restituisce una stringa in formato 0.0.0.0 dato un array di byte passato in ingresso
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] arr)
        {
            if (arr.Length != 4)
            {
                SegnalazioneErrori(5);
            }
            return string.Format($"{arr[0]}.{arr[1]}.{arr[2]}.{arr[3]}");
        }

        /// <summary>
        /// Metodo usato per ottenere un output correttamente tabulato
        /// </summary>
        /// <param name="intestazione"></param>
        /// <param name="dec"></param>
        /// <param name="bin"></param>
        /// <returns></returns>
        public static string Formatta(string intestazione, string dec, string bin)
        {
            string finale = string.Empty;
            int spazi = 0;

            finale += intestazione + ":";
            spazi = 20 - finale.Length;
            for (int i = 0; i < spazi; i++)
            {
                finale += ' ';
            }

            finale += dec;
            spazi = 45 - finale.Length;
            for (int i = 0; i < spazi; i++)
            {
                finale += ' ';
            }

            finale += bin;

            return finale;
        }

        public static string Formatta(string intestazione, int intero)
        {
            return Formatta(intestazione, intero.ToString(), "");
        }
        #endregion
    }
}
