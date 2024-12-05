using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTransform : MonoBehaviour
{
    [SerializeField] private ForceTransformInfo position, rotation, scale;

    [System.Serializable]
    struct ForceTransformInfo
    {
        [field: SerializeField] public bool Force { get; private set; }
        [field: SerializeField] public bool Local { get; private set; }
        [field: SerializeField] public Vector3 Value { get; private set; }
    }

    private void LateUpdate()
    {
        if (position.Force)
        {
            if (position.Local)
                transform.localPosition = position.Value;
            else
                transform.position = position.Value;
        }

        if (rotation.Force)
        {
            if (rotation.Local)
                transform.localRotation = Quaternion.Euler(rotation.Value);
            else
                transform.rotation = Quaternion.Euler(rotation.Value);
        }

        if (scale.Force)
        {
            if (scale.Local)
                transform.localScale = scale.Value;
            else
                transform.SetGlobalScale(scale.Value);
        }
    }
}
