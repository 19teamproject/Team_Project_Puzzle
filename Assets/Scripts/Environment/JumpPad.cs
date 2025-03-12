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
    [SerializeField] private PadType type = PadType.Jump;

    private Tween launchCall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController playerController))
        {
            switch (type)
            {
                case PadType.Jump:
                    playerController.AddJumpForce(jumpDir.normalized * jumpForce);
                    break;

                case PadType.Launch:
                    launchCall = DOVirtual.DelayedCall(3f, () =>
                        playerController.AddJumpForce(jumpDir.normalized * jumpForce));
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        launchCall?.Kill();
    }

    private void OnCollisionExit(Collision collision)
    {
        launchCall.Kill();
    }
}
