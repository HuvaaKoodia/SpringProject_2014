using UnityEngine;
using System.Collections;

public class MechanicalMenu : MonoBehaviour {

    public MechStatisticsMain Statistics;
    public MechaPartRepairPanel LH,RH,LS,RS,UT,LT;
    public UILabel MoneyLabel;

    public bool allow_buying=true;

    public AmmoPanelMain AmmoPanel;

    PlayerObjData player;

    public void SetPlayer(PlayerObjData player){
        this.player=player;
        Statistics.SetPlayer(player);

		LH.SetPlayer(player,player.GetPart(UIEquipmentSlot.Slot.WeaponLeftHand),allow_buying);
		RH.SetPlayer(player,player.GetPart(UIEquipmentSlot.Slot.WeaponRightHand),allow_buying);
		LS.SetPlayer(player,player.GetPart(UIEquipmentSlot.Slot.WeaponLeftShoulder),allow_buying);
		RS.SetPlayer(player,player.GetPart(UIEquipmentSlot.Slot.WeaponRightShoulder),allow_buying);

        UT.SetPlayer(player,player.Equipment.UpperTorso.ObjData,allow_buying);
        //if (LT!=null)
            //LT.SetPlayer(player,player.Equipment.LowerTorso,allow_buying);

        AmmoPanel.SetPlayer(player,allow_buying);
        UpdateStats();
    }

    void Update(){
        UpdateStats();
    }

    void UpdateStats(){
		if (MoneyLabel!=null) MoneyLabel.text="Money: "+player.Money+" "+XmlDatabase.MoneyUnit;
    }
}
