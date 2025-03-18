using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "StageList", menuName = "Scriptable Object/Stage List", order = 0)]
public class StageList : ScriptableObject
{
    public SceneAsset titleScene;
    public List<SceneAsset> stageScenes;

    /// <summary>
    /// 씬의 실제 이름을 반환
    /// </summary>
    /// <param name="index">스테이지 인덱스</param>
    /// <returns></returns>
    public string GetSceneName(int index)
    {
        if (index < 0 || index >= stageScenes.Count || stageScenes[index] == null)
        {
            Debug.LogError("씬 에셋이 없거나 잘못된 스테이지입니다!");
            return null;
        }

        return stageScenes[index].name;
    }
}