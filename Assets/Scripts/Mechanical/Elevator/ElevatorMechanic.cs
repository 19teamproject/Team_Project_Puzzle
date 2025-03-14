using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMechanic : MonoBehaviour
{
    public void MoveUp()
    {
        transform.DOMoveY(2f, 2f);
    }

    public void MoveDown()
    {
        transform.DOMoveY(0f, 2f);
    }
}
