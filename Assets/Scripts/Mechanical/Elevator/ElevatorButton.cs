using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public ElevatorMechanic machanic;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            machanic.MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            machanic.MoveDown();
        }
    }
}
