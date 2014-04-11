using UnityEngine;
using System.Collections;

public class LootCrateMain : InteractableMain {

	public GameController GC;
    public InvItemStorage Items{get;private set;}
	public GameObject graphics;

	public Animation openAnimation;
	public bool isOpen { get; private set; }

	public bool Looted ()
	{
		if (!isOpen)
		{
			isOpen = true;
			openAnimation.Play("Open_LootBox");
		}

		if (!GC.Inventory.LootParent.gameObject.activeSelf)
		{
			if (openAnimation.isPlaying)
				Invoke ("SetLootToInventory", 0.9f);
			else
				SetLootToInventory();

			return true;
		}
		else
		{
			GC.HUD.DeactivateInventoryHUD();
			return false;
		}
	}

	// Use this for initialization
	void Awake () {
        Items=new InvItemStorage(8,4,2);
		InteractCost = 0;

		isOpen = false;

		openAnimation["Open_LootBox"].normalizedTime = 0;
		openAnimation["Open_LootBox"].normalizedSpeed = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		interactSub.InteractFinished();
		return Looted();
	}

	void SetLootToInventory()
	{
		GC.Inventory.SetLoot(this);
	}
}
