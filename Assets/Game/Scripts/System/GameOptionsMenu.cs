using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class that updates the Options Menu in the game
//allows for changes in Quality Settings to be made
public class GameOptionsMenu : MonoBehaviour
{
	GameOptionsMain GOpsMain;
	public List<UILabel> SettingsLabel;								//labels of the various properties in Quality Settings

	// Use this for initialization
	void Start ()
	{
		GOpsMain = SharedSystemsMain.I.GOps;
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateSettings();
	}

	public void SetGameOptionsMain(GameOptionsMain newGOpsMain)
	{
		GOpsMain = newGOpsMain;
	}

	void ChangeQuality()
	{
		if(SettingsLabel[0].text == "Fastest")
		{
			GOpsMain.SetQuality(Quality.FASTEST);
		}
		else if(SettingsLabel[0].text == "Fast")
		{
			GOpsMain.SetQuality(Quality.FAST);
		}
		else if(SettingsLabel[0].text == "Simple")
		{
			GOpsMain.SetQuality(Quality.SIMPLE);
		}
		else if(SettingsLabel[0].text == "Good")
		{
			GOpsMain.SetQuality(Quality.GOOD);
		}
		else if(SettingsLabel[0].text == "Beautiful")
		{
			GOpsMain.SetQuality(Quality.BEAUTIFUL);
		}
		else if(SettingsLabel[0].text == "Fantastic")
		{
			GOpsMain.SetQuality(Quality.FANTASTIC);
		}
	}

	void ChangeLight()
	{
		if(SettingsLabel[1].text == "Low")
		{
			GOpsMain.SetPixelLightCount(LightQuality.LOW);
		}
		else if(SettingsLabel[1].text == "Medium")
		{
			GOpsMain.SetPixelLightCount(LightQuality.MEDIUM);
		}
		else if(SettingsLabel[1].text == "High")
		{
			GOpsMain.SetPixelLightCount(LightQuality.HIGH);
		}
	}

	void ChangeTexture()
	{
		if(SettingsLabel[2].text == "Full Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.FULL_RES);
		}
		else if(SettingsLabel[2].text == "Half Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.HALF_RES);
		}
		else if(SettingsLabel[2].text == "Quarter Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.QUARTER_RES);
		}
		else if(SettingsLabel[2].text == "Eighth Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.EIGHTH_RES);
		}
	}

	void ChangeAnisotropic()
	{
		if(SettingsLabel[3].text == "Disable")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.DISABLE);
		}
		else if(SettingsLabel[3].text == "Enable")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.ENABLE);
		}
		else if(SettingsLabel[3].text == "Force Enable")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.FORCE_ENABLE);
		}
	}

	void ChangeAntiAliasing()
	{
		if(SettingsLabel[4].text == "Disabled")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.DISABLED);
		}
		else if(SettingsLabel[4].text == "x2")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X2);
		}
		else if(SettingsLabel[4].text == "x4")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X4);
		}
		else if(SettingsLabel[4].text == "x8")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X8);
		}
	}

	void ChangeShadow()
	{
		if(SettingsLabel[5].text == "Off")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.OFF);
		}
		else if(SettingsLabel[5].text == "Low")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.LOW);
		}
		else if(SettingsLabel[5].text == "Medium")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.MEDIUM);
		}
		else if(SettingsLabel[5].text == "High")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.HIGH);
		}
	}

	void ChangeVSync()
	{
		if(SettingsLabel[6].text == "No Sync")
		{
			GOpsMain.SetVSync(VSync.NO_SYNC);
		}
		else if(SettingsLabel[6].text == "Every VBlank")
		{
			GOpsMain.SetVSync(VSync.EVERY_VBLANK);
		}
		else if(SettingsLabel[6].text == "Every 2nd VBlank")
		{
			GOpsMain.SetVSync(VSync.EVERY_2ND_VBLANK);
		}
	}

	void UpdateSettings()
	{
		ChangeQuality();
		ChangeLight();
		ChangeTexture();
		ChangeAnisotropic();
		ChangeAntiAliasing();
		ChangeShadow();
		ChangeVSync();
	}
}

