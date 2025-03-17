using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerCondition player))
        {
            StartCoroutine(DamageOverTime(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerCondition player))
        {
            StopAllCoroutines(); // 레이저에서 벗어나면 데미지 중단
        }
    }

    private IEnumerator DamageOverTime(PlayerCondition player)
    {
        while (true)
        {
            player.TakePhysicalDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval); // 지정한 간격마다 데미지 적용
        }
    }
}
