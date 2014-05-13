using UnityEngine;
using System.Collections;

public class MechaPartRepairPanel : MonoBehaviour {

    PlayerObjData Player;
    MechaPartObjData Part;
    public UILabel Condition,Name,Cost;
    public UIButton Button;
	private bool ButtonActive;

    public string _Name;
    public float cost_multi=2f;
    public bool AllowBuying=true;

	public UIEquipmentSlot.Slot EquipmentSlot;

    int cost;

    void Start(){
        Name.text=_Name;
		ButtonActive = true;
    }

    public void SetPlayer(PlayerObjData player,MechaPartObjData part, bool allow_buying){
        Player=player;
		Part=part;
        AllowBuying=allow_buying;
    }

    public void UpdateStats(){
		int con=(int)((float)Part.HP/MechaPartObjData.MaxHP*100f);
		cost=(int)((MechaPartObjData.MaxHP-Part.HP)*XmlDatabase.MechaPartRepairCostMulti);
		
		Condition.text=con+"%";
		Cost.text="Cost: "+cost;
		
		// Lazy. Best Regards, Your Divine Producer
		if (AllowBuying) {
			SetButtonActive(cost>0);
		}

		//FIX for previous lazyness.
		if (!AllowBuying){
			ButtonActive = false;
			Cost.gameObject.SetActive(false);
			Button.gameObject.SetActive(false);
		}
    }

	private void SetButtonActive(bool active){
		ButtonActive = active;

		var alpha = active ? 1f : 0.05f;

		Button.gameObject.GetComponent<UIButton> ().enabled = active; 
		Button.gameObject.GetComponent<UIButtonScale> ().enabled = active;
		Cost.gameObject.SetActive (active);

		var bg = Button.GetComponentInChildren<UISprite> ();
		var c = bg.color;
		bg.color = new Color (c.r, c.g, c.b, alpha);
		Button.gameObject.GetComponentInChildren<UILabel> ().alpha = alpha;
	}

    public void Repair(){
        if (ButtonActive && Player.Money>=cost){
            Player.Money-=cost;
            Part.ResetHP();
            UpdateStats();
        }
    }
}
