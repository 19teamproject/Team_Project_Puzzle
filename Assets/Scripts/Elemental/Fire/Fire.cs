using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour
{
    public GameObject fireEffect;  // 불이 붙을 이펙트

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Flammable")) // 가연성 오브젝트(ex.woodBlock)
        {

            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Ice"))  // 얼음을 녹임
        {
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Torch"))  // 횃불에 불 붙이기
        {
            other.GetComponent<Torch>().Ignite();
        }

    }

}
