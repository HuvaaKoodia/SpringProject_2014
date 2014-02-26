using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunDisplayMain : MonoBehaviour {

	public GameController GC;
	public List<GunDisplayScreenSub> gunInfoScreens;

	public Color[] weaponColors { get; private set; }
	public Color inActiveColor { get; private set; }

	// Use this for initialization
	void Awake()
	{
		weaponColors = new Color[] 
		{
			Color.cyan,
			Color.magenta,
			Color.red,
			Color.yellow
		};

		inActiveColor = Color.black;
	}

	public void SetWeaponToDisplay(WeaponID ID, WeaponMain weapon)
	{
		gunInfoScreens[(int)ID].weapon = weapon;
		UpdateDisplay(ID);
	}

	public void UpdateAllDisplays()
	{
		for (int i = 0; i < 4; i++)
		{
			UpdateDisplay((WeaponID)i);
		}

		if (!GC.Player.GetCurrentWeapon().Usable())
            gunInfoScreens[(int)GC.Player.currentWeaponID].SetHighlightColor(inActiveColor);
	}

	public void UpdateDisplay(WeaponID id)
	{
		gunInfoScreens[(int)id].UpdateGunInfo();
	}

	public Color GetWeaponColor(WeaponID id)
	{
		return weaponColors[(int)id];
	}

	public void ChangeCurrentHighlight(WeaponID currentWeapon)
	{
		foreach(GunDisplayScreenSub screen in gunInfoScreens)
			screen.SetHighlightColor(inActiveColor);

		if (GC.Player.GetCurrentWeapon().Weapon != null)
			gunInfoScreens[(int)currentWeapon].SetHighlightColor(weaponColors[(int)currentWeapon]);
	}
}
