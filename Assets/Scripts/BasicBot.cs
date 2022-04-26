using System.Collections.Generic;
using UnityEngine;

public class BasicBot : Part
{
    public new string description = "Can move one square in any direction.";

    public override Hex[] GetTargets(Hex current, CombatController controller)
    {
        List<Hex> targets = new List<Hex>();
        for (int i = 0; i < 6; ++i)
        {
            Hex target = current.Neighbor(i);
            if (!controller.pieces.ContainsKey(target) &&
                controller.tiles.ContainsKey(target)) targets.Add(target);
        }
        return targets.ToArray();
    }

    public override bool Act(Hex current, Hex target, CombatController controller)
    {
        GameObject piece = controller.pieces[current];
        controller.pieces.Remove(current);
        controller.pieces.Add(target, piece);
        PositionObject(piece, target.Subtract(current));
        return true;
    }
}
