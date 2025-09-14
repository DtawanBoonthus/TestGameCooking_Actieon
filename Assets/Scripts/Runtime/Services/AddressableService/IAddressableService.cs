using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cooking.Services;

public interface IAddressableService
{
    UniTask PreloadAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
    UniTask PreloadByLabelAsync(string label, CancellationToken cancellationToken);
    Sprite? TryGetFromCache(string key);
    bool IsCached(string key);
    void Release(string key);
    void ReleaseAll();
}