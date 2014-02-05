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

        LS.text="LS: "+Player.ObjData.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftShoulder).ObjData.HP;
        LH.text="LH: "+Player.ObjData.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftHand).ObjData.HP;
        RS.text="RS: "+Player.ObjData.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightShoulder).ObjData.HP;
        RH.text="RH: "+Player.ObjData.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightHand).ObjData.HP;

        UT.text="UT: "+Player.ObjData.Equipment.UpperTorso.ObjData.HP;
	}
}
