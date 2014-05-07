using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechanicalMenu : MonoBehaviour {

    public MechStatisticsMain Statistics;
    public UILabel MoneyLabel;

    public bool allow_buying=true;

    public AmmoPanelMain AmmoPanel;

	public MechaPartRepairPanel UT,LT;
	public List<MechaPartRepairPanel> RepairPanels;

    public void SetPlayer(PlayerObjData player){
        Statistics.SetPlayer(player);

		foreach(var panel in RepairPanels){
			panel.SetPlayer(player,player.GetPart(panel.EquipmentSlot),allow_buying);
		}

        UT.SetPlayer(player,player.Equipment.UpperTorso.ObjData,allow_buying);
        //if (LT!=null)
            //LT.SetPlayer(player,player.Equipment.LowerTorso,allow_buying);
        UpdateStats();
    }

    void UpdateStats(){
		foreach (var panel in RepairPanels){
			panel.UpdateStats();
		}
		UT.UpdateStats();
    }

	public void OpenPanel(){
		UpdateStats();
	}
}
