using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Vector3 Reflect(Vector3 incomingDirection, Vector3 normal)
    {
        return Vector3.Reflect(incomingDirection, normal);
    }
    
    // �ſ� ���� �ٸ� ��͵� ����� ���⿡ �߰�
}
