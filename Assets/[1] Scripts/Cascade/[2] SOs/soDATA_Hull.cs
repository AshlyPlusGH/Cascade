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

public string GetDataParagraph()
    {
        string toReturn = "";

        toReturn += "\n" + "<color=red>" + "HP: " + hp + "</color>";
        toReturn += "\n" + "<color=yellow>" + "AP: " + ap + "</color>";
        toReturn += "\n" + "<color=blue>" + "SP: " + sp + "</color>";
        toReturn += "\n" + "<color=green>" + "Speed: " + speed + "</color>";

        return toReturn;
    }
}