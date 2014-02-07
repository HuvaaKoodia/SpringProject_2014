using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionObjData {
    public enum Type{TradeVesselInfo,RetrieveCargo,EmergencyBeacon,ExploreVessel}
    public enum AlienAmount{None,Small,Medium,Large}
    public enum SecuritySystems{None,Small,Medium,Large}
    public enum ShipCondition{Intact,Damaged,BadlyDamaged}
    public enum ShipPower{On,Off}

    public enum Objective{FindLogs,FindItem,Explore,Loot}

    public enum InformationRating{None,Something,Everything}

    public string Info;

    //stats
    public Type MissionType;
    public AlienAmount      MissionAlienAmount;
    public SecuritySystems  MissionSecuritySystem;
    public ShipCondition    MissionShipConditions;
    public ShipPower        MissionShipPower;

    //info
    public InformationRating InfoAlienAmount;
    public InformationRating InfoSecuritySystem;
    public InformationRating InfoShipConditions;
    public InformationRating InfoShipPower;

    //objectives
    public List<Objective> PrimaryObjectives=new List<Objective>();
    public List<Objective> SecondaryObjectives=new List<Objective>();

    public MissionObjData(){

    }

}
