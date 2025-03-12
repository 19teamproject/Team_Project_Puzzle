using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableObject : MonoBehaviour
{
    public Renderer targetRenderer; // 변경할 Renderer
    public Rigidbody rigid;
    public GameObject flameEffect,endEffect;
    private bool isBurning = false;  // 불이 붙었는지 여부
    public Color startColor = new Vector4(1,1,1,1);
    public Color endColor = new Vector4(0,0,0, 1);

    public float duration = 2f;

    public void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fire") && !isBurning)
        {
            StartCoroutine(BurnWood());
        }
    }

    private IEnumerator BurnWood()
    {
        rigid.isKinematic = true;
        isBurning = true;
        GameObject[] flame = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 myPos = transform.position;
            flame[i] = Instantiate(flameEffect,myPos,Quaternion.identity);
            flame[i].transform.SetParent(transform);
            flame[i].transform.position = new Vector3(myPos.x + Random.Range(0f, 1f), myPos.y + Random.Range(0f, 1f), myPos.z + Random.Range(0f, 1f));
        }

        

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetRenderer.material.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetRenderer.material.color = endColor;
        Instantiate(endEffect, transform.position, Quaternion.identity);
        Destroy(transform.parent.gameObject);
    }

}
