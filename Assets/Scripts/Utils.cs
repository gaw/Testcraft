using UnityEngine;
using System.Collections;

public static class Utils 
{
    public static string VectorToHash(Vector3 v)
    {
        return string.Format("{0},{1},{2}", v.x, v.y, v.z);
    }
}
