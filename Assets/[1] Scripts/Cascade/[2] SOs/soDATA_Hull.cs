using UnityEngine;

public abstract class soDATA_Hull : ScriptableObject
{
    public Sprite icon;
    [TextArea] public string info;
    public abstract string type {get;}
    public int hp;
    public int ap;
    public int sp;
    public int speed;
    public int cost;
}