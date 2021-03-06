﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoPanelMain : MonoBehaviour {
    public Transform AmmoButtonsParent;
    public UIAmmoSlot AmmoPanelPrefab;

	List<UIAmmoSlot> Slots=new List<UIAmmoSlot>();

	public int xoff=105;

    public void SetPlayer(PlayerObjData player,bool allow_buying){
        //create ammo buttons
        int i=0;
        foreach(var m in player.Ammo){
            var data=player.GetAmmoData(m.Key);
            if (!data.ShowInGame) continue;
            
			var button=Instantiate(AmmoPanelPrefab) as UIAmmoSlot;
            button.transform.parent=AmmoButtonsParent;
            
            button.transform.localScale=Vector3.one;
            button.transform.localPosition=Vector3.zero;
			button.transform.localPosition+=Vector3.right*(xoff*i);//DEV.Magic number!
            ++i;

            button.AllowBuying=allow_buying;
            button.SetPlayer(player,m.Key);

			Slots.Add(button);
        }
    }

	public void UpdateAllSlots(){
		foreach (var s in Slots){
			s.UpdateStats();
		}
	}
}
