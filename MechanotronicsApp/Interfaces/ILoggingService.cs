using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanotronicsApp.Interfaces
{
    public interface ILoggingService
    {
        void LogInformation(string message);
        void LogError(string message, Exception? ex = null);
        void LogWarning(string message);
    }
}
