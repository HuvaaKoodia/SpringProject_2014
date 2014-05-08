using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechWarningSystem : MonoBehaviour {
	
	public List<Light> warning_lights;																			//instantiate a list for the warning lights in the various TilePrefabs under TestObjects of GameScene																		
	public float delay;																							//instantiate time to delay by in inspector
	
	private float ticks;																						//instantiate time since last toggle
	private bool light_flicker;																					//instantiate boolean to allow light to flick
	private bool power_on=true;																					//instantiate boolean to allow electricity to flow

	public AudioClip warning_sound;

	// Use this for initialization
	void Start () {
		light_flicker = false;
		ticks = 0.0f;
		
		EnableLights();
	}
	
	// Update is called once per frame
	void Update () {
		if(power_on)
		{
			Flicker ();
			PlaySound();
		}
	}

	public bool PowerOn {
		get {return power_on;}
		set{
			power_on=value;
			EnableLights();
		}
	}

	public void EnableLights()
	{
		for(int i = 0; i < warning_lights.Count; i++)
		{
			var warning_light=warning_lights[i];
			bool light_on=true;
			
			if(warning_light != null)
			{
				if (power_on){
					light_on = light_flicker;
				}
				else{
					light_on = false;
				}
				
				warning_light.enabled=light_on;
			}
		}
	}
	
	//function to set white light to flicker
	private void Flicker()
	{
		//toggling of enabled value of the white lights in TilePrefabs under TestObjects in GameScene
		if(ticks >= delay)
		{
			light_flicker = !light_flicker;
			
			ticks = 0.0f;
			EnableLights();
			
			//set new random delay
			if (light_flicker){
				delay=Subs.GetRandom(10,100)*0.001f;
			}
			else{
				if (Subs.RandomPercent()<20){
					delay=Subs.GetRandom(10,100)*0.001f;
				}
				else
					delay=Subs.GetRandom(1000,4000)*0.001f;
			}
			
		}
		//increment ticks every second
		else
		{
			ticks += Time.deltaTime;
		}
	}

	private void PlaySound()
	{
		AudioSource.PlayClipAtPoint(warning_sound, transform.position);
	}
}
