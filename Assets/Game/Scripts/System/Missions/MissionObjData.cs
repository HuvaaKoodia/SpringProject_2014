using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionObjData {

    public MissionXmlData XmlData{get;set;}

    public enum Type{TradeVesselInfo,RetrieveCargo,EmergencyBeacon,ExploreVessel}
    public enum AlienAmount{None,Small,Medium,Large}
    public enum SecuritySystems{None,Small,Medium,Large}
    public enum ShipCondition{Intact,Damaged,BadlyDamaged}
    public enum ShipPower{On,Off}

    public enum Objective{FindLogs,FindItem,Explore,Loot}
   
    public enum InformationRating{None,Something,Everything}

	public string Briefing{get;set;}
	public string Objectives{get;set;}

    //stats
	public Type				MissionType	{get;set;}
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
	public List<Objective> PrimaryObjectives{get;set;}
	public List<Objective> SecondaryObjectives{get;set;}

    public MissionObjData(){
		PrimaryObjectives=new List<Objective>();
		SecondaryObjectives=new List<Objective>();
    }

    public bool ContainsObjective(MissionObjData.Objective o){
        return PrimaryObjectives.Contains(o)||SecondaryObjectives.Contains(o);
    }

}
