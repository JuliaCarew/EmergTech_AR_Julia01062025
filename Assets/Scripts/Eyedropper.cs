using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Eyedropper : MonoBehaviour
{
    private Brush brush;
    private Color colordropper_color;

    [SerializeField] private Camera arCamera;

    // eyedropper tool that serves as a way to change the color of the paint 
    // will need to have raycast to a point in real space, then take some sort of color data from that point

    private void Awake()
    {
        // reference brush script
    }
    private void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                arCamera = FindObjectOfType<Camera>();
            }
        }
    }

    public void GetColorAtPosition()
    {
        Debug.Log("GetColorAtPosition called");

        Vector2 screenPosition = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            screenPosition = Mouse.current.position.ReadValue();

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown((int)KeyCode.Mouse2))
                ChangeFromDropper();
        }
    }

    void ChangeFromDropper()
    {
        brush.SetBrushColor(colordropper_color);
        // will need to test if the colors return accurately
        Debug.Log("color set from eyedropper to: " + colordropper_color);
    }
    
}