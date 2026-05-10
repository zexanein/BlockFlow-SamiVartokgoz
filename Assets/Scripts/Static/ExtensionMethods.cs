using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }
}
