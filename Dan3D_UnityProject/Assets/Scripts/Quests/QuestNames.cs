using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestNames 
{
    public enum Names {
        LostSundial,
        BasilVeggies,
        HungryFox,
        ElderberryRitual
    };

    public static Names StringToQuestName(string val) {
        Names name = (Names)System.Enum.Parse(typeof(Names), val);
        return name;
    }
}
