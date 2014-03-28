using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//enumerator for the environment lights in TilePrefabs under TestObjects in GameScene
public enum Lighting_State
{
	Broken = 0,
	Flickering,
	Normal
}

public class AllLights : MonoBehaviour
{	
	public List<Light> white_lights;																			//instantiate a list for the white lights in the various TilePrefabs under TestObjects of GameScene
	
	private bool light_flicker;																					//instantiate boolean to allow light to flicker

	public float delay;																							//instantiate time to delay by in inspector
	private float ticks;																						//instantiate time since last toggle

	private bool power_on;																						//instantiate boolean to allow electricity to flow

	private Lighting_State lighting_state = Lighting_State.Normal;												//instantiate instance of Lighting_State to Normal

	float init_light_power = 4.0f;

	// Use this for initialization
	void Start ()
	{
		light_flicker = false;																					//initialize light to not flicker
		//power_on = true;																						//initialize electricity to flow
		ticks = 0.0f;																							//initialize value for ticks and set it to be 0.0f
	}
	//changes
	
	// Update is called once per frame
	void Update ()
	{
		//as long as state of light is currently set to flicker, call Flicker function
		if(lighting_state == Lighting_State.Flickering)
		{
			Flicker (delay);
		}
	}

	//function to enable the white lights in TilePrefabs under TestObjects in GameScene 
	//manipulation of white light intensity is passed in here
	//switch case handles the differernt light states in TilePrefabs under TestObjects in GameScene
	public void EnableLights(float light_power)
	{
		switch(lighting_state)
		{
		case Lighting_State.Broken:
			//as long as there is a white_lights
			if(white_lights.Count > 0)
			{				
				//traverse through the list of white_lights
				for(int i = 0; i < white_lights.Count; i++)
				{
					//as long as there is a white_lights, set value of enabled to value of on
					if(white_lights[i] != null)
					{
						white_lights[i].light.enabled = false;
						//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);			//debug to display value of white_lights enabled
					}
				}
			}
			break;
		case Lighting_State.Flickering:
			if(power_on)
			{
				//as long as there is a white_lights
				if(white_lights.Count > 0)
				{				
					//traverse through the list of white_lights
					for(int i = 0; i < white_lights.Count; i++)
					{
						//as long as there is a white_lights, set value of enabled to value of on
						if(white_lights[i] != null)
						{
							white_lights[i].light.enabled = light_flicker;
							white_lights[i].light.intensity = light_power;
							//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);			//debug to display value of white_lights enabled
						}
					}
				}
			}
			break;
		case Lighting_State.Normal:
			//Debug.Log("Power_on; " + power_on);
			if(power_on)
			{
				//as long as there is a white_lights
				if(white_lights.Count > 0)
				{				
					//traverse through the list of white_lights
					for(int i = 0; i < white_lights.Count; i++)
					{
						//as long as there is a white_lights, set value of enabled to value of on
						if(white_lights[i] != null)
						{
							white_lights[i].light.enabled = true;
							white_lights[i].light.intensity = light_power;
						}
						//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);				//debug to display value of white_lights enabled
					}
				}
			}
			break;
		default:
			Debug.Log("PASS IN Broken, Flickering OR Normal");
			Debug.Break();
			break;
		}
		
//		//as long as there is a white_lights
//		if(white_lights.Count > 0)
//		{				
//			//traverse through the list of white_lights
//			for(int i = 0; i < white_lights.Count; i++)
//			{
//				//as long as there is a white_lights, set value of enabled to value of on
//				if(white_lights[i] != null)
//				{
//					white_lights[i].light.enabled = power_on;
//					white_lights[i].light.intensity = light_power;
//					//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);					//debug to display value of white_lights enabled
//				}
//			}
//		}
	}

	//function to set white light to flicker
	private void Flicker(float delay)
	{
		//Debug.Log("TICKS: " + (int)ticks);																		//display value of ticks
		
		//toggling of enabled value of the white lights in TilePrefabs under TestObjects in GameScene
		if(ticks >= delay)
		{
			light_flicker = !light_flicker;
			//Debug.Log("ON: " + light_flicker);
			
			ticks = 0.0f;
			EnableLights(init_light_power);
		}
		//increment ticks every second
		else
		{
			ticks += Time.deltaTime;
		}
	}

	//function to set the state of the white lights
	public void SetState(Lighting_State LS)
	{
		lighting_state = LS;
	}

	//function to get the current state of the white light
	public Lighting_State GetState()
	{
		return lighting_state;
	}

	//function to set whether electricity flow
	public void SetPowerOn(bool on)
	{
		power_on = on;
	}

	//function to get current status of electricity flow
	public bool GetPowerOn()
	{
		return power_on;
	}

//	public Lighting_State RandomizeStartState(int broken_percent, int flickering_percent, int normal_percent)
//	{
//		int totalpercent = broken_percent + flickering_percent + normal_percent;
//		
//		if(totalpercent != 100)
//		{
//			Debug.Log("Percentages passed in do not add up to 100%");
//			return Lighting_State.Normal;
//		}
//		
//		int value = Subs.RandomPercent();
//		
//		if(value < broken_percent)
//		{
//			return Lighting_State.Broken;
//		}
//		else if(value >= broken_percent && value < flickering_percent)
//		{
//			return Lighting_State.Flickering;
//		}
//		else if(value >= flickering_percent && value < normal_percent)
//		{
//			return Lighting_State.Normal;
//		}
//		else
//		{
//			return Lighting_State.Normal;
//		}
//	}
}