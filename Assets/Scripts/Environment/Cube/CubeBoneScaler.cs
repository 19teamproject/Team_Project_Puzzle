using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class CubeBoneScaler : MonoBehaviour
{
    [SerializeField] private Transform armature;
    [SerializeField] private List<Transform> xBones;
    [SerializeField] private List<Transform> yBones;
    [SerializeField] private List<Transform> zBones;
    [SerializeField] private float maxScale = 3f;
    [SerializeField] private float offset = 0.08f;
    [SerializeField] private float speed = 1f;

    [Space(10f)]
    [SerializeField] private BoxCollider boxCollider;

    private List<Transform> allBones = new();
    private List<Transform> xNegBones;
    private List<Transform> yNegBones;
    private List<Transform> zNegBones;

    private int xDir = 1;
    private int yDir = 1;
    private int zDir = 1;

    private Vector3 currentScale = Vector3.one;

    [Foldout("Debug")]
    [SerializeField] private float xScale = 1f;
    [Foldout("Debug")]
    [SerializeField] private float yScale = 1f;
    [Foldout("Debug")]
    [SerializeField] private float zScale = 1f;

    [Button]
    private void DebugUpdateScale()
    {
        DebugUpdateScale(xScale, yScale, zScale);
    }

    private void DebugUpdateScale(float x, float y, float z)
    {
        foreach (Transform t in xBones)
            t.localPosition = new(x - offset, t.localPosition.y, t.localPosition.z);

        foreach (Transform t in yBones)
            t.localPosition = new(t.localPosition.x, y - offset, t.localPosition.z);

        foreach (Transform t in zBones)
            t.localPosition = new(t.localPosition.x, t.localPosition.y, z - offset);

        boxCollider.center = new Vector3(x, y, z) / 2f;
        boxCollider.size = new Vector3(x, y, z);
    }

    private void Start()
    {
        int childCount = armature.childCount;

        for (int i = 0; i < childCount; i++)
        {
            allBones.Add(armature.GetChild(i));
        }

        xNegBones = allBones.Except(xBones).ToList();
        yNegBones = allBones.Except(yBones).ToList();
        zNegBones = allBones.Except(zBones).ToList();
    }

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
                currentScale.x += amount * xDir;

                if (currentScale.x >= maxScale)
                {
                    currentScale.x = maxScale;
                    xDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.x <= 1f)
                {
                    currentScale.x = 1f;
                    xDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in xBones)
                    t.DOLocalMoveX(currentScale.x - offset, speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(currentScale.x / 2f, boxCollider.center.y, boxCollider.center.z), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(currentScale.x, boxCollider.size.y, boxCollider.size.z), speed);
                break;

            case ScaleDir.Minus_X:
                currentScale.x += amount * xDir;

                if (currentScale.x >= maxScale)
                {
                    currentScale.x = maxScale;
                    xDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.x <= 1f)
                {
                    currentScale.x = 1f;
                    xDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in xNegBones)
                    t.DOLocalMoveX(1f - (currentScale.x - offset), speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(1f - (currentScale.x / 2f), boxCollider.center.y, boxCollider.center.z), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(currentScale.x, boxCollider.size.y, boxCollider.size.z), speed);
                break;

            case ScaleDir.Y:
                currentScale.y += amount * yDir;

                if (currentScale.y >= maxScale)
                {
                    currentScale.y = maxScale;
                    yDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.y <= 1f)
                {
                    currentScale.y = 1f;
                    yDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in yBones)
                    t.DOLocalMoveY(currentScale.y - offset, speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(boxCollider.center.x, currentScale.y / 2f, boxCollider.center.z), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(boxCollider.size.x, currentScale.y, boxCollider.size.z), speed);
                break;

            case ScaleDir.Minus_Y:
                currentScale.y += amount * yDir;

                if (currentScale.y >= maxScale)
                {
                    currentScale.y = maxScale;
                    yDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.y <= 1f)
                {
                    currentScale.y = 1f;
                    yDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in yNegBones)
                    t.DOLocalMoveY(1f - (currentScale.y - offset), speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(boxCollider.center.x, 1f - (currentScale.y / 2f), boxCollider.center.z), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(boxCollider.size.x, currentScale.y, boxCollider.size.z), speed);
                break;

            case ScaleDir.Z:
                currentScale.z += amount * zDir;

                if (currentScale.z >= maxScale)
                {
                    currentScale.z = maxScale;
                    zDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.z <= 1f)
                {
                    currentScale.z = 1f;
                    zDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in zBones)
                    t.DOLocalMoveZ(currentScale.z - offset, speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(boxCollider.center.x, boxCollider.center.y, currentScale.z / 2f), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(boxCollider.size.x, boxCollider.size.y, currentScale.z), speed);
                break;

            case ScaleDir.Minus_Z:
                currentScale.z += amount * zDir;

                if (currentScale.z >= maxScale)
                {
                    currentScale.z = maxScale;
                    zDir = -1; // 감소 방향으로 변경
                }
                else if (currentScale.z <= 1f)
                {
                    currentScale.z = 1f;
                    zDir = 1; // 증가 방향으로 변경
                }

                foreach (Transform t in zNegBones)
                    t.DOLocalMoveZ(1f - (currentScale.z - offset), speed);

                DOTween.To(() => boxCollider.center, x => boxCollider.center = x, new Vector3(boxCollider.center.x, boxCollider.center.y, 1f - (currentScale.z / 2f)), speed);
                DOTween.To(() => boxCollider.size, x => boxCollider.size = x, new Vector3(boxCollider.size.x, boxCollider.size.y, currentScale.z), speed);
                break;
        }
    }
}
