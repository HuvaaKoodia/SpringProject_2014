using UnityEngine;
using System.Collections;

public class MechaPartRepairPanel : MonoBehaviour {

    PlayerObjData Player;
    MechaPartObjData Part;
    public UILabel Condition,Name,Cost;
    public string _Name;

    public float cost_multi=2f;

    int cost;

    void Start(){
        Name.text=_Name;
    }

    public void SetPlayer(PlayerObjData player,MechaPartObjData part){
        Player=player;
        Part=part;

        UpdateStats();
    }

    void UpdateStats(){
        int con=(int)((float)Part.HP/MechaPartObjData.MaxHP*100f);
        cost=(int)((MechaPartObjData.MaxHP-Part.HP)*cost_multi);

        Condition.text=con+"%";
        Cost.text="Cost: "+cost;

        if (cost==0)
            Cost.gameObject.SetActive(false);
    }

    public void Repair(){
        Part.ResetHP();
        if (Player.Money>=cost){
            Player.Money-=cost;
        }
        UpdateStats();
    }
}
