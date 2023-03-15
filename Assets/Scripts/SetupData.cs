using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SetupData
{
    //colors can not serializations
    public Dictionary<string, List<int>> setupColors;
    public Dictionary<string, int> setupIndex;
    public string teamName;
}
