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
    [Header("Jump")]
    [SerializeField] private Vector3 jumpDir = Vector3.up;
    [SerializeField] private float jumpForce = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] jumpAudioClips;
    [Range(0, 1)]
    [SerializeField] private float jumpAudioVolume;

    [Header("Color")]
    [ColorUsage(true, true)][SerializeField] private Color baseColor;
    [ColorUsage(true, true)][SerializeField] private Color activeColor;

    // [SerializeField] private float delay = 1f;
    // [SerializeField] private PadType type = PadType.Jump;

    private MeshRenderer meshRenderer;
    private Material[] materials;
    // private MaterialPropertyBlock materialPropertyBlock;

    private Tween launchCall;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        // materialPropertyBlock = new MaterialPropertyBlock();

        // meshRenderer.GetPropertyBlock(materialPropertyBlock, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ThirdPersonController playerController))
        {
            if (jumpAudioClips.Length > 0)
            {
                var index = Random.Range(0, jumpAudioClips.Length);
                AudioSource.PlayClipAtPoint(jumpAudioClips[index], transform.position, jumpAudioVolume);
            }
            launchCall = DOVirtual.DelayedCall(0.1f, () => 
            {
                playerController.AddJumpForce(jumpDir.normalized * jumpForce);
            });

            // ChangeMaterialColor(activeColor, 0.2f);
            ActivateJumpPad(activeColor, 0.2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        launchCall?.Kill();

        // ChangeMaterialColor(activeColor, 0.5f);
        DOVirtual.DelayedCall(0.2f, () => ActivateJumpPad(baseColor, 0.5f));
    }

    // private void ChangeMaterialColor(Color targetColor, float duration)
    // {
    //     Color startColor = materialPropertyBlock.HasProperty("_Color") ? materialPropertyBlock.GetColor("_Color") : Color.white;

    //     DOTween.To(() => startColor, x => startColor = x, targetColor, duration)
    //         .SetEase(Ease.OutCubic)
    //         .OnUpdate(() => {
    //             materialPropertyBlock.SetColor("_Color", startColor);
    //             meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
    //         });
    // }

    public void ActivateJumpPad(Color targetColor, float duration)
    {
        // DOTween.To(() => materials[1].GetColor("_Color"), x => SetJumpPadColor(x), targetColor, duration)
        //     .SetEase(Ease.OutQuad);

        materials[1].DOColor(targetColor, duration);
    }

    // public void SetJumpPadColor(Color color)
    // {
    //     materials[1] = new Material(materials[1]);  // 개별 인스턴스 복사
    //     materials[1].SetColor("_Color", color);     // "_Color" 변경
    //     meshRenderer.materials = materials;         // 변경된 머티리얼 적용
    // }
}
