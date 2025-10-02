using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class SaveSerial : Singleton<SaveSerial>
{
    public const string SaveFile = "/SaveData.json";

    public Dictionary<string, object> Data => JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonData);

    public string JsonData => File.Exists(Path) ? File.ReadAllText(Path) : string.Empty;
    public string Path => $"{Application.persistentDataPath}{SaveFile}";

    public void Increment(string key, int value)
    {
        Save(data =>
        {
            if (data.ContainsKey(key))
            {
                if (data[key] is long number)
                    data[key] = (int)number + value;
                else
                    throw new InvalidCastException($"Increment type is not Int64 - {data[key].GetType()}");
            }
            else
            {
                data.Add(key, value);
            }
        });
    }

    public void Save(string key, object value)
    {
        Save(data =>
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        });
    }

    public object Load(string key, object defaultValue = default)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        if (File.Exists(Path))
            data = Data;

        return data.ContainsKey(key)
                   ? data[key]
                   : defaultValue;
    }

    private void Save(Action<Dictionary<string, object>> onExistsCheck)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        if (File.Exists(Path))
            data = Data;

        onExistsCheck.Invoke(data);

        SerializeAndWrite(data);
    }

    private void SerializeAndWrite(Dictionary<string, object> data)
    {
        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Path, json);
    }

    public void ResetData()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }
}
