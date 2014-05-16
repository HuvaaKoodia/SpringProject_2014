using UnityEngine;
using System.Collections;

public class GameOptionsMain : MonoBehaviour {

	GameOptionsObjData GOpsObjD = new GameOptionsObjData();

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
		}
		QualitySettings.SetQualityLevel(GOpsObjD.quality_level);
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
		QualitySettings.pixelLightCount = GOpsObjD.pixel_light_count;
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
		QualitySettings.masterTextureLimit = GOpsObjD.texture_quality;
	}

	public void SetAnisotropicQuality(AnisotropicQuality AQ)
	{
		switch(AQ)
		{
		case AnisotropicQuality.DISABLE:
			GOpsObjD.anisotropic_textures = AnisotropicFiltering.Disable.ToString();
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			break;
		case AnisotropicQuality.ENABLE:
			GOpsObjD.anisotropic_textures = AnisotropicFiltering.Enable.ToString();
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
			break;
		case AnisotropicQuality.FORCE_ENABLE:
			GOpsObjD.anisotropic_textures = AnisotropicFiltering.ForceEnable.ToString();
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			break;
		}
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
	}

	public void SetShadowQuality(ShadowQuality SQ)
	{
		switch(SQ)
		{
		case ShadowQuality.OFF:
			GOpsObjD.shadow_projection = ShadowProjection.CloseFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.StableFit;

			GOpsObjD.shadow_cascades = 0;
			GOpsObjD.shadow_distance = 0;
			break;
		case ShadowQuality.LOW:
			GOpsObjD.shadow_projection = ShadowProjection.CloseFit.ToString();
			QualitySettings.shadowProjection = ShadowProjection.StableFit;

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
		QualitySettings.shadowCascades = GOpsObjD.shadow_cascades;
		QualitySettings.shadowDistance = GOpsObjD.shadow_distance;
	}

	public void SetVSync(VSync VS)
	{
		switch(VS)
		{
		case VSync.NO_SYNC:
			GOpsObjD.vsync_count = 0;
			break;
		case VSync.EVERY_VBLANK:
			GOpsObjD.vsync_count = 1;
			break;
		case VSync.EVERY_2ND_VBLANK:
			GOpsObjD.vsync_count = 2;
			break;
		}
		QualitySettings.vSyncCount = GOpsObjD.vsync_count;
	}
}
