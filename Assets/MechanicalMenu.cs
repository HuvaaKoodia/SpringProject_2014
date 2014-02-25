using UnityEngine;
using System.Collections;

public class MechanicalMenu : MonoBehaviour {

    public MechStatisticsMain Statistics;
    public MechaPartRepairPanel LH,RH,LS,RS,UT,LT;
    public UILabel MoneyLabel;

    public AmmoPanelMain AmmoPanel;

    PlayerObjData player;

    public void SetPlayer(PlayerObjData player){
        this.player=player;
        Statistics.SetPlayer(player);

        LH.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftHand).ObjData);
        RH.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightHand).ObjData);
        LS.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponLeftShoulder).ObjData);
        RS.SetPlayer(player,player.Equipment.GetSlot(UIEquipmentSlot.Slot.WeaponRightShoulder).ObjData);

        UT.SetPlayer(player,player.Equipment.UpperTorso.ObjData);
        if (LT!=null)
            LT.SetPlayer(player,player.Equipment.LowerTorso.ObjData);

        AmmoPanel.SetPlayer(player);
    }

    void Update(){
        MoneyLabel.text="Money: "+player.Money+" BC";
    }
}
