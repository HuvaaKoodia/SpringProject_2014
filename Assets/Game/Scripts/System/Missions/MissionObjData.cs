﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionObjective{
	public string Objective{get;private set;}
	public int status{get;set;}

	public MissionObjective(){}

	public MissionObjective(string obj){
		Objective=obj;
	}
}

public class MissionObjData {
	
	public MissionXmlData XmlData{get{return XmlDatabase.GetMission(MissionType);}}
	
	public enum EnemyTypes{Aliens,Security,Both}
	public enum AlienAmount{None,Small,Medium,Large}
    public enum SecuritySystems{None,Small,Medium,Large}
	public enum ShipCondition{BadlyDamaged,Damaged,Intact}
	public enum ShipPower{Broken,Off,On}
	public enum Risk{Unknown,Low,Medium,High,Extreme}
   
    public enum InformationRating{None,Something,Everything}

	public bool NoInfo{get;set;}
	public string Briefing{get;set;}
	public string Objectives{get;set;}

	public string LootQuality{get;set;}

	public int TravelTime{get;set;}
	public int ExpirationTime{get;set;}	

	public int Reward{get;set;}
	public string MissionPoolIndex{get;set;}
	public Risk MissionRisk{get;set;}

    //stats
	public string			MissionType	{get;set;}
	public EnemyTypes		MissionEnemyTypes{get;set;}
	public AlienAmount      MissionAlienAmount {get;set;}
	public SecuritySystems  MissionSecuritySystem {get;set;}
	public ShipCondition    MissionShipConditions {get;set;}
	public ShipPower        MissionShipPower {get;set;}

    //info
	public InformationRating InfoAlienAmount {get;set;}
	public InformationRating InfoSecuritySystem {get;set;}
	public InformationRating InfoShipConditions {get;set;}
	public InformationRating InfoShipPower {get;set;}

    //objectives
	public List<MissionObjective> PrimaryObjectives{get;set;}
	public List<MissionObjective> SecondaryObjectives{get;set;}

    public MissionObjData(){
		NoInfo=false;
		PrimaryObjectives=new List<MissionObjective>();
		SecondaryObjectives=new List<MissionObjective>();
    }

	public void AddPrimaryObjective(string obj){
		PrimaryObjectives.Add(new MissionObjective(obj));
	}

	public void AddSecondaryObjective(string obj){
		SecondaryObjectives.Add(new MissionObjective(obj));
	}

	public bool ContainsObjective(string o){
		return PrimaryObjectives.Exists(obj=>obj.Objective==o)||SecondaryObjectives.Exists(obj=>obj.Objective==o);
    }
	
	//getters
	public float GetSecurityPercent ()
	{
		return (int)MissionSecuritySystem/3f;//Dev.magnum!
	}
	
	public float GetAlienPercent ()
	{
		return (int)MissionAlienAmount/3f;//Dev.magnum!
	}
}
