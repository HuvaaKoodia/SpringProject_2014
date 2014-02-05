using UnityEngine;
using System.Collections;

public class DoorMain : InteractableMain {

	public bool IsOpen{get;private set;}
	public string open_animation,close_animation;
	public GameObject graphics;

	public GameObject doorCollider;

	bool anim_on;

	void Start(){
		IsOpen=false;
	}

	public bool Open(bool open){
		if (anim_on) return false;

		anim_on=false;
		if (open){
			graphics.animation.Play(open_animation);
			StartCoroutine(OpenTimer(graphics.animation[open_animation].length));
		}
		else{
			graphics.animation.Play(close_animation);
			IsOpen=false;
			doorCollider.SetActive(true);
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
		doorCollider.SetActive(false);
	}

	public override void Interact()
	{
		Toggle();
	}
}
