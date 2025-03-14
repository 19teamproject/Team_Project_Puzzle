using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("Data")]
    [SerializeField] protected CubeData data;
    [SerializeField] protected Outline outline;
    [SerializeField] 

    private Cube cube;

    private bool isMove = false;

    public string GetInteractPrompt()
    {
        string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">{data.displayName}</font> - {data.description}";
        return str;
    }

    public bool OnInteract()
    {
        if(cube != null)
        {
            cube.OnInteract(); //여기에 상호작용시 하고싶은 메서드 입력
            return true;
        }
        Debug.Log("Cube가 연결되지 않았습니다.");
        return false;
    }

    public virtual void SetOutline(bool show)
    {
        if (outline != null) outline.color = show ? 0 : 1;
    }
}
