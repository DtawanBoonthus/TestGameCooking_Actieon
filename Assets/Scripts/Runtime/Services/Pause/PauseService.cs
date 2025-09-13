using R3;
using UnityEngine;

namespace Cooking.Services;

public class PauseService : IPauseService
{
    private readonly ReactiveProperty<bool> isPausing = new(false);
    public ReadOnlyReactiveProperty<bool> IsPausing => isPausing;

    public void Pause()
    {
        isPausing.Value = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPausing.Value = false;
        Time.timeScale = 1f;
    }
}