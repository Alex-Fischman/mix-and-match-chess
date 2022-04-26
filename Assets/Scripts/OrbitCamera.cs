using UnityEngine;
using UnityEngine.UI;

public class OrbitCamera : MonoBehaviour
{
    public Transform board;

    public Slider yAxis;

    public void Start()
    {
        yAxis.onValueChanged.AddListener(
            value => transform.RotateAround(board.position, Vector3.up, -value - transform.eulerAngles.y)
        );
    }
}
