using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ȶ���� 

public class Torch : MonoBehaviour
{
    public GameObject fireEffect;
    private bool isLit = false;

    public void Ignite()
    {
        if(!isLit)
        {
            isLit = true;
            fireEffect.SetActive(true);
        }
    }
}
