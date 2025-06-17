using UnityEngine;

[ExecuteAlways]
public class PixelationController : MonoBehaviour
{
    public Material pixelationMaterial;  
    public float pixelSize = 4.0f;       

    private void Update()
    {
        if (pixelationMaterial == null) return;

        float width = Screen.width;
        float height = Screen.height;

        
        float resX = Mathf.Floor(width / pixelSize);
        float resY = Mathf.Floor(height / pixelSize);

        // Shader Graph의 Vector2 프로퍼티에 전달
        pixelationMaterial.SetVector("_PixelResolution", new Vector4(resX, resY, 0, 0));
    }
}