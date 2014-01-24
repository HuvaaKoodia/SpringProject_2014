using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer{
	
	//class
	public delegate void TimerEvent();
	
	//object
	public bool Active=false;
	
	bool over;
	public bool OVER{
		get{
			if (over){
				over=false;
				return true;}
			return false;
		}
		private set{over=value;}
	}
	
	public float Delay{get{return delay;} set{delay=value;}}
	public float Percent{get{return tick/delay;}}
	public float Tick{get{return tick;}}
	public bool Destroyed{get;private set;}
	
	float delay,tick;
	public TimerEvent Timer_Event{get;set;}
	
	
	/// <summary>
	/// Creates an inactive timer.
	/// </summary>
	public Timer():this(null){}
	/// <summary>
	/// Creates an active timer with a certain delay.
	/// </summary>
	public Timer(int millis):this(millis,null){}
	
	/// <summary>
	/// Creates an inactive timer.
	/// </summary>
	public Timer(TimerEvent te){
		Timer_Event=te;
		OVER=false;
	}
	/// <summary>
	/// Creates an active timer with a certain delay.
	/// </summary>
	public Timer(int millis,TimerEvent te):this(te){
		delay=tick=millis;
		Active=true;
	}
	
	public void Update(float delta){
		if (!Active) return;
		tick-=delta*1000f;
		
		if (tick<=0){
			if (Timer_Event!=null)
				Timer_Event();
			OVER=true;
			Reset();
		}
	}
	
	public void Update(){
		Update(Time.deltaTime);
	}
	
	//subs
	
	public void Reset(){
		tick=delay;
	}
	public void Reset(bool active){
		Active=active;
		tick=delay;
	}
	
	public void Destroy(){
		Destroyed=true;
	}

	public void Done ()
	{
		over=true;
		Reset();
	}
}
