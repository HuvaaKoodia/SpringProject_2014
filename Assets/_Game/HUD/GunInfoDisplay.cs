using UnityEngine;
using System.Collections;

public class GunInfoDisplay : MonoBehaviour {

	public MenuHandler menuHandler;

	public UILabel label;

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

		WeaponMain gun = menuHandler.player.currentGun;

		string info = gun.GunName;

		info += "\n";

		info += "ammo: " + gun.CurrentAmmo + "/" + gun.MaxAmmo;

		info += "\n";

		info += "heat: " + gun.CurrentHeat + "/100";

		info += "\n";

		info += "ROF: " + gun.RateOfFire;

		label.text = info;
	}
}
