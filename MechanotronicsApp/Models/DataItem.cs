using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanotronicsApp.Models
{
    public class DataItem
    {
        public DateTime Timestamp { get; set; }
        public string? CarName { get; set; }
        public string? DriverName { get; set; }
    }
}
