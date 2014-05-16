using System.Collections;

public enum Quality
{
	FASTEST = 0,
	FAST,
	SIMPLE,
	GOOD,
	BEAUTIFUL,
	FANTASTIC
};

public enum LightQuality
{
	LOW = 0,
	MEDIUM,
	HIGH
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
	ENABLE,
	FORCE_ENABLE
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
	NO_SYNC = 0,
	EVERY_VBLANK,
	EVERY_2ND_VBLANK
};

public class GameOptionsObjData
{
	public int quality_level{get;set;}
	public int pixel_light_count{get;set;}
	public int texture_quality{get;set;}
	public string anisotropic_textures{get;set;}
	public int anti_Aliasing{get;set;}
	public string shadow_projection{get;set;}
	public int shadow_cascades{get;set;}
	public float shadow_distance{get;set;}
	public int vsync_count{get;set;}

	public GameOptionsObjData()
	{
		quality_level = 0;
		pixel_light_count = 0;
		texture_quality = 0;
		anisotropic_textures = "";
		anti_Aliasing = 0;
		shadow_projection = "";
		shadow_cascades = 0;
		shadow_distance = 0.0f;
		vsync_count = 0;
	}
}

