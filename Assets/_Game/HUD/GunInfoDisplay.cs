using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunInfoDisplay : MonoBehaviour {

	public MenuHandler menuHandler;

	public UILabel infoLabel;
	public UISprite selectionHighlight;
	public List<UISprite> weaponSelectionButtons;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateGunInfo()
	{
		if (menuHandler.player == null)
			return;

		selectionHighlight.transform.position = 
			weaponSelectionButtons[(int)menuHandler.player.currentGunID].transform.position;

		WeaponMain gun = menuHandler.player.GetCurrentWeapon();

		string info = gun.GunName;

		info += "\n";

		info += "ammo: " + gun.CurrentAmmo + "/" + gun.MaxAmmo;

		info += "\n";

		info += "heat: " + gun.CurrentHeat + "/100";

		info += "\n";
	
		info += "ROF: " + gun.GetNumShotsTargetedTotal() + "/" + gun.RateOfFire;

		infoLabel.text = info;
	}

	public Color GetGunColor(WeaponID id)
	{
		return weaponSelectionButtons[(int)id].color;
	}
}
