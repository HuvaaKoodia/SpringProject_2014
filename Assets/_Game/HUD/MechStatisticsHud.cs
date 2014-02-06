using UnityEngine;
using System.Collections;

public class MechStatisticsHud : MonoBehaviour {

    public UILabel LS,LH,RS,RH,UT;
    PlayerMain Player;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update (){
        if (Player==null){
            Player=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        }

        SetWeaponText("LS: ",LS,UIEquipmentSlot.Slot.WeaponLeftShoulder);
        SetWeaponText("LH: ",LH,UIEquipmentSlot.Slot.WeaponLeftHand);
        SetWeaponText("RS: ",RS,UIEquipmentSlot.Slot.WeaponRightShoulder);
        SetWeaponText("RH: ",RH,UIEquipmentSlot.Slot.WeaponRightHand);

        UT.text="UT: "+Player.ObjData.Equipment.UpperTorso.ObjData.HP;
	}

    void SetWeaponText(string text,UILabel label,UIEquipmentSlot.Slot slot){
        var data=Player.ObjData.Equipment.GetSlot(slot).ObjData;
        label.text=text+data.HP+", "+data.HEAT+" "+(data.OVERHEAT?"OVERHEAT":"");
    }
}
