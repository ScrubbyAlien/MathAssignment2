using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public static class VectorExtensions
{
    public static Vector4 ProjectOnto(this Vector4 v1, Vector4 v2) {
        return (v1.Dot(v2) / v2.Dot(v2)) * v2;
    }

    public static Vector3 ProjectOnto(this Vector3 v1, Vector3 v2) {
        return (v1.Dot(v2) / v2.Dot(v2)) * v2;
    }

    public static Vector4 Normalized(this Vector4 v) {
        return v / v.Magnitude();
    }
    public static Vector3 Normalized(this Vector3 v) {
        return v / v.Magnitude();
    }

    public static float Magnitude(this Vector4 v) {
        return Mathf.Sqrt(SquareMagnitude(v));
    }
    public static float Magnitude(this Vector3 v) {
        return Mathf.Sqrt(SquareMagnitude(v));
    }

    public static float SquareMagnitude(this Vector4 v) {
        return v.Dot(v);
    }

    public static float SquareMagnitude(this Vector3 v) {
        return v.Dot(v);
    }

    public static float Dot(this Vector4 v1, Vector4 v2) {
        return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z) + (v1.w * v2.w);
    }

    public static float Dot(this Vector3 v1, Vector3 v2) {
        return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
    }

    public static Vector4 Cross(this Vector4 v1, Vector4 v2) {
        float x = v1.y * v2.z - v1.z * v2.y;
        float y = v1.z * v2.x - v1.x * v2.z;
        float z = v1.x * v2.y - v1.y * v2.x;
        float w = 0;
        return new Vector4(x, y, z, w);
    }

    public static Vector3 Cross(this Vector3 v1, Vector3 v2) {
        float x = v1.y * v2.z - v1.z * v2.y;
        float y = v1.z * v2.x - v1.x * v2.z;
        float z = v1.x * v2.y - v1.y * v2.x;
        return new Vector3(x, y, z);
    }

    public static bool IsZeroVector(this Vector4 v) {
        if (!Mathf.Approximately(v.x, 0)) return false;
        if (!Mathf.Approximately(v.y, 0)) return false;
        if (!Mathf.Approximately(v.z, 0)) return false;
        return true;
    }
    public static bool IsZeroVector(this Vector3 v) {
        if (!Mathf.Approximately(v.x, 0)) return false;
        if (!Mathf.Approximately(v.y, 0)) return false;
        if (!Mathf.Approximately(v.z, 0)) return false;
        return true;
    }

    public static Quaternion Conjugate(this Quaternion q) {
        return new Quaternion(-q.x, -q.y, -q.z, q.w);
    }
}