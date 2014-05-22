using UnityEngine;
using System.Collections;

public class InstantiateTooltips : MonoBehaviour {
	
	public UITooltipInstance TooltipInstancePrefab;
	public UITooltip TooltipParent;

	void Start () {
		var tt1=Instantiate(TooltipInstancePrefab) as UITooltipInstance;
		var tt2=Instantiate(TooltipInstancePrefab) as UITooltipInstance;

		tt1.transform.parent=TooltipParent.transform;
		tt2.transform.parent=TooltipParent.transform;
		
		tt1.transform.position=Vector3.one;
		tt2.transform.position=Vector3.one;
		
		tt1.transform.localScale=Vector3.one;
		tt2.transform.localScale=Vector3.one;

		TooltipParent.Tooltip1=tt1;
		TooltipParent.Tooltip2=tt2;
		TooltipParent.SetMembersToStatics();
	}
}
