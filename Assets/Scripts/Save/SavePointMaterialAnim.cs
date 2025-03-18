using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointMaterialAnim : MonoBehaviour
{
    [ColorUsage(true, true)][SerializeField] private Color color1;
    [ColorUsage(true, true)][SerializeField] private Color color2;
    [ColorUsage(true, true)][SerializeField] private Color color3;
    [SerializeField] private Material material;
    [SerializeField] private float duration;
    private int phase = 0;

    private void Start()
    {
        material.SetColor("_Color", color1);
    }

    private void Update()
    {
        if (phase == 0)
        {
            material.DOColor(color2, duration)
                .OnComplete(() => phase = 1);
        }
        else if (phase == 1)
        {
            material.DOColor(color3, duration)
                .OnComplete(() => phase = 2);
        }
        else if (phase == 2)
        {
            material.DOColor(color1, duration)
                .OnComplete(() => phase = 0);
        }
    }
}
