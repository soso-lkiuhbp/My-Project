using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ConfigManager : Singleton<ConfigManager>
{
    public ConfigInfo ConfigInfo { get; private set; }
    public UserData UserData { get; private set; }
    public void Initial()
    {
        string path = Path.Combine(Application.streamingAssetsPath,"config.json");
        string json = File.ReadAllText(path);
        ConfigInfo = JsonConvert.DeserializeObject<ConfigInfo>(json);

        LoadUserData();
    }
    public void SaveUserData(UserData userData)
    {
        UserData = userData;
        string path = Path.Combine(Application.streamingAssetsPath, "userdata.json");
        string json = JsonConvert.SerializeObject(userData);
        File.WriteAllText(path, json);
    }

    void LoadUserData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "userdata.json");
        if (!File.Exists(path))
        {
            return ;
        }
        string json = File.ReadAllText(path);
        try
        {
            UserData userData = JsonConvert.DeserializeObject<UserData>(json);
            UserData= userData;
        }
        catch(System.Exception ex)
        {
            
        }
    }
}
