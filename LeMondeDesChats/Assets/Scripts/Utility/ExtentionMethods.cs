using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ExtentionMethods
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        if (x is not null)
            vector.x = (float)x;
        if (y is not null)
            vector.y = (float)y;
        if (z is not null)
            vector.z = (float)z;

        return vector;
    }

    public static Vector3 DirectionTo(this Vector3 owner, Vector3 target, bool normalize = true)
    {
        var direction = target - owner;
        if (normalize)
            direction.Normalize();

        return direction;
    }

    public static Quaternion ToRotation(this Vector3 owner) => Quaternion.LookRotation(owner);

    public static Vector3 ClampMagnitude(this Vector3 vector, float maxLength) => Vector3.ClampMagnitude(vector, maxLength);
    public static Vector2 ClampMagnitude(this Vector2 vector, float maxLength) => Vector2.ClampMagnitude(vector, maxLength);

    public static Vector3 To3D(this Vector2 vector) => new Vector3(vector.x, 0f, vector.y);

    public static Vector3 MakeCameraBased(this Vector3 movement, Transform camera)
    {
        Vector3 forward = camera.forward.With(y: 0f).normalized;
        Vector3 right = camera.right.With(y: 0f).normalized;

        return forward * movement.z + right * movement.x;
    }

    public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);

    public static void Align(this Transform owner, Transform target, bool position, bool rotation)
    {
        if (position)
            owner.position = target.position;
        if (rotation)
            owner.rotation = target.rotation;
    }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    public static bool TryGetComponentInParent<T>(this Component component, out T result)
    {
        result = component.GetComponentInParent<T>();
        return result != null;
    }

    public static bool Contains(this LayerMask mask, int layer) => mask == (mask | (1 << layer));

    public static bool ScreenPick(this Camera camera, Vector2 position, out Vector2 point, float maxDistance, int mask)
    {
        point = Vector2.zero;
        var hit = Physics.Raycast(camera.ScreenPointToRay(position), out RaycastHit info, maxDistance, mask);
        if (hit)
            point = info.point;

        return hit;
    }
}
