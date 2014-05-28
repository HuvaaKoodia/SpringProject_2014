using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class that updates the Options Menu in the game
//allows for changes in Quality Settings to be made
public class GameOptionsMenu : MonoBehaviour
{
	
	public string[] quality_settings;

	GameOptionsMain Main;

	public UILabel QualityLabel,GlowAndBloom;
	public UILabel LightLabel;
	public UILabel TextureLabel;
	public UILabel AnisotropicLabel;
	public UILabel AntiAliasingLabel;
	public UILabel ShadowLabel;
	public UILabel VSyncLabel;

	public UISlider MasterVolume, Brightness;
	public UIToggle MovementAnimationsToggle,CombatAnimationsToggle,MouseLookToggle,GuiTips,FPS;

	// Use this for initialization
	void Awake ()
	{
		//set automatic texts
		quality_settings=QualitySettings.names;

		Main = SharedSystemsMain.I.GOps;
	}

	public void OpenMenu(){
		UpdateAllLabelsToCurrentQualitySettings();
		UpdateAllWidgetsToGameOptions();
	}

	//audio

	public void UpdateMasterVolume(){
		Main.Data.MasterVolume=MasterVolume.value;
		Main.UpdateMasterVolume();
	}

	public void UpdateBrightness(){
		Main.Data.Brightness=Brightness.value;
		Main.UpdateGameBrightness();
	}

	//gameoptions

	public void ToggleMovementAnimations(){
		Main.ToggleMovementAnimations();
	}

	public void ToggleCombatAnimations(){
		Main.ToggleCombatAnimations();
	}

	public void ToggleMouseLook(){
		Main.ToggleMouseLook();
	}

	public void ToggleGuiTips(){
		Main.ToggleGuiTips();
	}
	
	public void ToggleFPS(){
		Main.Data.ShowFPS=!Main.Data.ShowFPS;
	}

	void UpdateAllWidgetsToGameOptions ()
	{
		MovementAnimationsToggle.SetValueInstant(Main.Data.MovementAnimations);
		CombatAnimationsToggle.SetValueInstant(Main.Data.CombatAnimations);
		MouseLookToggle.SetValueInstant(Main.Data.MouseLook);
		GuiTips.SetValueInstant(Main.Data.GuiTips);
		FPS.SetValueInstant(Main.Data.ShowFPS);

		Brightness.value=Main.Data.Brightness;
		MasterVolume.value=Main.Data.MasterVolume;
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
		if(Main.Data.anti_Aliasing_On)
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
		if(Main.Data.GlowAndBloom_On)
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
			Main.SetPixelLightCount(LightQuality.VERYLOW);
		}
		if(LightLabel.text == "Low")
		{
			Main.SetPixelLightCount(LightQuality.LOW);
		}
		else if(LightLabel.text == "Medium")
		{
			Main.SetPixelLightCount(LightQuality.MEDIUM);
		}
		else if(LightLabel.text == "High")
		{
			Main.SetPixelLightCount(LightQuality.HIGH);
		}
		else if(LightLabel.text == "Extra")
		{
			Main.SetPixelLightCount(LightQuality.EXTRA);
		}
	}

	void ChangeTexture()
	{
		if(TextureLabel.text == "Full Res")
		{
			Main.SetTextureQuality(TextureQuality.FULL_RES);
		}
		else if(TextureLabel.text == "Half Res")
		{
			Main.SetTextureQuality(TextureQuality.HALF_RES);
		}
		else if(TextureLabel.text == "Quarter Res")
		{
			Main.SetTextureQuality(TextureQuality.QUARTER_RES);
		}
		else if(TextureLabel.text == "Eighth Res")
		{
			Main.SetTextureQuality(TextureQuality.EIGHTH_RES);
		}
	}

	void ChangeAnisotropic()
	{
		if(AnisotropicLabel.text == "Off")
		{
			Main.SetAnisotropicQuality(AnisotropicQuality.DISABLE);
		}
		else if(AnisotropicLabel.text == "On")
		{
			Main.SetAnisotropicQuality(AnisotropicQuality.ENABLE);
		}
	}

	void ChangeGlowAndBloom()
	{
		if(GlowAndBloom.text == "On")
		{
			Main.Data.GlowAndBloom_On=true;
		}
		else if(GlowAndBloom.text == "Off")
		{
			Main.Data.GlowAndBloom_On=false;
		}
	}

	void ChangeAntiAliasing()
	{
		if(AntiAliasingLabel.text == "Off")
		{
			Main.SetAntiAliasing(AntiAliasing.Off);
		}
		else if(AntiAliasingLabel.text == "On")
		{
			Main.SetAntiAliasing(AntiAliasing.On);
		}
	}

	void ChangeShadow()
	{
		if(ShadowLabel.text == "Off")
		{
			Main.SetShadowQuality(ShadowQuality.OFF);
		}
		else if(ShadowLabel.text == "Low")
		{
			Main.SetShadowQuality(ShadowQuality.LOW);
		}
		else if(ShadowLabel.text == "Medium")
		{
			Main.SetShadowQuality(ShadowQuality.MEDIUM);
		}
		else if(ShadowLabel.text == "High")
		{
			Main.SetShadowQuality(ShadowQuality.HIGH);
		}
	}

	void ChangeVSync()
	{
		if(VSyncLabel.text == "Off")
		{
			Main.SetVSync(VSync.OFF);
		}
		else if(VSyncLabel.text == "On")
		{
			Main.SetVSync(VSync.ON);
		}
	}

	void UpdateLabel_preset(){
		QualityLabel.text=quality_settings[Main.Data.quality_level];
	}

	//update functions
	
	public void UpdateQualityToSelected()
	{
		if(QualityLabel.text == "Fastest")
		{
			Main.SetQuality(Quality.FASTEST);
		}
		else if(QualityLabel.text == "Fast")
		{
			Main.SetQuality(Quality.FAST);
		}
		else if(QualityLabel.text == "Simple")
		{
			Main.SetQuality(Quality.SIMPLE);
		}
		else if(QualityLabel.text == "Good")
		{
			Main.SetQuality(Quality.GOOD);
		}
		else if(QualityLabel.text == "Beautiful")
		{
			Main.SetQuality(Quality.BEAUTIFUL);
		}
		else if(QualityLabel.text == "Fantastic")
		{
			Main.SetQuality(Quality.FANTASTIC);
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
		Main.Data.quality_level=6;

		ChangeGlowAndBloom();
		ChangeLight();
		ChangeTexture();
		ChangeAnisotropic();
		ChangeAntiAliasing();
		ChangeShadow();
		ChangeVSync();
	}
}

