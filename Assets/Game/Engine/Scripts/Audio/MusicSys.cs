using UnityEngine;
using System.Collections;

public class MusicSys : MonoBehaviour {
	
	int game_track_index=0;
	public AudioClip menu_track;
	public AudioClip[] game_tracks;
	public AudioSource musicSource;
	public AudioSource elevatorSource;
	
	public float music_volume=0.5f;
	
	void Start(){
		musicSource.loop = true;
		elevatorSource.loop = false;
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
		musicSource.clip=clip;
		VolumeInc();
		musicSource.Play();
	}

	public void StartElevatorSound()
	{
		elevatorSource.Play();
	}
	
	public void VolumeInc(){
		musicSource.volume=0f;
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
			musicSource.volume+=music_volume*amount;
			on=musicSource.volume<target;
			if (amount<0){
				on=musicSource.volume>target;
				if (!on)
					musicSource.Stop();
			}
		}
	}
}
