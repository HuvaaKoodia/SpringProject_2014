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
	DISABLED = 0,
	X2,
	X4,
	X8
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
	public int quality_level{get;set;}
	public int pixel_light_count{get;set;}
	public int texture_quality{get;set;}
	public UnityEngine.AnisotropicFiltering anisotropic_filtering{get;set;}
	public int anti_Aliasing{get;set;}
	public UnityEngine.ShadowProjection shadow_projection{get;set;}
	public int shadow_cascades{get;set;}
	public float shadow_distance{get;set;}
	public int vsync_count{get;set;}

	public GameOptionsObjData()
	{
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

