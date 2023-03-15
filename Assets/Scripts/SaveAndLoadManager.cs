using UnityEngine;
//for Dictionary
using System.Collections.Generic;
//for Serializable
using System;
//for file format
using System.Runtime.Serialization.Formatters.Binary;
//for file create/open/delete
using System.IO;
public class SaveAndLoadManager
{
    //save
    public void Save(SetupData setupData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/setup.sav", FileMode.Create);

        bf.Serialize(stream, setupData);
        stream.Close();
    }

    //load
    public SetupData Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/setup.sav", FileMode.Open);
        SetupData setupData = bf.Deserialize(stream) as SetupData;
        stream.Close();
        return setupData;
    }

    //delete save file
    public static void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/setup.sav"))
        {
            File.Delete(Application.persistentDataPath + "/setup.sav");
        }
    }
}


