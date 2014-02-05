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
            StartCoroutine(ToggleTimer(true,graphics.animation[open_animation].length));
		}
		else{
			graphics.animation.Play(close_animation);
            StartCoroutine(ToggleTimer(false,graphics.animation[open_animation].length));
		}

		return true;
	}

	public void Toggle ()
	{
		Open(!IsOpen);
	}

    IEnumerator ToggleTimer(bool open,float delay){
        if (!open){
            IsOpen=open;
            doorCollider.SetActive(!open);
        }
		anim_on=true;
		yield return new WaitForSeconds(delay);
		anim_on=false;

        if (open){
    		IsOpen=open;
            doorCollider.SetActive(!open);
        }
	}

	public override void Interact()
	{
		Toggle();
	}
}
