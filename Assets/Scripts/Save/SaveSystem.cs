using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath => Application.persistentDataPath + "/gameSave.json";

    /// <summary>
    /// 게임 데이터 저장
    /// </summary>
    /// <param name="data">세이브 데이터</param>
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved to: " + savePath);
    }

    /// <summary>
    /// 게임 데이터 불러오기
    /// </summary>
    /// <returns></returns>
    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        Debug.LogWarning("No save file found! Creating a new save.");
        return new SaveData(); // 새 세이브 데이터 생성
    }

    /// <summary>
    /// 세이브 데이터 초기화 (새 게임)
    /// </summary>
    public static void ResetGame()
    {
        File.Delete(savePath);
    }
}
