using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootCrateMain : InteractableMain {

	public GameController GC;
    public InvItemStorage Items{get;private set;}
	public GameObject graphics;

	public Animation openAnimation;
	public bool isOpen { get; private set; }

	public List<Light> Lights=new List<Light>();

	public bool Looted ()
	{
		if (openAnimation.isPlaying) return false;

		if (!isOpen)
		{
			isOpen = true;
			openAnimation.Play("Open_LootBox");
			audio.Play();
			Invoke ("SetLootToInventory", 0.9f);
			Invoke("FinishAnimation", 0.9f);
		}
		else{
			SetLootToInventory();
			FinishAnimation();
		}
		return true;
	}

	// Use this for initialization
	void Awake () {
        Items=new InvItemStorage(8,4,2);
		InteractCost = 0;

		isOpen = false;

		openAnimation["Open_LootBox"].normalizedTime = 0;
		openAnimation["Open_LootBox"].normalizedSpeed = 1;
	}
	void DisableLights(){
		foreach(var l in Lights){
			l.enabled=false;
		}
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		interactor = interactSub;
		return Looted();
	}

	void SetLootToInventory()
	{
		GC.Inventory.SetLoot(this);
	}

	void FinishAnimation()
	{
		interactor.InteractFinished();
		DisableLights();
	}
}
