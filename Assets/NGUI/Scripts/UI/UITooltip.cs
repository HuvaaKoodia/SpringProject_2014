using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Example script that can be used to show tooltips.
/// </summary>

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	static protected UITooltipInstance mInstance,mInstance2;
    public static float y_off=-60;

    public UITooltipInstance Tooltip1,Tooltip2;

    void Awake () {
		SetMembersToStatics();
	}

	public void SetMembersToStatics ()
	{
		mInstance = Tooltip1;mInstance2=Tooltip2; 
	}

    static public void ClearTexts ()
    {
        if (mInstance != null) mInstance.SetText(null,Vector3.zero);
        if (mInstance2 != null) mInstance2.SetText(null,Vector3.zero);
    }
	
	static public void ShowText (string tooltipText)
	{
        if (mInstance != null) mInstance.SetText(tooltipText,new Vector3(0,y_off));
	}

    static public void ShowText (string tooltipText,string compareString)
    {
        if (mInstance == null||mInstance2==null) return;

        mInstance.SetText(tooltipText,new Vector3(-1,y_off));
        mInstance2.SetText(compareString,new Vector3(0,y_off));
    }
}
