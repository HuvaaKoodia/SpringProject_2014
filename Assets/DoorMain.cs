using UnityEngine;
using System.Collections;

public class DoorMain : MonoBehaviour {

	public bool IsOpen{get;private set;}
	public string open_animation,close_animation;
	public GameObject graphics;

	bool anim_on;

	void Start(){
		IsOpen=false;
	}

	public bool Open(bool open){
		if (anim_on) return false;

		anim_on=false;
		if (open){
			graphics.animation[open_animation].normalizedTime = 0;
			graphics.animation[open_animation].speed=1;
			graphics.animation.Play(open_animation);
			StartCoroutine(OpenTimer(graphics.animation[open_animation].length));
		}
		else{
			graphics.animation[open_animation].normalizedTime = 1;
			graphics.animation[open_animation].speed=-1;
			graphics.animation.Play(open_animation);
			IsOpen=false;
		}

		return true;
	}

	public void Toggle ()
	{
		Open(!IsOpen);
	}

	IEnumerator OpenTimer(float delay){
		anim_on=true;
		yield return new WaitForSeconds(delay);
		anim_on=false;
		IsOpen=true;
	}
}
