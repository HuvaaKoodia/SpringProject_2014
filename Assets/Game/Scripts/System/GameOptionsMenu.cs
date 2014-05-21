using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class that updates the Options Menu in the game
//allows for changes in Quality Settings to be made
public class GameOptionsMenu : MonoBehaviour
{
	GameOptionsMain GOpsMain;

	public UILabel QualityLabel;
	public UILabel LightLabel;
	public UILabel TextureLabel;
	public UILabel AnisotropicLabel;
	public UILabel AntiAliasingLabel;
	public UILabel ShadowLabel;
	public UILabel VSyncLabel;

	// Use this for initialization
	void Awake ()
	{
		GOpsMain = SharedSystemsMain.I.GOps;
	}

	public void OpenMenu(){
		UpdateAllLabelsToCurrentQualitySettings();
	}

	//label updaters

	void GetLightProperty()
	{
		if(QualitySettings.pixelLightCount == 0)
		{
			LightLabel.text = "Very Low";
		}
		if(QualitySettings.pixelLightCount == 1)
		{
			LightLabel.text = "Low";
		}
		else if(QualitySettings.pixelLightCount ==2)
		{
			LightLabel.text = "Medium";
		}
		else if(QualitySettings.pixelLightCount ==3)
		{
			LightLabel.text = "High";
		}
		else if(QualitySettings.pixelLightCount >=4)
		{
			LightLabel.text = "Extra";
		}
	}

	void GetTextureProperty()
	{
		if(QualitySettings.masterTextureLimit == 0)
		{
			TextureLabel.text = "Full Res";
		}
		else if(QualitySettings.masterTextureLimit == 1)
		{
			TextureLabel.text = "Half Res";
		}
		else if(QualitySettings.masterTextureLimit == 2)
		{
			TextureLabel.text = "Quarter Res";
		}
		else if(QualitySettings.masterTextureLimit == 3)
		{
			TextureLabel.text = "Eighth Res";
		}
	}

	void GetAnisotropicProperty()
	{
		if(QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
		{
			AnisotropicLabel.text = "Disable";
		}
		else if(QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
		{
			AnisotropicLabel.text = "Enable";
		}
	}

	void GetAntiAliasingProperty()
	{
		if(QualitySettings.antiAliasing == 0)
		{
			AntiAliasingLabel.text = "Disabled";
		}
		else if(QualitySettings.antiAliasing == 2)
		{
			AntiAliasingLabel.text = "x2";
		}
		else if(QualitySettings.antiAliasing == 4)
		{
			AntiAliasingLabel.text = "x4";
		}
		else if(QualitySettings.antiAliasing == 8)
		{
			AntiAliasingLabel.text = "x8";
		}
	}

	void GetShadowProperty()
	{
		if(QualitySettings.shadowProjection == ShadowProjection.CloseFit)
		{
			if(QualitySettings.shadowDistance <= 0)
			{
				ShadowLabel.text = "Off";
			}
			else
			{
				ShadowLabel.text = "Low";
			}
		}
		else
		{
			if(QualitySettings.shadowCascades == 2)
			{
				ShadowLabel.text = "Medium";
			}
			else if(QualitySettings.shadowCascades == 4)
			{
				ShadowLabel.text = "High";
			}
		}
	}

	void GetVSyncProperty()
	{
		if(QualitySettings.vSyncCount == 0)
		{
			VSyncLabel.text = "Off";
		}
		else if(QualitySettings.vSyncCount == 1)
		{
			VSyncLabel.text = "On";
		}
	}

	//setting changers

	void ChangeLight()
	{
		if(LightLabel.text == "Very Low")
		{
			GOpsMain.SetPixelLightCount(LightQuality.VERYLOW);
		}
		if(LightLabel.text == "Low")
		{
			GOpsMain.SetPixelLightCount(LightQuality.LOW);
		}
		else if(LightLabel.text == "Medium")
		{
			GOpsMain.SetPixelLightCount(LightQuality.MEDIUM);
		}
		else if(LightLabel.text == "High")
		{
			GOpsMain.SetPixelLightCount(LightQuality.HIGH);
		}
		else if(LightLabel.text == "Extra")
		{
			GOpsMain.SetPixelLightCount(LightQuality.HIGH);
		}
	}

	void ChangeTexture()
	{
		if(TextureLabel.text == "Full Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.FULL_RES);
		}
		else if(TextureLabel.text == "Half Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.HALF_RES);
		}
		else if(TextureLabel.text == "Quarter Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.QUARTER_RES);
		}
		else if(TextureLabel.text == "Eighth Res")
		{
			GOpsMain.SetTextureQuality(TextureQuality.EIGHTH_RES);
		}
	}

	void ChangeAnisotropic()
	{
		if(AnisotropicLabel.text == "Disable")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.DISABLE);
		}
		else if(AnisotropicLabel.text == "Enable")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.ENABLE);
		}
	}

	void ChangeAntiAliasing()
	{
		if(AntiAliasingLabel.text == "Disabled")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.DISABLED);
		}
		else if(AntiAliasingLabel.text == "x2")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X2);
		}
		else if(AntiAliasingLabel.text == "x4")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X4);
		}
		else if(AntiAliasingLabel.text == "x8")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.X8);
		}
	}

	void ChangeShadow()
	{
		if(ShadowLabel.text == "Off")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.OFF);
		}
		else if(ShadowLabel.text == "Low")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.LOW);
		}
		else if(ShadowLabel.text == "Medium")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.MEDIUM);
		}
		else if(ShadowLabel.text == "High")
		{
			GOpsMain.SetShadowQuality(ShadowQuality.HIGH);
		}
	}

	void ChangeVSync()
	{
		if(VSyncLabel.text == "Off")
		{
			GOpsMain.SetVSync(VSync.OFF);
		}
		else if(VSyncLabel.text == "On")
		{
			GOpsMain.SetVSync(VSync.ON);
		}
	}

	//update functions
	
	public void UpdateQualityToSelected()
	{
		if(QualityLabel.text == "Fastest")
		{
			GOpsMain.SetQuality(Quality.FASTEST);
		}
		else if(QualityLabel.text == "Fast")
		{
			GOpsMain.SetQuality(Quality.FAST);
		}
		else if(QualityLabel.text == "Simple")
		{
			GOpsMain.SetQuality(Quality.SIMPLE);
		}
		else if(QualityLabel.text == "Good")
		{
			GOpsMain.SetQuality(Quality.GOOD);
		}
		else if(QualityLabel.text == "Beautiful")
		{
			GOpsMain.SetQuality(Quality.BEAUTIFUL);
		}
		else if(QualityLabel.text == "Fantastic")
		{
			GOpsMain.SetQuality(Quality.FANTASTIC);
		}

		UpdateAllLabelsToCurrentQualitySettings();
	}
	
	public void UpdateAllLabelsToCurrentQualitySettings()
	{
		GetLightProperty();
		GetTextureProperty();
		GetAnisotropicProperty();
		GetAntiAliasingProperty();
		GetShadowProperty();
		GetVSyncProperty();
	}

	public void UpdateQualitySettingsToLabels()
	{
		QualityLabel.text = "Custom";

		ChangeLight();
		ChangeTexture();
		ChangeAnisotropic();
		ChangeAntiAliasing();
		ChangeShadow();
		ChangeVSync();
	}
}

