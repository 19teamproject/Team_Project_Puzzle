using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public Material burnedMaterial;  // 불에 탄 후 변경될 material
    private bool isBurning = false;  // 불이 붙었는지 여부

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
