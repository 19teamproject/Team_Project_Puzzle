using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioClip audioClip;

    public event Action OnTakeDamage;
    public event Action OnDeath;

    public UICondition UICondition { get; set; }

    private Condition Health => UICondition.Health;
    private Condition Stamina => UICondition.Stamina;
    private bool isDead;

    private Coroutine speedBoostCoroutine;
    public float SpeedBoost { get; private set; }


    private void Update()
    {
        Stamina.Add(Stamina.PassiveValue * Time.deltaTime);

        if (Health.CurValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        Health.Add(amount);
    }

    public void ApplySpeedBoost(float amount, float duration)
    {
        if (isDead) return;

        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
            ResetSpeed();
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(amount, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float amount, float duration)
    {
        SpeedBoost += amount;

        yield return new WaitForSeconds(duration);

        ResetSpeed();
    }

    private void ResetSpeed()
    {
        SpeedBoost = 0f;
        speedBoostCoroutine = null;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        if (isDead) return;

        Health.Subtract(damageAmount);
        OnTakeDamage?.Invoke();
        SoundManager.Instance.PlayClip(audioClip);

        if (Health.CurValue <= 0f)
        {
            Die();
        }
    }
}