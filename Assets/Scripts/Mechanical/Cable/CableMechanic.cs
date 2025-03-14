using HPhysic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableMechanic : MonoBehaviour
{
    public Connector startConnector;
    public Connector endConnector;

    public float energy = 0f;
    public float maxEnergy = 1f;
    public bool isCharging = false;

    private void Update()
    {
        if (startConnector.IsConnectedRight &&
            endConnector.IsConnectedRight &&
            energy < maxEnergy && !isCharging)
        {
            isCharging = true;
            StartCoroutine(ChargeEnergy());
        }
    }

    IEnumerator ChargeEnergy()
    {
        while (true)
        {
            Debug.Log(energy);
            energy += 0.2f;

            if (energy >= maxEnergy)
            {
                energy = maxEnergy;
                isCharging = false;
                yield break;
            }

            if (!startConnector.IsConnectedRight || !endConnector.IsConnectedRight)
            {
                isCharging = false;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
