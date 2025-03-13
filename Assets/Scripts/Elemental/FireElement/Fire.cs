using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour
{
    public GameObject fireEffect;  // 불이 붙을 이펙트
    public bool isFireActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFireActive) return;  // 이미 꺼진 불이면 아무 것도 하지 않음

        if (other.CompareTag("Flammable")) // 가연성 오브젝트(ex.woodBlock)
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Ice"))  // 얼음을 녹임
        {
            Destroy(other.gameObject);
        }
    }
}
