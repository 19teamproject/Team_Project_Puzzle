using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public Material burnedMaterial;  // �ҿ� ź �� ����� material
    private bool isBurning = false;  // ���� �پ����� ����

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fire") && !isBurning)
        {
            StartCoroutine(BurnWood());
        }
    }

    private IEnumerator BurnWood()
    {
        isBurning = true;

        GetComponent<Renderer>().material = burnedMaterial;

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
