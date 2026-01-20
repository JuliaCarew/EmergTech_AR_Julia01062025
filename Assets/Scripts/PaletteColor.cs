using UnityEngine;

public class PaletteColor : MonoBehaviour
{
    public Color color;
    public Brush brushRef;
    public void ChangeBrushColor()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        // get color
        Color newColor = renderer.material.color;
        // set color
        brushRef.SetBrushColor(newColor);
        Debug.Log("Brush color set: " + newColor.ToString());
    }
}
