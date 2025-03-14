using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class ScalableCube : Cube //상호작용시 크기의 형태가 변하는 큐브
{
    public void ChangeScale(Vector3 scaleDir, float scaleAmount)
    {
        //transform.localScale += scaleDir * scaleAmount; //원하는 방향으로 x또는 y축으로 원하는 만큼의 크기를 증가
        transform.DOScale(transform.localScale + scaleAmount * scaleDir, 1f);
    }

    // public override void HandleInteraction(GameObject player)
    // {
    //     if (gameObject.CompareTag("XCube"))
    //     {
    //         ChangeScale(new Vector3(1, 0, 0), 1f);
    //     }
    //     else if (gameObject.CompareTag("YCube"))
    //     {
    //         ChangeScale(new Vector3(0, 1, 0), 1f);
    //     }
    // }
}

