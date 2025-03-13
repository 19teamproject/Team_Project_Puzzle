using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour
{
    public GameObject fireEffect;  // ���� ���� ����Ʈ
    public bool isFireActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Flammable")) // ������ ������Ʈ(ex.woodBlock)
        {
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Ice"))  // ������ ����
        {
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Water"))
        {
            ExtinguishFire();
        }

    }

    void ExtinguishFire()
    {
        if(isFireActive)
        {
            isFireActive = false;
            fireEffect.SetActive(false);
        }
    }

}
