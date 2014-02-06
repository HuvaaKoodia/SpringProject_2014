using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunDisplayMain : MonoBehaviour {

	public GameController GC;
	public List<GunDisplayScreenSub> gunInfoScreens;

	public Color[] weaponColors;
	public Color inActiveColor;

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
			screen.highlight.color = inActiveColor;

		gunInfoScreens[(int)currentWeapon].highlight.color = weaponColors[(int)currentWeapon];
	}
}
