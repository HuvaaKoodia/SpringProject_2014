using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechStatisticsMain : MonoBehaviour {
    PlayerObjData PlayerData;

	public Color FullHealthColor = Color.green;
	public Color DamagedColor = Color.yellow;
	public Color CriticalColor = Color.red;
	public Color BrokenColor = Color.black;

	public List<MechPartsStatsSub> weaponSlots;
	public MechPartsStatsSub upperTorso;
	public MechPartsStatsSub lowerTorso;

	public UISprite HeatMeter;
	public GameObject DataPanel,OverheatSprite;

	// Use this for initialization
	void Start (){
		lowerTorso.ChangePartColor(FullHealthColor);
	}
	
    public void SetPlayer(PlayerObjData player){
        PlayerData=player;
    }

	public void UpdateStats(){
		SetPartInfo(weaponSlots[(int)WeaponID.LeftShoulder], UIEquipmentSlot.Slot.WeaponLeftShoulder);
		SetPartInfo(weaponSlots[(int)WeaponID.LeftHand], UIEquipmentSlot.Slot.WeaponLeftHand);
		SetPartInfo(weaponSlots[(int)WeaponID.RightShoulder], UIEquipmentSlot.Slot.WeaponRightShoulder);
		SetPartInfo(weaponSlots[(int)WeaponID.RightHand], UIEquipmentSlot.Slot.WeaponRightHand);

        SetPartInfo(upperTorso, PlayerData.Equipment.UpperTorso);

		var percent=PlayerData.UpperTorso.HeatPercent();

		if (HeatMeter!=null) HeatMeter.fillAmount=percent;
		if (DataPanel!=null) DataPanel.SetActive(!PlayerData.UpperTorso.OVERHEAT);
		if (OverheatSprite!=null)OverheatSprite.SetActive(PlayerData.UpperTorso.OVERHEAT);
	}

    void SetWeaponText(string text,UILabel label,UIEquipmentSlot.Slot slot){
        var data=PlayerData.Equipment.GetSlot(slot).ObjData;
        label.text=text+data.HP+", "+data.HEAT+" "+(data.OVERHEAT?"OVERHEAT":"");
    }

	void SetPartInfo(MechPartsStatsSub part, UIEquipmentSlot.Slot slot)
	{
        SetPartInfo(part,PlayerData.Equipment.GetSlot(slot).ObjData);
	}

	void SetPartInfo(MechPartsStatsSub part, InvEquipmentSlot slot)
	{
        SetPartInfo(part,slot.ObjData);
	}

    void SetPartInfo(MechPartsStatsSub part, MechaPartObjData data)
    {
        part.ShowOverheat(data.OVERHEAT);
        
        int hp = data.HP;
        
        if (hp == 0)
        {
            part.ChangePartColor(BrokenColor);
        }
		else if (hp <= MechaPartObjData.MaxHP*XmlDatabase.MechaPartConditionBadThreshold)
        {
            part.ChangePartColor(CriticalColor);
        }
		else if (hp <= MechaPartObjData.MaxHP*XmlDatabase.MechaPartConditionFairThreshold)
        {
            part.ChangePartColor(DamagedColor);
        }
        else
        {
            part.ChangePartColor(FullHealthColor);
        }
    }
}
