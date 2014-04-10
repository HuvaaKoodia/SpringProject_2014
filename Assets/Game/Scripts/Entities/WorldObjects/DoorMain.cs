using UnityEngine;
using System.Collections;

public class DoorMain : InteractableMain {

    public GameController GC;
	public bool IsOpen{get;private set;}
	public string open_animation,close_animation;
	public GameObject graphics;

	public BoxCollider doorCollider;

    public bool isAirlockDoor=false;

	bool anim_on;

	void Start(){
		InteractCost = 1;
		IsOpen=false;
	}

	public bool Open(bool open){
		if (anim_on) return false;

		anim_on=false;
		if (open){
			graphics.animation.Play(open_animation);
            StartCoroutine(ToggleTimer(true,graphics.animation[open_animation].length));
			Invoke("hitboxOff", graphics.animation[open_animation].length / 1.25f);
		}
		else{
			graphics.animation.Play(close_animation);
            StartCoroutine(ToggleTimer(false,graphics.animation[open_animation].length));
			hitboxOn();
		}

		return true;
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
