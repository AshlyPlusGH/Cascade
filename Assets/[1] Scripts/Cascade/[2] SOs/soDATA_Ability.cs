using UnityEngine;using System;

[Serializable]
public struct Ability
{
    public string name;
    public ENUM_AbilityType type;
    public int abilityStrength;
}
    [CreateAssetMenu(fileName = "[Ability] New",menuName = "++Ability")]
    public class soDATA_Ability : ScriptableObject
    {
        public Ability data;
        public Sprite icon;
        public string description;
        public int cost;
    }
    
public class AbilityEffects
{
    public static void Activate(Ability arg, Ship source)
    {
        switch (arg.type)
        {
            case ENUM_AbilityType.None:
                break;
            case ENUM_AbilityType.HullRepair:
                source.Heal(arg.abilityStrength*1,0,0);
                break;
            case ENUM_AbilityType.ShieldRecharge:
                source.Heal(0,0,Mathf.Clamp(source.STAT_maxSP-source.STAT_sp,0,source.STAT_maxSP));
                break;
            case ENUM_AbilityType.SprintDrives:
                source.RecoverMoves();
                break;
        }
    }
}

public enum ENUM_AbilityType
{
    None,
    HullRepair,
    ShieldRecharge,
    SprintDrives,
}