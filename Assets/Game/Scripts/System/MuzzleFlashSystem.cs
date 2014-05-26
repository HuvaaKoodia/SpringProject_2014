using UnityEngine;
using System.Collections;

public class MuzzleFlashSystem : MonoBehaviour {

	bool on;
	public int FlashTimes=-1;
	public float life_time,flash_time_min,flash_time_max;
	public GameObject LightGO;

	public void Play(){
		on=true;
		StartCoroutine(Flash(FlashTimes));
		if (FlashTimes<=0) Invoke("Stop",life_time);
	}

	public void Stop(){
		on=false;
	}

	IEnumerator Flash(int times){

		while (on){
			times--;
			LightGO.SetActive(true);
			yield return new WaitForSeconds(Subs.GetRandom(flash_time_min,flash_time_max));
			LightGO.SetActive(false);
			yield return new WaitForSeconds(Subs.GetRandom(flash_time_min,flash_time_max)*2);

			if (times==0) break;
		}
	}

}
