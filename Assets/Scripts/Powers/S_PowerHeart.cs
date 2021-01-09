using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PowerHeart : S_Power
{
    void Awake() {
        description = "You gain a heart";
    }
    public override void Action()
    {
        Player.instance.Heal(1f);
    }
}
