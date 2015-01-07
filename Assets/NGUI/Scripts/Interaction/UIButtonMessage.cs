//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Message")]
public class UIButtonMessage : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
		OnDown,
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren = false;
	public bool useAutoIncrement = false;
	public float AutoIncrementDelay = 1;
	public float AutoIncrementDelayMulti = 0.8f;

	float autoIncrementTimer = 0;
	float autoIncrementDelay = 0;

	bool mStarted = false;

	public int OnPressInputIndex=0;

	void Start () 
	{
		mStarted = true; 
		autoIncrementDelay = AutoIncrementDelay;
	}

	void OnEnable () { if (mStarted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (((isOver && trigger == Trigger.OnMouseOver) ||
				(!isOver && trigger == Trigger.OnMouseOut))) Send();
		}
	}

	void OnPress (UICamera.InputEvent isPressed)
	{
		if (enabled&&OnPressInputIndex==isPressed.PressIndex)
		{
			if (((isPressed && trigger == Trigger.OnPress) ||
			     (!isPressed && trigger == Trigger.OnRelease))) Send();

			//if (trigger == Trigger.OnDown) Send();

			if (!isPressed)
			{
				autoIncrementTimer = 0;
				autoIncrementDelay = AutoIncrementDelay;
			}
		}
	}

	void OnDown (bool isPressed)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnDown)
			{
				if (useAutoIncrement)
				{
					if (autoIncrementTimer <= 0)
					{
						autoIncrementDelay *= AutoIncrementDelayMulti;
						if (autoIncrementDelay < 0.1f) autoIncrementDelay = 0.1f;
						autoIncrementTimer = autoIncrementDelay;

						Send();
					}
					autoIncrementTimer -= Time.deltaTime;
				}
				else
				{
					Send();
				}

			}
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}

	void OnClick () { if (enabled && trigger == Trigger.OnClick) Send(); }

	void OnDoubleClick () { if (enabled && trigger == Trigger.OnDoubleClick) Send(); }

	void Send ()
	{
		if (string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;

		if (includeChildren)
		{
			Transform[] transforms = target.GetComponentsInChildren<Transform>();

			for (int i = 0, imax = transforms.Length; i < imax; ++i)
			{
				Transform t = transforms[i];
				t.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
