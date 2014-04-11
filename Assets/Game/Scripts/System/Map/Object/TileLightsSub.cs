using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//enumerator for the state of the white lights in TilePrefabs under TestObjects in GameScene
public enum Lighting_State
{
	Broken = 0,																									//broken lights will be off
	Flickering,																									//broken lights will be toggling between on and off
	Normal																										//normal lights will be on
}

public class TileLightsSub : MonoBehaviour
{	
	public List<Light> white_lights;																			//instantiate a list for the white lights in the various TilePrefabs under TestObjects of GameScene																		
	public float delay;																							//instantiate time to delay by in inspector

	private float ticks;																						//instantiate time since last toggle
	private bool light_flicker;																					//instantiate boolean to allow light to flick

	private bool power_on=true;																						//instantiate boolean to allow electricity to flow
	private Lighting_State lighting_state = Lighting_State.Normal;

	/// <summary>
	/// Power state property.
	/// Automatically updates the light.
	/// </summary>
	public bool PowerOn {
		get {return power_on;}
		set{
			power_on=value;
			EnableLights();
		}
	}

	/// <summary>
	/// Light state property.
	/// Automatically updates the light.
	/// </summary>
	public Lighting_State LightingState{
		get{
			return lighting_state;
		}
		set{
			lighting_state=value;
			EnableLights();
		}
	}
	
	void Start ()
	{
		light_flicker = false;
		ticks = 0.0f;

		EnableLights();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(power_on&&lighting_state == Lighting_State.Flickering)
		{
			Flicker (delay);
		}
	}

	/// <summary>
	/// Turns the light on/off based on its state and power setting.
	/// </summary>
	public void EnableLights()
	{
		for(int i = 0; i < white_lights.Count; i++)
		{
			var white_light=white_lights[i];
			bool light_on=true;

			if(white_light != null)
			{
			if (power_on){
					switch(lighting_state)
					{
					case Lighting_State.Broken:
						light_on = false;
						break;
					case Lighting_State.Flickering:
						light_on = light_flicker;
						break;
					case Lighting_State.Normal:
						light_on = true;
						break;
					}
				}
				else{
					light_on = false;
				}

				white_light.enabled=light_on;
			}
		}
	}

	//function to set white light to flicker
	private void Flicker(float delay)
	{
		//toggling of enabled value of the white lights in TilePrefabs under TestObjects in GameScene
		if(ticks >= delay)
		{
			light_flicker = !light_flicker;
			
			ticks = 0.0f;
			EnableLights();
		}
		//increment ticks every second
		else
		{
			ticks += Time.deltaTime;
		}
	}
}