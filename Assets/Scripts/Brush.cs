using UnityEngine;
using UnityEngine.InputSystem;

public enum BrushControlMode
{
    ScreenTouch,  
}

public class Brush : MonoBehaviour
{
    [Header("Control Mode")]
    [SerializeField] private BrushControlMode controlMode = BrushControlMode.ScreenTouch;
    
    [Header("Brush Settings")]
    [SerializeField] private Color brushColor = Color.black;
    [SerializeField] private int brushSize = 10;
    [SerializeField] private float paintDistance = 0.05f;
    [SerializeField] private LayerMask canvasLayer = -1;
    
    [Header("Screen Touch Settings")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private float brushOffsetFromCanvas = 0.01f; // distance brush sits in front of canvas
    [SerializeField] private Vector3 brushRotationOffset = new Vector3(180f, 0f, 0f); // rotation offset for brush model
    
    private Canvas targetCanvas;
    private bool isPainting = false;
    private Renderer brushRenderer;
    
    void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                arCamera = FindObjectOfType<Camera>();
            }
        }
        
        brushRenderer = GetComponent<Renderer>();
        if (brushRenderer == null)
        {
            brushRenderer = GetComponentInChildren<Renderer>();
        }
    }
    
    void Update()
    {
        if (controlMode == BrushControlMode.ScreenTouch) { CheckScreenTouch(); }
        else { CheckCanvasContact(); }
    }
    
    private void CheckScreenTouch()
    {
        bool isTouching = false;
        Vector2 screenPosition = Vector2.zero;
        
        // mobile touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            isTouching = true;
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // mouse input 
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            isTouching = true;
            screenPosition = Mouse.current.position.ReadValue();
        }
        // update brush position when mouse is moving
        else if (Mouse.current != null && controlMode == BrushControlMode.ScreenTouch)
        {
            screenPosition = Mouse.current.position.ReadValue();
        }
        
        if (arCamera != null)
        {
            // Always try to raycast to update brush position
            Ray ray = arCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, canvasLayer))
            {
                Canvas canvas = hit.collider.GetComponent<Canvas>();
                if (canvas != null)
                {
                    UpdateBrushPosition(hit.point, hit.normal);
                    
                    // Only paint if actually touching
                    if (isTouching)
                    {
                        targetCanvas = canvas;
                        PaintOnCanvas(hit.point);
                        isPainting = true;
                        
                        return;
                    }
                }
            }
        }
        
        isPainting = false;
        targetCanvas = null;
    }
    
    private void CheckCanvasContact()
    {
        // Raycast from brush tip to detect canvas 
        RaycastHit hit;
        Vector3 brushTip = transform.position + transform.forward * 0.01f;
        
        if (Physics.SphereCast(brushTip, paintDistance * 0.5f, transform.forward, out hit, paintDistance, canvasLayer))
        {
            Canvas canvas = hit.collider.GetComponent<Canvas>();
            if (canvas != null)
            {
                targetCanvas = canvas;
                PaintOnCanvas(hit.point);
                isPainting = true;
                return;
            }
        }
        
        // Also try a regular raycast as fallback
        if (Physics.Raycast(brushTip, transform.forward, out hit, paintDistance, canvasLayer))
        {
            Canvas canvas = hit.collider.GetComponent<Canvas>();
            if (canvas != null)
            {
                targetCanvas = canvas;
                PaintOnCanvas(hit.point);
                isPainting = true;
                return;
            }
        }
        
        isPainting = false;
        targetCanvas = null;
    }
    
    private void UpdateBrushPosition(Vector3 canvasHitPoint, Vector3 canvasNormal)
    {
        Vector3 brushPosition = canvasHitPoint + (canvasNormal * brushOffsetFromCanvas);
        transform.position = brushPosition;
        
        // Orient brush to face the canvas & point to surface
        Quaternion baseRotation;
        if (arCamera != null)
        {
            // Look at camera but keep brush tip pointing at canvas
            Vector3 directionToCamera = (arCamera.transform.position - transform.position).normalized;
            Vector3 forward = Vector3.Slerp(-canvasNormal, directionToCamera, 0.5f);
            baseRotation = Quaternion.LookRotation(forward, canvasNormal);
        }
        else
        {
            baseRotation = Quaternion.LookRotation(-canvasNormal);
        }
        
        // Apply rotation offset X 180
        transform.rotation = baseRotation * Quaternion.Euler(brushRotationOffset);
    }
    
    private void PaintOnCanvas(Vector3 hitPoint)
    {
        if (targetCanvas == null)
            return;
        
        // Paint on the canvas at the hit point
        targetCanvas.PaintAtPosition(hitPoint, brushColor, brushSize);
    }
    
    public void SetBrushColor(Color color) { brushColor = color;}
    public Color GetBrushColor() { return brushColor; }
    public void SetBrushSize(int size) { brushSize = Mathf.Max(1, size); }
}