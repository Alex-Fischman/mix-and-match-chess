using UnityEngine;

public class Clickable : MonoBehaviour
{
    public bool click { get; private set; }

    public void Start()
    {
        click = false;
    }

    public void OnMouseDown()
    {
        click = true;
    }

    public void OnMouseUp()
    {
        click = false;
    }

    public void OnMouseExit()
    {
        click = false;
    }
}
