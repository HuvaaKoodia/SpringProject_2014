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

        LH.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftHand).ObjData,allow_buying);
        RH.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightHand).ObjData,allow_buying);
        LS.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftShoulder).ObjData,allow_buying);
        RS.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightShoulder).ObjData,allow_buying);

        UT.SetPlayer(player,player.Equipment.UpperTorso.ObjData,allow_buying);
        if (LT!=null)
            LT.SetPlayer(player,player.Equipment.LowerTorso.ObjData,allow_buying);

        AmmoPanel.SetPlayer(player,allow_buying);
        UpdateStats();
    }

    void Update(){
        UpdateStats();
    }

    void UpdateStats(){
        if (MoneyLabel!=null) MoneyLabel.text="Money: "+player.Money+" BC";
    }
}
