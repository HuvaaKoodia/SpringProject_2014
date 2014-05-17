using UnityEngine;
using System.Collections;

public class DoorMain : InteractableMain {

    public GameController GC;
	public bool IsOpen{get;private set;}
	public string open_animation,close_animation;
	public GameObject graphics;

	public BoxCollider doorCollider;

    public bool isAirlockDoor=false,canOpenDoorOnStartUp=true;

	public bool anim_on { get; private set; }

	public AudioClip CloseSoundFX;
	public AudioClip OpenSoundFX;

	void Awake(){
		InteractCost = 1;
		IsOpen=false;
	}

	public bool Open(bool open){
		if (anim_on) return false;

		anim_on=false;
		if (open){
			graphics.animation[open_animation].speed = 1;
			graphics.animation.Play(open_animation);
			audio.PlayOneShot(OpenSoundFX);
            StartCoroutine(ToggleTimer(true,graphics.animation[open_animation].length));
			//Invoke("hitboxOff", graphics.animation[open_animation].length / 1.25f);
			hitboxOff();
		}
		else{
			graphics.animation[close_animation].speed = 1;
			graphics.animation.Play(close_animation);
			audio.PlayOneShot(CloseSoundFX);
            StartCoroutine(ToggleTimer(false,graphics.animation[close_animation].length));

			Invoke("hitboxOn", graphics.animation[close_animation].length*0.5f);
			//hitboxOn();
		}
		return true;
	}

	public void ForceOpen(){
		hitboxOff();
		IsOpen=true;
		var anim=graphics.animation[open_animation];

		anim.time = anim.length;
		anim.speed = 0;
		graphics.animation.Play(open_animation);
	}

    IEnumerator ToggleTimer(bool open,float delay){
        if (!open){
            IsOpen=open;
        }

		anim_on=true;
		yield return new WaitForSeconds(delay);
		anim_on=false;
		interactor.InteractFinished();
        if (open){
    		IsOpen=open;
        }
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		interactor = interactSub;

		if (isAirlockDoor)
		{
			GC.HUD.OpenEndMissionPanel();
			return false;
		}
		else
		{
			return Open(!IsOpen);
		}
	}

	void hitboxOff()
	{
		doorCollider.enabled = false;
	}

	void hitboxOn()
	{
		doorCollider.enabled = true;
	}
}
