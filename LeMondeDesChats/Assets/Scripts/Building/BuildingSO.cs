using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Building")]
public class BuildingSO : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField, Min(0)] public int Wood { get; private set; }
    [field: SerializeField, Min(0)] public int Stone { get; private set; }
}
