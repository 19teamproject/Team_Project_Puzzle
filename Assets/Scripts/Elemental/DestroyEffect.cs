using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    private void Start()
    {
        DestroyMe();
    }

    void DestroyMe()
    {
        Destroy(gameObject, 2f);
    }
}
