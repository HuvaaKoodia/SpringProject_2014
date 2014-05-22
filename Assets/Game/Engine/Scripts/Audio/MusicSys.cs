using UnityEngine;
using System.Collections;

public class MusicSys : MonoBehaviour {
	
	int game_track_index=0;
	public AudioClip menu_track;
	public AudioClip[] game_tracks;
	public AudioSource source;
	
	public float music_volume=0.5f;
	
	void Start(){	
	}
	
	public void StartMenuTrack(){
		startClip(menu_track);
	}
	
	public void StartGameTrack(){
		startClip(game_tracks[game_track_index]);
		
		game_track_index=Subs.Add(game_track_index,0,game_tracks.Length);
	}
	
	public void StopCurrent(){
		VolumeDec();
	}
	
	void startClip(AudioClip clip){
		source.clip=clip;
		VolumeInc();
		source.Play();
	}
	
	public void VolumeInc(){
		source.volume=0f;
		StartCoroutine(volumeChange(0.1f,1f,music_volume));
	}
	public void VolumeDec(){
		//source.volume=music_volume;
		StartCoroutine(volumeChange(-0.1f,0.2f,0f));
	}
	
	IEnumerator volumeChange(float amount,float seconds,float target){
		
		bool on =true;
		while (on){
			yield return new WaitForSeconds(seconds);
			source.volume+=music_volume*amount;
			on=source.volume<target;
			if (amount<0){
				on=source.volume>target;
				if (!on)
					source.Stop();
			}
		}
	}
}
