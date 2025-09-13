using R3;

namespace Cooking.Services;

public interface IPauseService
{
    ReadOnlyReactiveProperty<bool> IsPausing { get; }
    void Pause();
    void Resume();
}