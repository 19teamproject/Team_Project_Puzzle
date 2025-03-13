using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour
{
    public GameObject fireEffect;  // ���� ���� ����Ʈ

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
        else if(other.CompareTag("Torch"))  // ȶ�ҿ� �� ���̱�
        {
            other.GetComponent<Torch>().Ignite();
        }

    }

}
