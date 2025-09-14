using System.Linq;
using System.Threading;
using Cooking.Services;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace Cooking;

public class PlayerManager : IAsyncStartable
{
    [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;
    [Inject] private readonly IPlayerData playerData = null!;
    [Inject] private readonly ISaveLoadService saveLoadService = null!;
    [Inject] private readonly IEnergyManager energyManager = null!;

    public async UniTask StartAsync(CancellationToken cancellation = default)
    {
        LoadPlayerData();
        energyManager.Init(cancellation);
        await UniTask.CompletedTask;
    }

    private void LoadPlayerData()
    {
        var loadPlayerData = saveLoadService.Load<MockPlayerData>(SaveLoadKey.PLAYER_DATA);

        if (loadPlayerData == null)
        {
            CreateNewPlayer();
            return;
        }

        playerData.InitializeData(loadPlayerData);
    }

    private void CreateNewPlayer()
    {
        playerData.InitializeData(gameConfigDatabase.PlayerIngredients.ToDictionary(x => x.IngredientId, x => x.Amount),
            gameConfigDatabase.PlayerEnergy);
    }
}