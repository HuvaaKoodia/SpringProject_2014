using UnityEngine;
using System.Collections;

//class that has methods to manipulate the data in GameOptionsObjData class
//methods are to be called in GameOptionsMenu class
public class GameOptionsMain : MonoBehaviour {

	GameOptionsObjData GOpsObjD;

	void Start(){
		GOpsObjD=SharedSystemsMain.I.GDB.GameOptionsData;
	}

	public void SetQuality(Quality Q)
	{
		switch(Q)
		{
		case Quality.FASTEST:
			GOpsObjD.quality_level = 0;
			break;
		case Quality.FAST:
			GOpsObjD.quality_level = 1;
			break;
		case Quality.SIMPLE:
			GOpsObjD.quality_level = 2;
			break;
		case Quality.GOOD:
			GOpsObjD.quality_level = 3;
			break;
		case Quality.BEAUTIFUL:
			GOpsObjD.quality_level = 4;
			break;
		case Quality.FANTASTIC:
			GOpsObjD.quality_level = 5;
			break;
		case Quality.CUSTOM:
			GOpsObjD.quality_level = 6;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetPixelLightCount(LightQuality LQ)
	{
		switch(LQ)
		{
		case LightQuality.LOW:
			GOpsObjD.pixel_light_count = 3;
			break;
		case LightQuality.MEDIUM:
			GOpsObjD.pixel_light_count = 5;
			break;
		case LightQuality.HIGH:
			GOpsObjD.pixel_light_count = 10;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetTextureQuality(TextureQuality TQ)
	{
		switch(TQ)
		{
		case TextureQuality.FULL_RES:
			GOpsObjD.texture_quality = 0;
			break;
		case TextureQuality.HALF_RES:
			GOpsObjD.texture_quality = 1;
			break;
		case TextureQuality.QUARTER_RES:
			GOpsObjD.texture_quality = 2;
			break;
		case TextureQuality.EIGHTH_RES:
			GOpsObjD.texture_quality = 3;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetAnisotropicQuality(AnisotropicQuality AQ)
	{
		switch(AQ)
		{
		case AnisotropicQuality.DISABLE:
			GOpsObjD.anisotropic_filtering = AnisotropicFiltering.Disable;
			break;
		case AnisotropicQuality.ENABLE:
			GOpsObjD.anisotropic_filtering = AnisotropicFiltering.ForceEnable;
			break;
		}

		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetAntiAliasing(AntiAliasing AA)
	{
		switch(AA)
		{
		case AntiAliasing.DISABLED:
			GOpsObjD.anti_Aliasing = 0;
			break;
		case AntiAliasing.X2:
			GOpsObjD.anti_Aliasing = 2;
			break;
		case AntiAliasing.X4:
			GOpsObjD.anti_Aliasing = 4;
			break;
		case AntiAliasing.X8:
			GOpsObjD.anti_Aliasing = 8;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetShadowQuality(ShadowQuality SQ)
	{
		switch(SQ)
		{
		case ShadowQuality.OFF:
			GOpsObjD.shadow_projection = ShadowProjection.CloseFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.CloseFit;

			GOpsObjD.shadow_cascades = 0;
			GOpsObjD.shadow_distance = 0;
			break;
		case ShadowQuality.LOW:
			GOpsObjD.shadow_projection = ShadowProjection.CloseFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.CloseFit;

			GOpsObjD.shadow_cascades = 0;
			GOpsObjD.shadow_distance = 15;
			break;
		case ShadowQuality.MEDIUM:
			GOpsObjD.shadow_projection = ShadowProjection.StableFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.StableFit;

			GOpsObjD.shadow_cascades = 2;
			GOpsObjD.shadow_distance = 70;
			break;
		case ShadowQuality.HIGH:
			GOpsObjD.shadow_projection = ShadowProjection.StableFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.StableFit;

			GOpsObjD.shadow_cascades = 4;
			GOpsObjD.shadow_distance = 150;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	public void SetVSync(VSync VS)
	{
		switch(VS)
		{
		case VSync.OFF:
			GOpsObjD.vsync_count = 0;
			break;
		case VSync.ON:
			GOpsObjD.vsync_count = 1;
			break;
		}
		SetQualitySettingsToData(GOpsObjD);
	}

	/// <summary>
	/// Sets the quality values to a game options data object
	/// </summary>
	/// <param name="data">Data.</param>
	public void SetQualitySettingsToData(GameOptionsObjData data){

		QualitySettings.SetQualityLevel(GOpsObjD.quality_level);

		QualitySettings.masterTextureLimit = GOpsObjD.texture_quality;
		QualitySettings.pixelLightCount = GOpsObjD.pixel_light_count;
		QualitySettings.vSyncCount = GOpsObjD.vsync_count;
		QualitySettings.shadowCascades = GOpsObjD.shadow_cascades;
		QualitySettings.shadowDistance = GOpsObjD.shadow_distance;
		QualitySettings.antiAliasing = GOpsObjD.anti_Aliasing;
	}
}
