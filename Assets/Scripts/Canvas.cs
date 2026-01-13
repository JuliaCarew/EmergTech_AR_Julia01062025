using UnityEngine;

public class Canvas : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 512;
    [SerializeField] private Color defaultColor = Color.white;
    
    private Texture2D paintTexture;
    private Material canvasMaterial;
    private Renderer canvasRenderer;
    private MeshFilter meshFilter;
    
    void Start()
    {
        InitializeCanvas();
    }
    
    private void InitializeCanvas()
    {
        canvasRenderer = GetComponent<Renderer>();
        if (canvasRenderer == null)
        {
            Debug.LogError("Canvas: Renderer component not found");
            return;
        }
        
        meshFilter = GetComponent<MeshFilter>();
        
        paintTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        
        // fill texture with default color
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = defaultColor;
        }
        paintTexture.SetPixels(pixels);
        paintTexture.Apply();
        
        // apply texture to material
        canvasMaterial = canvasRenderer.material;
        canvasMaterial.mainTexture = paintTexture;
    }
    
    public void PaintAtPosition(Vector3 worldPosition, Color color, int size)
    {
        if (paintTexture == null || canvasRenderer == null)
            return;
        
        Vector2 uv = WorldToUV(worldPosition);
        PaintAtUV(uv, color, size);
    }
    
    // Paints on the canvas at the given UV coordinates
    public void PaintAtUV(Vector2 uv, Color color, int size)
    {
        if (paintTexture == null)
            return;
        
        // clamp UV coordinates
        uv.x = Mathf.Clamp01(uv.x);
        uv.y = Mathf.Clamp01(uv.y);
        
        // convert UV to pixel coordinates
        int pixelX = Mathf.RoundToInt(uv.x * textureWidth);
        int pixelY = Mathf.RoundToInt(uv.y * textureHeight);
        
        // paint a circle of pixels
        int halfSize = size / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                int px = pixelX + x;
                int py = pixelY + y;
                
                // check if pixel is within texture bounds
                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= halfSize)
                    {
                        // blend color with existing pixels
                        Color existingColor = paintTexture.GetPixel(px, py);
                        float alpha = color.a;
                        Color finalColor = Color.Lerp(existingColor, color, alpha);
                        paintTexture.SetPixel(px, py, finalColor);
                    }
                }
            }
        }
        
        paintTexture.Apply();
    }
    
    // converts a world position to UV coordinates on the canvas
    private Vector2 WorldToUV(Vector3 worldPosition)
    {
        // transform world position to local space
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);
        
        Bounds bounds = canvasRenderer.bounds;
        Vector3 worldSize = bounds.size;
        
        // Convert world size to local size 
        Vector3 localSize = new Vector3(
            worldSize.x / transform.lossyScale.x,
            worldSize.y / transform.lossyScale.y,
            worldSize.z / transform.lossyScale.z
        );
        
        // convert local position to UV coordinates (0-1)
        float u = (localPos.x / localSize.x) + 0.5f;
        float v = (localPos.y / localSize.y) + 0.5f;
        
        Debug.Log($"Calculated UV: ({u}, {v})");
        
        return new Vector2(u, v);
    }
    
    public void ClearCanvas()
    {
        if (paintTexture == null)
            return;
        
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = defaultColor;
        }
        paintTexture.SetPixels(pixels);
        paintTexture.Apply();
    }
}