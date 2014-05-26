using UnityEngine;
using System.Collections;

//class that has methods to manipulate the data in GameOptionsObjData class
//methods are to be called in GameOptionsMenu class
public class GameOptionsMain : MonoBehaviour {

	public GameOptionsObjData Data{get;set;}
	
	public void ToggleMovementAnimations ()
	{
		Data.MovementAnimations=!Data.MovementAnimations;
	}

	public void UpdateGameBrightness(){
		var b=Data.Brightness;
		RenderSettings.ambientLight=new Color(b,b,b,1);
	}

	public void UpdateMasterVolume ()
	{
		AudioListener.volume=Data.MasterVolume;
	}

	public void ToggleCombatAnimations ()
	{
		Data.CombatAnimations=!Data.CombatAnimations;
	}
	
	public void ToggleMouseLook()
	{
		Data.MouseLook=!Data.MouseLook;
	}

	public void ToggleGuiTips()
	{
		Data.GuiTips=!Data.GuiTips;
	}

	//graphics setting
	public void SetQuality(Quality Q)
	{
		switch(Q)
		{
		case Quality.FASTEST:
			Data.quality_level = 0;
			break;
		case Quality.FAST:
			Data.quality_level = 1;
			break;
		case Quality.SIMPLE:
			Data.quality_level = 2;
			break;
		case Quality.GOOD:
			Data.quality_level = 3;
			break;
		case Quality.BEAUTIFUL:
			Data.quality_level = 4;
			break;
		case Quality.FANTASTIC:
			Data.quality_level = 5;
			break;
		case Quality.CUSTOM:
			Data.quality_level = 6;
			break;
		}
		//SetQualitySettingsToData(Data);
		QualitySettings.SetQualityLevel(Data.quality_level);
	}

	public void SetPixelLightCount(LightQuality LQ)
	{
		switch(LQ)
		{
		case LightQuality.VERYLOW:
			Data.pixel_light_count = 0;
			break;
		case LightQuality.LOW:
			Data.pixel_light_count = 1;
			break;
		case LightQuality.MEDIUM:
			Data.pixel_light_count = 2;
			break;
		case LightQuality.HIGH:
			Data.pixel_light_count = 3;
			break;
		case LightQuality.EXTRA:
			Data.pixel_light_count = 4;
			break;
		}
		QualitySettings.pixelLightCount = Data.pixel_light_count;
	}

	public void SetTextureQuality(TextureQuality TQ)
	{
		switch(TQ)
		{
		case TextureQuality.FULL_RES:
			Data.texture_quality = 0;
			break;
		case TextureQuality.HALF_RES:
			Data.texture_quality = 1;
			break;
		case TextureQuality.QUARTER_RES:
			Data.texture_quality = 2;
			break;
		case TextureQuality.EIGHTH_RES:
			Data.texture_quality = 3;
			break;
		}
		QualitySettings.masterTextureLimit = Data.texture_quality;
	}

	public void SetAnisotropicQuality(AnisotropicQuality AQ)
	{
		switch(AQ)
		{
		case AnisotropicQuality.DISABLE:
			Data.anisotropic_filtering = AnisotropicFiltering.Disable;
			break;
		case AnisotropicQuality.ENABLE:
			Data.anisotropic_filtering = AnisotropicFiltering.ForceEnable;
			break;
		}

		QualitySettings.anisotropicFiltering=Data.anisotropic_filtering;
	}

	public void SetAntiAliasing(AntiAliasing AA)
	{
		switch(AA)
		{
		case AntiAliasing.Off:
			Data.anti_Aliasing_On = false;
			break;
		case AntiAliasing.On:
			Data.anti_Aliasing_On = true;
			break;
		}
	}

	public void SetShadowQuality(ShadowQuality SQ)
	{
		switch(SQ)
		{
		case ShadowQuality.OFF:
			Data.shadow_projection = ShadowProjection.CloseFit;

			Data.shadow_cascades = 0;
			Data.shadow_distance = 0;
			break;
		case ShadowQuality.LOW:
			Data.shadow_projection = ShadowProjection.CloseFit;

			Data.shadow_cascades = 1;
			Data.shadow_distance = 10;
			break;
		case ShadowQuality.MEDIUM:
			Data.shadow_projection = ShadowProjection.StableFit;

			Data.shadow_cascades = 2;
			Data.shadow_distance = 20;
			break;
		case ShadowQuality.HIGH:
			Data.shadow_projection = ShadowProjection.StableFit;

			Data.shadow_cascades = 4;
			Data.shadow_distance = 40;
			break;
		}

		QualitySettings.shadowProjection = Data.shadow_projection;
		QualitySettings.shadowCascades = Data.shadow_cascades;
		QualitySettings.shadowDistance = Data.shadow_distance;
	}

	public void SetVSync(VSync VS)
	{
		switch(VS)
		{
		case VSync.OFF:
			Data.vsync_count = 0;
			break;
		case VSync.ON:
			Data.vsync_count = 1;
			break;
		}
		QualitySettings.vSyncCount = Data.vsync_count;
	}

	/// <summary>
	/// Sets the quality values to a game options data object
	/// </summary>
	/// <param name="data">Data.</param>
	public void SetQualitySettingsToData(GameOptionsObjData data){
		Data=data;

		UpdateGameBrightness();
		UpdateMasterVolume();

		//graphics

		QualitySettings.SetQualityLevel(data.quality_level);
	
		//only read data if quality level is custom. Otherwize go with the preset.
		if (data.quality_level!=6) return;

		QualitySettings.masterTextureLimit = data.texture_quality;
		QualitySettings.pixelLightCount = data.pixel_light_count;
		QualitySettings.vSyncCount = data.vsync_count;
		QualitySettings.shadowCascades = data.shadow_cascades;
		QualitySettings.shadowDistance = data.shadow_distance;
		QualitySettings.shadowProjection=data.shadow_projection;
		QualitySettings.antiAliasing = data.anti_Aliasing;
		QualitySettings.anisotropicFiltering=data.anisotropic_filtering;


	}
}
