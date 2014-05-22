using UnityEngine;
using System.Collections;

public class Instantiate_MechanicalMenu : MonoBehaviour {

	public MechanicalMenu MechanicalMenu_prefab;
	public Transform parent;
	public bool AllowBuying=true;

	MechanicalMenu instance;
	public MechanicalMenu Instance{get{return instance;}}

	void Awake(){
		instance=Instantiate(MechanicalMenu_prefab) as MechanicalMenu;

		instance.transform.parent=parent;
		instance.transform.localScale=Vector3.one;
		instance.transform.localPosition=Vector3.one;

		instance.allow_buying=AllowBuying;
	}
}
