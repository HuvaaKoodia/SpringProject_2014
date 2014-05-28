using System.Collections;

//all the enumerators fir settings that can be adjusted in game
public enum Quality
{
	FASTEST = 0,
	FAST,
	SIMPLE,
	GOOD,
	BEAUTIFUL,
	FANTASTIC,
	CUSTOM
};

public enum LightQuality
{
	VERYLOW=0,
	LOW,
	MEDIUM,
	HIGH,
	EXTRA
};

public enum TextureQuality
{
	FULL_RES = 0,
	HALF_RES,
	QUARTER_RES,
	EIGHTH_RES
};

public enum AnisotropicQuality
{
	DISABLE = 0,
	ENABLE
};

public enum AntiAliasing
{
	Off,
	On
}

public enum ShadowQuality
{
	OFF = 0,
	LOW,
	MEDIUM,
	HIGH
};

public enum VSync
{
	OFF = 0,
	ON
};

//class that holds only data of Quality Settings to be manipulated in GameOptionsMain class
//data are all public properties for saving
public class GameOptionsObjData
{
	public bool MovementAnimations {get;set;}
	public bool MouseLook {get;set;}
	public bool CombatAnimations {get;set;}
	public bool GuiTips{get;set;}
	public bool ShowFPS{get;set;}

	public float Brightness {get;set;}
	public float MasterVolume {get;set;}

	public int quality_level{get;set;}
	public int pixel_light_count{get;set;}
	public int texture_quality{get;set;}
	public UnityEngine.AnisotropicFiltering anisotropic_filtering{get;set;}
	public int anti_Aliasing{get;set;}
	public bool anti_Aliasing_On{get;set;}
	public bool GlowAndBloom_On{get;set;}
	public UnityEngine.ShadowProjection shadow_projection{get;set;}
	public int shadow_cascades{get;set;}
	public float shadow_distance{get;set;}
	public int vsync_count{get;set;}

	public GameOptionsObjData()
	{
		MovementAnimations=true;
		CombatAnimations=true;
		MouseLook=true;
		GuiTips=true;
		ShowFPS=false;

		anti_Aliasing_On=true;
		GlowAndBloom_On=true;

		MasterVolume=1f;
		Brightness=0.2f;

		quality_level = 0;
		pixel_light_count = 0;
		texture_quality = 0;
		anisotropic_filtering = UnityEngine.AnisotropicFiltering.Disable;
		anti_Aliasing = 0;
		shadow_projection = UnityEngine.ShadowProjection.CloseFit;
		shadow_cascades = 0;
		shadow_distance = 0.0f;
		vsync_count = 0;
	}
}

