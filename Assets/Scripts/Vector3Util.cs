using UnityEngine;

public class Vector3Util
{
    public static bool compareVector3(Vector3 a, Vector3 b, float error)
    {
        return Mathf.Abs(a.x - b.x) < error
               && Mathf.Abs(a.y - b.y) < error
               && Mathf.Abs(a.z - b.z) < error;
    }
}
