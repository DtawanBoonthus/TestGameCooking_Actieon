using R3;

namespace Cooking;

public interface ICookingPauseViewModel
{
    ReadOnlyReactiveProperty<bool> IsShowPausePanel { get; }
    void Pause();
    void Resume();
}