using System.Collections.Generic;

public class GameObjectData
{
    public string m_Name { get; set; }
    public string m_TagString { get; set; }
    public int m_Layer { get; set; }
    public int m_IsActive { get; set; }
    public List<ComponentReference> m_Component { get; set; }
}

public class ComponentReference
{
    public FileId component { get; set; }
}

public class FileId
{
    public long fileID { get; set; }
}

public class TransformData
{
    public Vector3Data m_LocalPosition { get; set; }
    public QuaternionData m_LocalRotation { get; set; }
    public Vector3Data m_LocalScale { get; set; }
    public List<FileId> m_Children { get; set; }
    public FileId m_Father { get; set; }
}

public class Vector3Data
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class QuaternionData
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }
}

public class ColorData
{
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
    public float a { get; set; }
}

public class SpriteRendererData
{
    public FileId m_Sprite { get; set; }
    public ColorData m_Color { get; set; }
    public FileId m_Material { get; set; }
}

public class Rigidbody2DData
{
    public float m_Mass { get; set; }
    public float m_GravityScale { get; set; }
    public int m_BodyType { get; set; }
    public bool m_Simulated { get; set; }
    public Vector2Data m_Velocity { get; set; }
}

public class Vector2Data
{
    public float x { get; set; }
    public float y { get; set; }
}

public class BoxCollider2DData
{
    public bool m_UsedByComposite { get; set; }
    public Vector2Data m_Offset { get; set; }
    public Vector2Data m_Size { get; set; }
}

public class MonoBehaviourData
{
    public FileId m_GameObject { get; set; }
    public Dictionary<string, object> ScriptFields { get; set; } // catch-all for serialized fields
}

public class RectTransformData
{
    public Vector2Data m_AnchoredPosition { get; set; }
    public Vector2Data m_SizeDelta { get; set; }
    public Vector2Data m_Pivot { get; set; }
    public FileId m_Father { get; set; }
}