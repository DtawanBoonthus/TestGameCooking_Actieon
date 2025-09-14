using Cooking.Services;
using R3;
using VContainer;

namespace Cooking;

public class CookingPauseViewModel : ICookingPauseViewModel
{
    [Inject] private readonly IPauseService pauseService = null!;

    private readonly ReactiveProperty<bool> isShowPausePanel = new(false);
    public ReadOnlyReactiveProperty<bool> IsShowPausePanel => isShowPausePanel;

    public void Pause()
    {
        isShowPausePanel.Value = true;
        pauseService.Pause();
    }

    public void Resume()
    {
        pauseService.Resume();
        isShowPausePanel.Value = false;
    }
}