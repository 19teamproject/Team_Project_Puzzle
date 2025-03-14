using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ScaleDir
{
    X, Y, Z
}

public class CubeBoneScaler : MonoBehaviour
{
    [SerializeField] private List<Transform> xBones;
    [SerializeField] private List<Transform> yBones;
    [SerializeField] private List<Transform> zBones;
    [SerializeField] private float offset = 0.08f;
    [SerializeField] private float speed = 1f;

    private Vector3 currentScale = Vector3.one;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            ChangeScale(ScaleDir.X, 1f);

        if (Input.GetKeyDown(KeyCode.Y))
            ChangeScale(ScaleDir.Y, 1f);

        if (Input.GetKeyDown(KeyCode.Z))
            ChangeScale(ScaleDir.Z, 1f);
    }

    /// <summary>
    /// 큐브의 크기를 모양 변형 없이 늘리는 메서드
    /// </summary>
    /// <param name="dir">늘어날 방향 (X, Y, Z)</param>
    /// <param name="amount">늘어날 양 (기본값 = 1)</param>
    public void ChangeScale(ScaleDir dir, float amount = 1f)
    {
        switch (dir)
        {
            case ScaleDir.X:
                currentScale.x += amount;
                currentScale.x = Mathf.Max(currentScale.x, 0f);
                
                foreach (Transform t in xBones)
                    t.DOMoveX(currentScale.x - offset, speed);

                break;

            case ScaleDir.Y:
                currentScale.y += amount;
                currentScale.y = Mathf.Max(currentScale.y, 0f);

                foreach (Transform t in yBones)
                    t.DOMoveY(currentScale.y - offset, speed);

                break;

            case ScaleDir.Z:
                currentScale.z += amount;
                currentScale.z = Mathf.Max(currentScale.z, 0f);

                foreach (Transform t in zBones)
                    t.DOMoveZ(currentScale.z - offset, speed);

                break;
        }
    }
}
