using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class TilemapLayerExtender : EditorWindow
{
    [MenuItem("Tools/Tilemap Layer Extender")]
    public static void ShowWindow()
    {
        GetWindow<TilemapLayerExtender>("타일맵 레이어 확장");
    }

    private void OnGUI()
    {
        GUILayout.Label("타일 레이어를 선택하고 버튼을 누르세요.", EditorStyles.boldLabel);

        if (GUILayout.Button("위로 확장"))
        {
            ExtendLayerUp();
        }
    }

    private void ExtendLayerUp()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("선택된 타일 레이어가 없습니다.");
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        GameObject duplicatedObject = Instantiate(selectedObject);

        if (duplicatedObject != null)
        {
            Undo.RegisterCreatedObjectUndo(duplicatedObject, "Extend Tile Layer Up");
            duplicatedObject.transform.position += Vector3.up; // Move up by 1 unit

            // Keep the duplicated object under the same parent
            duplicatedObject.transform.SetParent(selectedObject.transform.parent);
            
            // Increment the layer number in the name
            duplicatedObject.name = IncrementLayerName(selectedObject.name);

            Selection.activeGameObject = duplicatedObject; // Select the duplicated tile layer
        }
    }

    private string IncrementLayerName(string originalName)
    {
        Match match = Regex.Match(originalName, @"^(.*?)(\d+)$"); // Capture base name and last number

        if (match.Success)
        {
            string baseName = match.Groups[1].Value.Trim(); // Get base name
            int number = int.Parse(match.Groups[2].Value) + 1; // Increment number
            return $"{baseName} {number}"; // Return new name with incremented number
        }
        else
        {
            return $"{originalName} 1"; // If no number exists, add " 1"
        }
    }
}
