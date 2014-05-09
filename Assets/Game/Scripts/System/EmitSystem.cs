using UnityEngine;
using System.Collections;

public class EmitSystem : MonoBehaviour {
	
	public GameObject emit;
	public float delay = 0.0f;
	public int limit = 0;
	public float max_delay = 0.0f;
	public int max_limit = 0;

	private bool emit_active = false;
	private float ticks = 0.0f;
//	private float ticks1 = 0.0f;
	private bool rand_delay = false;
	private int templimit;

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
			ActivateSmoke(true, false);
		}

		if(emit_active)
		{
			//UpdateTimer();

			if(rand_delay)
			{
				EmitSmoke(max_delay);
			}
			else
			{
				EmitSmoke();
			}
		}
	}

	public void ActivateSmoke(bool rand_delay, bool rand_limit)
	{
		emit_active = true;
		this.rand_delay = rand_delay;

		if(rand_limit)
		{
			templimit = Subs.GetRandom(max_limit);
		}
		else
		{
			templimit = limit;
		}
		//emit.particleSystem.Play();
	}

	private void EmitSmoke()
	{
		if(templimit > 0)
		{
			if(ticks >= delay)
			{
				emit.particleSystem.Play();
				templimit--;
				ticks = 0.0f;
			}
			else
			{
				ticks += Time.deltaTime;
			}
		}
	}

	private void EmitSmoke(float max_delay)
	{		
		if(templimit > 0)
		{
			if(ticks >= Subs.GetRandom(max_delay))
			{
				emit.particleSystem.Play();
				templimit--;
				ticks = 0.0f;
			}
			else
			{
				ticks += Time.deltaTime;
			}
		}
	}

//	private void UpdateTimer()
//	{
//		if(ticks1 >= limit)
//		{
//			emit_active = false;
//			emit.particleSystem.Stop();
//			ticks1 = 0.0f;
//		}
//		else
//		{
//			ticks1 += Time.deltaTime;
//		}
//	}
}
