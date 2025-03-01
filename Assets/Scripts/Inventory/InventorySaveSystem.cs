using System.IO;
using UnityEngine;

public static class InventorySaveSystem
{
    private static string path => Path.Combine(Application.persistentDataPath, "inventory.json");

    public static void SaveData(InventoryData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public static InventoryData LoadData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<InventoryData>(json);
        }
        return new InventoryData();
    }
}