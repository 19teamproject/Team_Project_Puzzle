using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentStage = 1;
    public List<int> clearedStages = new();
}
