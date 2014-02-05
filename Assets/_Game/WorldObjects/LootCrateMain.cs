using UnityEngine;
using System.Collections;

public class LootCrateMain : InteractableMain {

    public InvItemStorage Items{get;private set;}
	public GameObject graphics;
	public void Looted ()
	{
		graphics.transform.localScale=new Vector3(graphics.transform.localScale.x,1*0.8f,graphics.transform.localScale.z);
		graphics.renderer.material.color=Color.black;
	}

	// Use this for initialization
	void Awake () {
        Items=new InvItemStorage(8,4,2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Interact()
	{
		Looted();
	}
}
