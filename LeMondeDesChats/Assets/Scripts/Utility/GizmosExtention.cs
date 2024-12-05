using UnityEditor;
using UnityEngine;

public static class GizmosExtention
{
    public static void DrawWireCircle(Vector3 position, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.DrawWireDisc(position, Vector3.forward, radius);
#endif
    }

    public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
    {
#if UNITY_EDITOR
        Handles.DrawWireArc(center, normal, from, angle, radius);
#endif
    }

    public static void DrawwireArc(Vector3 center, Quaternion rotation, float radius, float angle, bool drawLines = true)
    {
#if UNITY_EDITOR
        Handles.DrawWireArc(
            center,
            rotation * Vector3.up,
            rotation * (Quaternion.Euler(0f, -angle / 2, 0) * (Vector3.forward * radius)),
            angle,
            radius);

        if (drawLines)
        {
            Gizmos.DrawLine(center, center + rotation * (Quaternion.Euler(0f, angle / 2, 0f) * (Vector3.forward * radius)));
            Gizmos.DrawLine(center, center + rotation * (Quaternion.Euler(0f, -angle / 2, 0f) * (Vector3.forward * radius)));
        }
#endif
    }
}
