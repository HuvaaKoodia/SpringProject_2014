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

        SetWeaponText(LS,UIEquipmentSlot.Slot.WeaponLeftShoulder);
        SetWeaponText(LH,UIEquipmentSlot.Slot.WeaponLeftHand);
        SetWeaponText(RS,UIEquipmentSlot.Slot.WeaponRightShoulder);
        SetWeaponText(RH,UIEquipmentSlot.Slot.WeaponRightHand);

        UT.text="UT: "+Player.ObjData.Equipment.UpperTorso.ObjData.HP;
	}

    void SetWeaponText(UILabel label,UIEquipmentSlot.Slot slot){
        var data=Player.ObjData.Equipment.GetSlot(slot).ObjData;
        label.text="LS: "+data.HP+", "+data.HEAT;
    }
}
