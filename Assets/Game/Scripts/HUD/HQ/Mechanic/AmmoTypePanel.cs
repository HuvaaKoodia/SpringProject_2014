using UnityEngine;
using System.Collections;

public class AmmoTypePanel : MonoBehaviour {

    PlayerObjData Player;
    public UILabel Amount,Name,Cost;
    public UIButton Button;

    public float cost_multi=2f;
    public bool AllowBuying=true;
  
    int cost;
    string index;

    void Start(){

    }

    public void SetPlayer(PlayerObjData player,string ammo_index){
        Player=player;
        index=ammo_index;
        Name.text=player.GetAmmoData(index).Name;

        UpdateStats();
    }

    void UpdateStats(){
        int amount=Player.GetAmmoAmount(index);
        var data=Player.GetAmmoData(index);
        cost=(int)((data.MaxAmount-amount)*cost_multi);

        Amount.text=amount+"/"+data.MaxAmount;
		Cost.text=""+cost+" "+XmlDatabase.MoneyUnit;

        if (!AllowBuying||cost==0){
            Button.gameObject.SetActive(false);
            Cost.gameObject.SetActive(false);
        }
    }

    public void FillAmmo(){
        if (Player.Money>=cost){
            Player.Money-=cost;

            Player.FillAmmo(index);

            UpdateStats();
        }

    }
}
