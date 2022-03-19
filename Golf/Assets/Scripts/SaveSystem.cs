using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    public static void SaveData(Manager manager) {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameData.b";

        FileStream fs = new FileStream(path, FileMode.Create);

        GameData data = new GameData(manager);

        bf.Serialize(fs,data);
        fs.Close();
    }

    public static GameData LoadData() {
        string path = Application.persistentDataPath + "/GameData.b";

        if (File.Exists(path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            GameData data = bf.Deserialize(fs) as GameData;
            
            fs.Close();

            return data;
        } else {
            return null;
        }

    }

    public static void DeleteSaves() {
        string path = Application.persistentDataPath + "/GameData.b";

        if (File.Exists(path)) {
            File.Delete(path);
        }
    }
}
