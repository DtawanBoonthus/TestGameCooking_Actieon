using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cooking.Services;

public interface IAddressableImageService : IAddressableService
{
    UniTask<Sprite> LoadSpriteAsync(string key, CancellationToken cancellationToken);
}