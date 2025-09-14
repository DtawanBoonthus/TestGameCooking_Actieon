namespace Cooking.Services;

public interface ISaveLoadService
{
    void Save(string key, object? value);
    T? Load<T>(string key);
}