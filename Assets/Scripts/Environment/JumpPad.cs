using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using StarterAssets;

public enum PadType
{
    Jump,
    Launch
}

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 jumpDir = Vector3.up;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private AudioClip[] jumpAudioClips;
    [Range(0, 1)]
    [SerializeField] private float jumpAudioVolume;
    // [SerializeField] private float delay = 1f;
    // [SerializeField] private PadType type = PadType.Jump;

    private Tween launchCall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController playerController))
        {
            // switch (delay)
            // {
            //     case 0f:
            //         playerController.AddJumpForce(jumpDir.normalized * jumpForce);
            //         if (jumpAudioClips.Length > 0)
            //         {
            //             var index = Random.Range(0, jumpAudioClips.Length);
            //             AudioSource.PlayClipAtPoint(jumpAudioClips[index], transform.position, jumpAudioVolume);
            //         }
            //         break;

            //     default:
            //         if (jumpAudioClips.Length > 0)
            //         {
            //             var index = Random.Range(0, jumpAudioClips.Length);
            //             AudioSource.PlayClipAtPoint(jumpAudioClips[index], transform.position, jumpAudioVolume);
            //         }
            //         launchCall = DOVirtual.DelayedCall(delay, () => 
            //         {
            //             playerController.AddJumpForce(jumpDir.normalized * jumpForce);
            //         });
            //         break;
            // }

            if (jumpAudioClips.Length > 0)
            {
                var index = Random.Range(0, jumpAudioClips.Length);
                AudioSource.PlayClipAtPoint(jumpAudioClips[index], transform.position, jumpAudioVolume);
            }
            launchCall = DOVirtual.DelayedCall(0.1f, () => 
            {
                playerController.AddJumpForce(jumpDir.normalized * jumpForce);
            });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        launchCall?.Kill();
    }
}
