using UnityEngine;

public class WaterSoundManager : MonoBehaviour
{
    private AudioSource playerAudioSource;
    private bool isInWater = false;

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            Debug.LogError("플레이어 오브젝트에 AudioSource 컴포넌트가 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && playerAudioSource != null)
        {
            // 물에 들어갔을 때 볼륨을 감소
            playerAudioSource.volume *= 0.1f;
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water") && playerAudioSource != null)
        {
            // 물에서 나왔을 때 볼륨을 원래대로 되돌림
            playerAudioSource.volume *= 10f;
            isInWater = false;
        }
    }

    private void Update()
    {
        // 플레이어가 물에 있는 동안 볼륨을 지속적으로 조절
        if (isInWater && playerAudioSource != null)
        {
            // 물에 있을 때 볼륨을 서서히 증가
            playerAudioSource.volume = Mathf.Lerp(playerAudioSource.volume, 1f, Time.deltaTime);
        }
    }
}
