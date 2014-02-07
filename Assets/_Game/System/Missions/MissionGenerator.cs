using UnityEngine;
using System.Collections;

public class MissionGenerator{

    public static MissionObjData GenerateMission(){
        var mission=new MissionObjData();

        mission.MissionType=            Subs.GetRandomEnum<MissionObjData.Type>();
        mission.MissionAlienAmount=     Subs.GetRandomEnum<MissionObjData.AlienAmount>();
        mission.MissionSecuritySystem=  Subs.GetRandomEnum<MissionObjData.SecuritySystems>();
        mission.MissionShipPower=       Subs.GetRandomEnum<MissionObjData.ShipPower>();
        mission.MissionShipConditions=  Subs.GetRandomEnum<MissionObjData.ShipCondition>();

        mission.InfoAlienAmount=GetRandomInfo();
        mission.InfoSecuritySystem=GetRandomInfo();
        mission.InfoShipPower=GetRandomInfo();
        mission.InfoShipConditions=GetRandomInfo();

        //objectives
        switch (mission.MissionType){
            case MissionObjData.Type.TradeVesselInfo:
                mission.PrimaryObjectives.Add(MissionObjData.Objective.FindLogs);
                mission.SecondaryObjectives.Add(MissionObjData.Objective.Loot);
                break;
                
            case MissionObjData.Type.RetrieveCargo:
                mission.PrimaryObjectives.Add(MissionObjData.Objective.FindItem);
                mission.SecondaryObjectives.Add(MissionObjData.Objective.Loot);
                break;

            case MissionObjData.Type.ExploreVessel:
                mission.PrimaryObjectives.Add(MissionObjData.Objective.Loot);
                mission.PrimaryObjectives.Add(MissionObjData.Objective.Explore);
                break;

            case MissionObjData.Type.EmergencyBeacon:
                mission.PrimaryObjectives.Add(MissionObjData.Objective.FindLogs);
                mission.SecondaryObjectives.Add(MissionObjData.Objective.Explore);
                mission.SecondaryObjectives.Add(MissionObjData.Objective.Loot);
                break;
        }

        mission.Info=MissionDebriefText(mission);
        return mission;
    }

    static MissionObjData.InformationRating GetRandomInfo(){
        var ScanningRating= Subs.RandomPercent();
        if (ScanningRating<XmlDatabase.MissionInfoFailRating) return MissionObjData.InformationRating.None;
        if (ScanningRating>=XmlDatabase.MissionInfoSuccessRating) return MissionObjData.InformationRating.Everything;
        return MissionObjData.InformationRating.Something;
    }


    /// <summary>
    /// Swich case from hell!
    /// </summary>
    static string MissionDebriefText(MissionObjData mission){

        string base_text="";

        switch(mission.MissionType){
            case MissionObjData.Type.TradeVesselInfo:
                base_text="We've been contracted to investigate the fate of a trade vessel.\nFind out what happened.\n\n" +
                    "The client doesn't care about the cargo. Grab whatever you can.";
                break;
                
            case MissionObjData.Type.RetrieveCargo:
                base_text="We've been contracted to retrieve a certain valuable item from a abandoned vessel.\n" +
                    "Anything else you find is a for us to keep.";
                break;
                
            case MissionObjData.Type.ExploreVessel:
                base_text="We've spotted a derelict vessel.\n" +
                    "Loot and explore!";
                break;
                
            case MissionObjData.Type.EmergencyBeacon:
                base_text="We've received an emergency message from deep space.\n" +
                    "Investigate and explore!";
                break;
        }

        base_text+="\n";

        string info_text="";

        switch(mission.InfoAlienAmount){
            case MissionObjData.InformationRating.None:
                info_text+="We were unable to determine organic presence.";
                break;
            case MissionObjData.InformationRating.Something:
                info_text+=MissionAlienInfoSomething(mission);
                break;
            case MissionObjData.InformationRating.Everything:
                info_text+=MissionAlienInfoEverything(mission);
                break;
        }
        info_text+="\n";
        switch(mission.InfoSecuritySystem){
            case MissionObjData.InformationRating.None:
                info_text+="We were unable to determine the security system status.";
                break;
            case MissionObjData.InformationRating.Something:
                info_text+=MissionSecurityInfoSomething(mission);
                break;
            case MissionObjData.InformationRating.Everything:
                info_text+=MissionSecurityInfoEverything(mission);
                break;
        }
        info_text+="\n";
        switch(mission.InfoShipPower){
            case MissionObjData.InformationRating.None:
            case MissionObjData.InformationRating.Something:
                info_text+="We were unable to determine the power status of the ship.";
                break;
            case MissionObjData.InformationRating.Everything:
                info_text+=MissionPowerInfoEverything(mission);
                break;
        }
        info_text+="\n";
        switch(mission.InfoShipConditions){
            case MissionObjData.InformationRating.None:
                info_text+="We were unable to determine the ship condition.";
                break;
            case MissionObjData.InformationRating.Something:
                info_text+=MissionConditionInfoSomething(mission);
                break;
            case MissionObjData.InformationRating.Everything:
                info_text+=MissionConditionInfoEverything(mission);
                break;
        }


        return base_text+info_text;
    }
    //aliens
    static string MissionAlienInfoSomething(MissionObjData mission){
        switch(mission.MissionAlienAmount){
            case MissionObjData.AlienAmount.None:
            case MissionObjData.AlienAmount.Small:
                return "The ship might have some organics.";
            case MissionObjData.AlienAmount.Medium:
            case MissionObjData.AlienAmount.Large:
                return "The ship has an undetermined amount of organics.";
        }
        return "";
    }

    static string MissionAlienInfoEverything(MissionObjData mission){
        switch(mission.MissionAlienAmount){
            case MissionObjData.AlienAmount.None:
                return "The ship has no organic signatures.";
            case MissionObjData.AlienAmount.Small:
                return "The ship a small organic signature.";
            case MissionObjData.AlienAmount.Medium:
                return "The ship has a medium organic signature.";
            case MissionObjData.AlienAmount.Large:
                return "The ship is crawling with organics.";
        }
        return "";
    }
    //security
    static string MissionSecurityInfoSomething(MissionObjData mission){
        switch(mission.MissionSecuritySystem){
            case MissionObjData.SecuritySystems.None:
            case MissionObjData.SecuritySystems.Small:
                return "The ship might have a simple security system.";
            case MissionObjData.SecuritySystems.Medium:
            case MissionObjData.SecuritySystems.Large:
                return "The ship has an advanced security system of some sort.";
        }
        return "";
    }
    
    static string MissionSecurityInfoEverything(MissionObjData mission){
        switch(mission.MissionSecuritySystem){
            case MissionObjData.SecuritySystems.None:
                return "The ship doesn't have a security system.";
            case MissionObjData.SecuritySystems.Small:
                return "The ship has a simple security system.";
            case MissionObjData.SecuritySystems.Medium:
                return "The ship has an average security system.";
            case MissionObjData.SecuritySystems.Large:
                return "The ship has an advanced security system.";
        }
        return "";
    }
    //power
    static string MissionPowerInfoEverything(MissionObjData mission){
        switch(mission.MissionShipPower){
            case MissionObjData.ShipPower.On:
                return "The ship has no power.";
            case MissionObjData.ShipPower.Off:
                return "The ship has power.";
        }
                
        return "";
    }
    //condition
    static string MissionConditionInfoSomething(MissionObjData mission){
        string t1="The ship might be damaged.",t2="The ship is damaged to some degree.";
        switch(mission.MissionShipConditions){
            case MissionObjData.ShipCondition.Intact:
                return t1;
            case MissionObjData.ShipCondition.Damaged:
                return Subs.RandomBool()?t1:t2;
            case MissionObjData.ShipCondition.BadlyDamaged:
                return t2;
        }
        return "";
    }
    
    static string MissionConditionInfoEverything(MissionObjData mission){
        switch(mission.MissionShipConditions){
            case MissionObjData.ShipCondition.Intact:
                return "The ship is intact.";
            case MissionObjData.ShipCondition.Damaged:
                return "The ship is damaged.";
            case MissionObjData.ShipCondition.BadlyDamaged:
                return "The ship is badly damaged.";
        }
        return "";
    }
}
