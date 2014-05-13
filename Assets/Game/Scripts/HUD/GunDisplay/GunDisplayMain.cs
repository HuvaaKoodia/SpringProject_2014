using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunDisplayMain : MonoBehaviour {

	public GameController GC;
	public List<GunDisplayScreenSub> gunInfoScreens;

	public List<Color> weaponColors;
	public Color inActiveColor;

	// Use this for initialization
	void Awake()
	{
		inActiveColor = Color.black;
	}

	public void Start(){
		UpdateAllDisplays();
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

		//activate current item
		if (!GC.Player.GetCurrentWeapon().Usable())
            gunInfoScreens[(int)GC.Player.currentWeaponID].SetHighlightColor(inActiveColor);
	}

	public void UpdateDisplay(WeaponID id)
	{
		gunInfoScreens[(int)id].UpdateGunInfo(GC.Player);
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
