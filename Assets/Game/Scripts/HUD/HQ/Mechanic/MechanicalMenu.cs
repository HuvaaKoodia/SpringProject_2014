using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechanicalMenu : MonoBehaviour {

    public MechStatisticsMain Statistics;

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
        
		foreach (var panel in RepairPanels){
			panel.OnRepair+=UpdateStats;
		}
		UT.OnRepair+=UpdateStats;
    }

    void UpdateStats(){
		foreach (var panel in RepairPanels){
			panel.UpdateStats();
		}
		UT.UpdateStats();
		Statistics.UpdateStats();
    }

	public void OpenPanel(){
		UpdateStats();
	}
}
