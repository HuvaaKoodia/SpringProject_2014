using UnityEngine;
using System.Collections;

public class LootCrateMain : InteractableMain {

	public GameController GC;
    public InvItemStorage Items{get;private set;}
	public GameObject graphics;
	public bool Looted ()
	{
		graphics.transform.localScale=new Vector3(graphics.transform.localScale.x,1*0.8f,graphics.transform.localScale.z);
		graphics.renderer.material.color=Color.black;

		if (!GC.Inventory.LootParent.gameObject.activeSelf)
		{
			GC.Inventory.SetLoot(this);
			return true;
		}
		else
		{
			GC.menuHandler.DeactivateInventoryHUD();
			return false;
		}
	}

	// Use this for initialization
	void Awake () {
        Items=new InvItemStorage(8,4,2);
		InteractCost = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		interactSub.InteractFinished();
		return Looted();
	}
}
