using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour
{
    public int seed;
    public Color normal;
    public Color selected;
    public string description;
    public abstract Hex[] GetTargets(Hex current, CombatController controller);
    public abstract bool Act(Hex current, Hex target, CombatController controller);

    protected void PositionObject(GameObject gameObject, Hex position)
    {
        Point a = Layout.basic.HexToPixel(position);
        gameObject.transform.Translate((float)a.x, 0, (float)a.y);
    }
}
