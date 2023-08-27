using System.Linq;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using AegisInteract.Classes.Interfaces;
using System.Collections.Generic;
using Microsoft.MobileBlazorBindings;

namespace AegisInteract.Classes.State
{
    public class WakeOnLanState : IWakeOnLan
    {
        public string MacAddress{ get; set; }
        public event Action StateChanged;

        public WakeOnLanState() { }

        public async Task DoWakeOnLan()
        {
            var result = await SendWakeOnLan2(PhysicalAddress.Parse(MacAddress));

            return;

            /* byte[] magicPacket = BuildMagicPacket(macAddress);
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
                n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up))
            {
                var iPInterfaceProperties = networkInterface.GetIPProperties();
                var addresses = iPInterfaceProperties.MulticastAddresses;
                foreach (var multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses)
                {
                    IPAddress multicastIpAddress = multicastIPAddressInformation.Address;

                    if (multicastIpAddress.AddressFamily != AddressFamily.InterNetworkV6) continue;

                    int result2 = 0;

                    var unicastIPAddressInformation2 = iPInterfaceProperties.UnicastAddresses.Where((u) =>
    u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
                    if (unicastIPAddressInformation2 != null)
                    {
                        result = await SendWakeOnLan(unicastIPAddressInformation2.Address, multicastIpAddress, magicPacket);
                    }
                    if (result > 0) continue;


                    Console.WriteLine(multicastIPAddressInformation.Address.ToString());
                    var unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
                        u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
                    if (unicastIPAddressInformation != null)
                    {
                        await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
                    }
                } 
            }*/ 
        }
        static byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
        {
            macAddress = Regex.Replace(macAddress, "[: -]", "");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            }

            IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6); //First 6 times 0xff
            IEnumerable<byte> data = Enumerable.Repeat(macBytes, 16).SelectMany(m => m); // then 16 times MacAddress
            return header.Concat(data).ToArray();
        }

        async Task<int> SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
        {
            try
            {
                using var client = new UdpClient(new IPEndPoint(localIpAddress, 0));
                return await client.SendAsync(magicPacket, magicPacket.Length, new IPEndPoint(multicastIpAddress, 9));
            }
            catch (Exception ex)
            {
                Console.WriteLine("fucked", ex);
                return -1;
            }
        }

        async Task<int> SendWakeOnLan2(PhysicalAddress target)
        {
            var header = Enumerable.Repeat(byte.MaxValue, 6);
            var data = Enumerable.Repeat(target.GetAddressBytes(), 16).SelectMany(mac => mac);

            var magicPacket = header.Concat(data).ToArray();

            using var client = new UdpClient();

            return await client.SendAsync(magicPacket, magicPacket.Length, new IPEndPoint(IPAddress.Broadcast, 9));
        }
    }
}
