using System;
using System.Threading;
using Cooking.Services;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Cooking;

public class EnergyManager : IEnergyManager
{
    [Inject] private readonly IPlayerData playerData = null!;
    [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;
    [Inject] private readonly ISaveLoadService saveLoadService = null!;

    public void Init(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var intervalSec = gameConfigDatabase.EnergyRecoverySeconds;
        var perTick = Math.Max(1, gameConfigDatabase.EnergyRecoveryCount);
        var maxEnergy = gameConfigDatabase.MaxCookingEnergy;

        var lastActiveTime = saveLoadService.Load<DateTime>(SaveLoadKey.LAST_ACTIVE_TIME);

        if (lastActiveTime == default || lastActiveTime > now)
        {
            lastActiveTime = now;
        }

        var elapsedSec = (now - lastActiveTime).TotalSeconds;
        var ticks = intervalSec > 0 ? (int)Math.Floor(elapsedSec / intervalSec) : 0;

        if (ticks > 0 && playerData.Energy.CurrentValue < maxEnergy)
        {
            var recovered = ticks * perTick;
            playerData.IncreaseEnergy(recovered);
        }

        var lastTickAligned = lastActiveTime.AddSeconds(ticks * intervalSec);
        var initialDelay = TimeSpan.FromSeconds(Math.Clamp(intervalSec - Math.Max(0, (now - lastTickAligned).TotalSeconds), 0, intervalSec));

        RecoveryEnergyAsync(initialDelay, cancellationToken).Forget();
    }

    private async UniTask RecoveryEnergyAsync(TimeSpan initialDelay, CancellationToken cancellationToken)
    {
        var intervalSec = gameConfigDatabase.EnergyRecoverySeconds;
        var perTick = Math.Max(1, gameConfigDatabase.EnergyRecoveryCount);
        var maxEnergy = gameConfigDatabase.MaxCookingEnergy;

        if (intervalSec <= 0)
        {
            return;
        }

        if (initialDelay > TimeSpan.Zero)
        {
            await UniTask.Delay(initialDelay, cancellationToken: cancellationToken, ignoreTimeScale: true);
        }

        var interval = TimeSpan.FromSeconds(intervalSec);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (playerData.Energy.CurrentValue < maxEnergy)
            {
                var canGain = maxEnergy - playerData.Energy.CurrentValue;
                if (canGain > 0)
                {
                    playerData.IncreaseEnergy(Math.Min(perTick, canGain));
                }
            }

            await UniTask.Delay(interval, cancellationToken: cancellationToken, ignoreTimeScale: true);
        }
    }
}