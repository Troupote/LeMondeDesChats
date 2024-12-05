using System;

[System.Serializable]
public abstract class EnumMask
{
    public int mask = int.MaxValue;
    public Enum type = default;
}

[System.Serializable]
public class EnumMask<T> : EnumMask where T : Enum
{
    public new T type = default;

    public bool IsValid(T value) => mask == (mask | (1 << Convert.ToInt32(value)));
}
