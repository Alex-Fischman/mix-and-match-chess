using System.Collections.Generic;
using UnityEngine;

public class BasicMid : Part
{
    public new string description = "Does nothing right now.";

    public override Hex[] GetTargets(Hex current, CombatController controller)
    {
        return new Hex[1] { current };
    }

    public override bool Act(Hex current, Hex target, CombatController controller)
    {
        return true;
    }
}
