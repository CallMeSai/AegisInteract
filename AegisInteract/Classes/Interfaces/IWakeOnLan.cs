using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AegisInteract.Classes.Interfaces
{
    public interface IWakeOnLan
    {
        string MacAddress { get; set; }
        Task DoWakeOnLan();

        event Action StateChanged; 
    }
}
