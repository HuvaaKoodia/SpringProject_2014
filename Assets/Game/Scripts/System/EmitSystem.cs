using UnityEngine;
using System.Collections;

public class EmitSystem : MonoBehaviour {
	
	public GameObject emit;
	public float delay;
	public float limit;

	private bool emit_active;
	private float ticks = 0.0f;
	private float ticks1 = 0.0f;

	void Awake()
	{
		emit_active = false;
		emit.particleSystem.Pause();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//temporary
		if(Input.GetKeyDown(KeyCode.A))
		{
			ActivateSmoke();
		}

		if(emit_active)
		{
			UpdateTimer();
			EmitSmoke();
		}
	}

	public void ActivateSmoke()
	{
		emit_active = true;
		emit.particleSystem.Play();
	}

	private void EmitSmoke()
	{
		if(ticks >= delay)
		{
			emit.particleSystem.Play();
			ticks = 0.0f;
		}
		else
		{
			ticks += Time.deltaTime;
		}
	}

	private void UpdateTimer()
	{
		if(ticks1 >= limit)
		{
			emit_active = false;
			emit.particleSystem.Stop();
			ticks1 = 0.0f;
		}
		else
		{
			ticks1 += Time.deltaTime;
		}
	}
}
