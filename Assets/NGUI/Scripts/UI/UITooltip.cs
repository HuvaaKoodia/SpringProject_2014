using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Example script that can be used to show tooltips.
/// </summary>

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	static protected UITooltipInstance mInstance,mInstance2;

    public UITooltipInstance Tooltip1,Tooltip2;

    void Awake () { mInstance = Tooltip1;mInstance2=Tooltip2; }

    static public void ClearTexts ()
    {
        if (mInstance != null) mInstance.SetText(null,Vector3.zero);
        if (mInstance2 != null) mInstance2.SetText(null,Vector3.zero);
    }
	
	static public void ShowText (string tooltipText)
	{
		if (mInstance != null) mInstance.SetText(tooltipText,Vector3.zero);
	}

    static public void ShowText (string tooltipText,string compareString)
    {
        if (mInstance == null||mInstance2==null) return;

        mInstance.SetText(tooltipText,Vector3.left*110);
        mInstance2.SetText(compareString,Vector3.zero);
    }
}
