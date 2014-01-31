using UnityEngine;
using System.Collections;

public class Arbalest : WeaponMain {

	// Use this for initialization
	void Start () {

		MinDamage = 20;
		MaxDamage = 40;
		
		RateOfFire = 3;
		
		Accuracy = 90;
		
		HeatGeneration = 30;
		CoolingRate = 20;
		CurrentHeat = 0;
		
		MaxAmmo = 20;
		CurrentAmmo = MaxAmmo;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
