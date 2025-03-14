using UnityEngine;
using NaughtyAttributes;

public enum CubeType
{
    None,
    Scale,
    Teleport,
    Jump
}

public enum ScaleDir
{
    X, Y, Z
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