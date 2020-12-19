using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWorker
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}