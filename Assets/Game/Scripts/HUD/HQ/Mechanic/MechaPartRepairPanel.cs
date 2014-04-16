using UnityEngine;
using System.Collections;

public class MechaPartRepairPanel : MonoBehaviour {

    PlayerObjData Player;
    MechaPartObjData Part;
    public UILabel Condition,Name,Cost;
    public UIButton Button;

    public string _Name;
    public float cost_multi=2f;
    public bool AllowBuying=true;

    int cost;

    void Start(){
        Name.text=_Name;
    }

    public void SetPlayer(PlayerObjData player,MechaPartObjData part, bool allow_buying){
        Player=player;
		Part=part;
        AllowBuying=allow_buying;

        UpdateStats();
    }

    public void UpdateStats(){
        int con=(int)((float)Part.HP/MechaPartObjData.MaxHP*100f);
        cost=(int)((MechaPartObjData.MaxHP-Part.HP)*cost_multi);

        Condition.text=con+"%";
        Cost.text="Cost: "+cost;

        if (!AllowBuying||cost==0){
            Button.gameObject.SetActive(false);
            Cost.gameObject.SetActive(false);
        }
    }

    public void Repair(){
        if (Player.Money>=cost){
            Player.Money-=cost;
            Part.ResetHP();
            UpdateStats();
        }
    }
}
