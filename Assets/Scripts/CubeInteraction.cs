using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeInteraction : MonoBehaviour
{
    
    //큐브의 크기를 바꾸는 메서드
    public void ChangeScale(GameObject cube, Vector3 scaleDir, float scaleAmount) 
    {
        Vector3 newScale = cube.transform.localScale;
        newScale += scaleDir * scaleAmount; //원하는 방향으로 x또는 y축으로 원하는 만큼의 크기를 증가
        cube.transform.localScale = newScale;
    }


    // 큐브를 통해서 텔레포트를 할 수 있는 메서드
}
