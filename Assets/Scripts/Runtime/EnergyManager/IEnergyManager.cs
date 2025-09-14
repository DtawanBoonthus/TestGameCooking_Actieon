using System.Threading;

namespace Cooking;

public interface IEnergyManager
{
    void Init(CancellationToken cancellationToken);
}