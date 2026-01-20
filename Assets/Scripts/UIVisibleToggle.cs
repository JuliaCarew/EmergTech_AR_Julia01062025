using UnityEngine;

public class UIVisibleToggle : MonoBehaviour
{
    private bool isVisible;

    private void Start()
    {
        isVisible = true;
    }

    public void SetVisible(GameObject visibleObject)
    {
        if (isVisible)
        {
            visibleObject.SetActive(false);
            isVisible = false;
        }
        else
        {
            visibleObject.SetActive(true);
            isVisible = true;
        }
    }
}