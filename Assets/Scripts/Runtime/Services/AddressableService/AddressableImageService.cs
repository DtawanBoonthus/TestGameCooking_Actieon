using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cooking.Services;

public class AddressableImageService : IAddressableImageService
{
    private sealed class Entry
    {
        public AsyncOperationHandle<Sprite> Handle;
        public int RefCount;
    }

    private readonly Dictionary<string, Entry> caches = new();

    public async UniTask<Sprite> LoadSpriteAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key is null or empty.", nameof(key));
        }

        if (caches.TryGetValue(key, out var existing))
        {
            await Addressables.ResourceManager.Acquire(existing.Handle);
            existing.RefCount++;
            return existing.Handle.Result;
        }

        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        await handle.ToUniTask(cancellationToken: cancellationToken);

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            throw new InvalidOperationException($"Failed to load sprite with key: {key}");
        }

        caches[key] = new Entry { Handle = handle, RefCount = 1 };
        return handle.Result;
    }

    public async UniTask PreloadAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
    {
        var tasks = new List<UniTask>();
        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("Preload sprite failed: key is null or empty.");
                continue;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            tasks.Add(LoadSpriteAsync(key, cancellationToken));
        }

        try
        {
            await UniTask.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Preload sprites failed: {ex.Message}");
            throw;
        }
    }

    public async UniTask PreloadByLabelAsync(string label, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            Debug.LogWarning("PreloadByLabel skipped: label is null or empty.");
            return;
        }

        var locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));
        await locationsHandle.Task;

        if (locationsHandle.Status != AsyncOperationStatus.Succeeded || locationsHandle.Result == null)
        {
            Debug.LogWarning($"Preload label '{label}' failed: cannot resolve locations.");
            return;
        }

        var tasks = new List<UniTask>();

        foreach (var location in locationsHandle.Result)
        {
            var key = location.PrimaryKey;
            tasks.Add(LoadSpriteAsync(key, cancellationToken));
        }

        try
        {
            await UniTask.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Preload sprite failed from label '{label}': {ex.Message}");
            throw;
        }
    }

    public Sprite? TryGetFromCache(string key)
    {
        return caches.TryGetValue(key, out var entry) && entry.Handle.IsValid()
            ? entry.Handle.Result
            : null;
    }

    public bool IsCached(string key) => caches.ContainsKey(key);

    public void Release(string key)
    {
        if (!caches.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"Key {key} not found in cache.");
            return;
        }

        entry.RefCount--;

        if (entry.RefCount <= 0)
        {
            if (entry.Handle.IsValid())
            {
                Addressables.Release(entry.Handle);
            }

            caches.Remove(key);
        }
        else
        {
            if (entry.Handle.IsValid())
            {
                Addressables.Release(entry.Handle);
            }
        }
    }

    public void ReleaseAll()
    {
        foreach (var cache in caches)
        {
            var entry = cache.Value;

            if (!entry.Handle.IsValid())
            {
                Debug.LogWarning($"Key {cache.Key} not found in cache.");
                continue;
            }

            for (var i = 0; i < entry.RefCount; i++)
            {
                Addressables.Release(entry.Handle);
            }
        }

        caches.Clear();
    }
}