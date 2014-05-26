using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class that updates the Options Menu in the game
//allows for changes in Quality Settings to be made
public class GameOptionsMenu : MonoBehaviour
{
	
	public string[] quality_settings;

	GameOptionsMain GOpsMain;

	public UILabel QualityLabel,GlowAndBloom;
	public UILabel LightLabel;
	public UILabel TextureLabel;
	public UILabel AnisotropicLabel;
	public UILabel AntiAliasingLabel;
	public UILabel ShadowLabel;
	public UILabel VSyncLabel;

	public UISlider MasterVolume, Brightness;
	public UIToggle MovementAnimationsToggle,CombatAnimationsToggle,MouseLookToggle,GuiTips;
	
	// Use this for initialization
	void Awake ()
	{
		//set automatic texts
		quality_settings=QualitySettings.names;

		GOpsMain = SharedSystemsMain.I.GOps;
	}

	public void OpenMenu(){
		UpdateAllLabelsToCurrentQualitySettings();
		UpdateAllWidgetsToGameOptions();
	}

	//audio

	public void UpdateMasterVolume(){
		GOpsMain.Data.MasterVolume=MasterVolume.value;
		GOpsMain.UpdateMasterVolume();
	}

	public void UpdateBrightness(){
		GOpsMain.Data.Brightness=Brightness.value;
		GOpsMain.UpdateGameBrightness();
	}

	//gameoptions

	public void ToggleMovementAnimations(){
		GOpsMain.ToggleMovementAnimations();
	}

	public void ToggleCombatAnimations(){
		GOpsMain.ToggleCombatAnimations();
	}

	public void ToggleMouseLook(){
		GOpsMain.ToggleMouseLook();
	}

	public void ToggleGuiTips(){
		GOpsMain.ToggleGuiTips();
	}

	void UpdateAllWidgetsToGameOptions ()
	{
		MovementAnimationsToggle.SetValueInstant(GOpsMain.Data.MovementAnimations);
		CombatAnimationsToggle.SetValueInstant(GOpsMain.Data.CombatAnimations);
		MouseLookToggle.SetValueInstant(GOpsMain.Data.MouseLook);
		GuiTips.SetValueInstant(GOpsMain.Data.GuiTips);

		Brightness.value=GOpsMain.Data.Brightness;
		MasterVolume.value=GOpsMain.Data.MasterVolume;
	}

	//graphics

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
			AnisotropicLabel.text = "Off";
		}
		else if(QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
		{
			AnisotropicLabel.text = "On";
		}
	}

	void GetAntiAliasingProperty()
	{
		if(GOpsMain.Data.anti_Aliasing_On)
		{
			AntiAliasingLabel.text = "On";
		}
		else
		{
			AntiAliasingLabel.text = "Off";
		}
	}

	void GetGlowAndBloomProperty()
	{
		if(GOpsMain.Data.GlowAndBloom_On)
		{
			GlowAndBloom.text = "On";
		}
		else
		{
			GlowAndBloom.text = "Off";
		}
	}

	void GetShadowProperty()
	{
		if(QualitySettings.shadowDistance == 0)
		{
			ShadowLabel.text = "Off";
			return;
		}
		if(QualitySettings.shadowCascades == 1)
		{
			ShadowLabel.text = "Low";
		}
		else if(QualitySettings.shadowCascades == 2)
		{
			ShadowLabel.text = "Medium";
		}
		else if(QualitySettings.shadowCascades == 4)
		{
			ShadowLabel.text = "High";
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
			GOpsMain.SetPixelLightCount(LightQuality.EXTRA);
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
		if(AnisotropicLabel.text == "Off")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.DISABLE);
		}
		else if(AnisotropicLabel.text == "On")
		{
			GOpsMain.SetAnisotropicQuality(AnisotropicQuality.ENABLE);
		}
	}

	void ChangeGlowAndBloom()
	{
		if(GlowAndBloom.text == "On")
		{
			GOpsMain.Data.GlowAndBloom_On=true;
		}
		else if(GlowAndBloom.text == "Off")
		{
			GOpsMain.Data.GlowAndBloom_On=false;
		}
	}

	void ChangeAntiAliasing()
	{
		if(AntiAliasingLabel.text == "Off")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.Off);
		}
		else if(AntiAliasingLabel.text == "On")
		{
			GOpsMain.SetAntiAliasing(AntiAliasing.On);
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

	void UpdateLabel_preset(){
		QualityLabel.text=quality_settings[GOpsMain.Data.quality_level];
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
		UpdateLabel_preset();
		GetLightProperty();
		GetTextureProperty();
		GetAnisotropicProperty();
		GetAntiAliasingProperty();
		GetShadowProperty();
		GetVSyncProperty();
		GetGlowAndBloomProperty();
	}

	public void UpdateQualitySettingsToLabels()
	{
		QualityLabel.text = "Custom";
		GOpsMain.Data.quality_level=6;

		ChangeGlowAndBloom();
		ChangeLight();
		ChangeTexture();
		ChangeAnisotropic();
		ChangeAntiAliasing();
		ChangeShadow();
		ChangeVSync();
	}
}

