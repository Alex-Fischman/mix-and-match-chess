using System.Collections.Generic;
using UnityEngine;

public class BasicTop : Part
{
    public new string description = "Takes other pieces by moving on top of them.";

    public override Hex[] GetTargets(Hex current, CombatController controller)
    {
        List<Hex> targets = new List<Hex>();
        for (int i = 0; i < 6; ++i)
        {
            Hex target = current.Neighbor(i);
            if (controller.pieces.ContainsKey(target) &&
                controller.pieces[current].GetComponent<Piece>().team !=
                controller.pieces[target].GetComponent<Piece>().team) targets.Add(target);
        }
        return targets.ToArray();
    }

    public override bool Act(Hex current, Hex target, CombatController controller)
    {
        GameObject piece = controller.pieces[current];
        GameObject enemy = controller.pieces[target];
        controller.pieces.Remove(current);
        controller.pieces[target] = piece;
        GameObject.Destroy(enemy);
        PositionObject(piece, target.Subtract(current));
        return true;
    }
}
