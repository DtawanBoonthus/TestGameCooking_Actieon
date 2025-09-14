using Newtonsoft.Json;
using UnityEngine;

namespace Cooking.Services;

public class LocalSaveLoadService : ISaveLoadService
{
    public void Save(string key, object? value)
    {
        var json = JsonConvert.SerializeObject(value);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
        Debug.Log($"Save: {key}:{json}");
    }

    public T? Load<T>(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return default!;
        }

        var json = PlayerPrefs.GetString(key);
        Debug.Log($"Load: {key}:{json}");
        return JsonConvert.DeserializeObject<T>(json);
    }
}