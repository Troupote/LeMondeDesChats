using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposeComponentAttribute : PropertyAttribute
{
    public Type Type { get; set; }

    public ExposeComponentAttribute(Type type) => Type = type;
}
