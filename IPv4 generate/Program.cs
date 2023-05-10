using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4_generate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool[] arr = new bool[32];
            AddressGenerator ad;
            ConsoleKeyInfo s;
            Random r = new Random();
            bool ok = true;

            do
            {
                Console.Clear();
                ok = true;
                Console.WriteLine("Bit generati: ");
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = r.Next(0, 2) == 1;
                    Console.Write(arr[i]==true ? "1" : "0");
                }
                ad = new AddressGenerator(arr);
                Console.WriteLine();
                try
                {
                    Console.WriteLine($"Indirizzo ip completo di subnet generato: {ad.generateIPv4()}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Si è verificata un'eccezione: {ex.Message}");
                    ok = false;
                }

                if (ok)
                {
                    Console.WriteLine($"Subnet mask dell'indirizzo associato a AddressGenerator o generata randomicamente: {ad.generateSubnet()}\n");

                    Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}",
                    IPv4.Formatta("indirizzo ip", ad.Ip.IpString, ad.Ip.BinaryIpString),
                    IPv4.Formatta("subnet", ad.Ip.SubnetString, ad.Ip.BinarySubnetString),
                    IPv4.Formatta("netID", ad.Ip.NetIdIp.IpString, ad.Ip.NetIdIp.BinaryIpString),
                    IPv4.Formatta("broadcast", ad.Ip.BroadcastIp.IpString, ad.Ip.BroadcastIp.BinaryIpString),
                    IPv4.Formatta("cidr", ad.Ip.Cidr));
                }

                Console.WriteLine("Premere esc per uscire o premere invio per generare un'altro indirizzo..........");
                s = Console.ReadKey();
            } while (s.Key != ConsoleKey.Escape);
        }
    }
}
