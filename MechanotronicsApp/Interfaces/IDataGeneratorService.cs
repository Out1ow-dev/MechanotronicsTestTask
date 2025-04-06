using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MechanotronicsApp.Interfaces
{
    public interface IDataGeneratorService : IDisposable
    {
        Task StartCarGenerationAsync(CancellationToken cancellationToken);
        Task StartDriverGenerationAsync(CancellationToken cancellationToken);
        Task StopCarGeneration();
        Task StopDriverGeneration();
    }
}
