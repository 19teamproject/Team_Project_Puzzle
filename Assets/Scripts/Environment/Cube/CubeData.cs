using UnityEngine;
using NaughtyAttributes;

public enum CubeType
{
    None,
    Scale,
    Teleport,
    Jump,
    SavePoint
}

public enum ScaleDir
{
    X, Minus_X,  // +X, -X
    Y, Minus_Y,  // +Y, -Y
    Z, Minus_Z   // +Z, -Z
}

[CreateAssetMenu(fileName = "Cube", menuName = "Scriptable Object/Cube Data", order = 1)]
public class CubeData : EnvironmentData
{
    public CubeType type;

    [ShowIf("type", CubeType.Scale)]
    [Header("Scale")]
    public ScaleDir scaleDir;

    [ShowIf("type", CubeType.Jump)]
    [Header("Jump")]
    public Vector3 jumpForce;
}