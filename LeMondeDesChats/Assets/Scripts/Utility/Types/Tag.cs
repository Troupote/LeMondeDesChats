[System.Serializable]
public class Tag
{
    public string stringTag  = "Untagged";

    public static implicit operator string(Tag tag) => tag.stringTag;
}
