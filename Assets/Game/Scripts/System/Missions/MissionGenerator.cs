using UnityEngine;
using System.Collections;

public class MissionGenerator{

    public static MissionObjData GenerateMission(){
        var mission=new MissionObjData();

        mission.MissionType=            Subs.GetRandomEnum<MissionObjData.Type>();

        //DEV.TEMP force type
        //mission.MissionType= MissionObjData.Type.RetrieveCargo;

		mission.XmlData=XmlDatabase.Missions[mission.MissionType];

		//missions stats
        mission.MissionAlienAmount=     Subs.GetRandomEnum<MissionObjData.AlienAmount>();
        mission.MissionSecuritySystem=  Subs.GetRandomEnum<MissionObjData.SecuritySystems>();
        mission.MissionShipPower=       Subs.GetRandomEnum<MissionObjData.ShipPower>();
        mission.MissionShipConditions=  Subs.GetRandomEnum<MissionObjData.ShipCondition>();

		bool has_info=true;
		if (Subs.RandomPercent()<XmlDatabase.MissionCatastrophicIntelFailureChance) has_info=false;
		mission.NoInfo=true;
		//info stats
        if (has_info){
			mission.InfoAlienAmount=GetRandomInfo();
        	mission.InfoSecuritySystem=GetRandomInfo();
        	mission.InfoShipPower=GetRandomInfo();
        	mission.InfoShipConditions=GetRandomInfo();
		}
        //objectives

        foreach(var o in mission.XmlData.PrimaryObjectives){
            mission.AddPrimaryObjective((MissionObjData.Objective)System.Enum.Parse(typeof(MissionObjData.Objective),o,true));
        }

        foreach(var o in mission.XmlData.SecondaryObjectives){
            mission.AddSecondaryObjective((MissionObjData.Objective)System.Enum.Parse(typeof(MissionObjData.Objective),o,true));
        }

        mission.Briefing=MissionDebriefText(mission);
        mission.Objectives=MissionObjectivesText(mission);
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

        string base_text=mission.XmlData.Description;

		string info_text="\n\nAdditional Info:\n\n";

		if (mission.NoInfo){
			info_text="No Additional info available:";
		}
		else{
			info_text+="- ";
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
	        info_text+="\n- ";
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
	        info_text+="\n- ";
	        switch(mission.InfoShipPower){
	            case MissionObjData.InformationRating.None:
	            case MissionObjData.InformationRating.Something:
	                info_text+="We were unable to determine the power status of the ship.";
	                break;
	            case MissionObjData.InformationRating.Everything:
	                info_text+=MissionPowerInfoEverything(mission);
	                break;
	        }
	        info_text+="\n- ";
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
                return "The ship has a small organic signature.";
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

    static string MissionObjectivesText(MissionObjData mission)
    {
        var text="Primary:\n";
        foreach (var o in mission.PrimaryObjectives){
            text+=o.Objective.ToString()+"\n";
        }
        if (mission.PrimaryObjectives.Count==0){
            text+="NONE\n";
        }
        
        text+="\n\nSecondary:\n";
        foreach (var o in mission.SecondaryObjectives){
			text+=o.Objective.ToString()+"\n";
        }
        return text;
    }
	
	public static void UpdateMissionObjectiveStatus(MissionObjData mission,PlayerObjData player){
		var quest_items=player.Items.GetItems(item=>{return item.baseItem.type==InvBaseItem.Type.QuestItem;});

		foreach (var o in mission.PrimaryObjectives){
			o.status=0;
			if (o.Objective==MissionObjData.Objective.FindItem){
				var objective=XmlDatabase.Objectives[o.Objective];
				foreach(var qi in quest_items){
					if (qi.baseItem.name==objective.Item){
						o.status=1;
						break;
					}
				}
			}
		}
	}
	
    public static string MissionDebriefText(MissionObjData mission,PlayerObjData player){
        string text="";

		text+="Primary objectives:\n\n";
		foreach (var o in mission.PrimaryObjectives){
			var objective=XmlDatabase.Objectives[o.Objective];
			text+=objective.Name+": ";
			if (o.status==0){
				text+="NOT COMPLETED";
			}
			else{
				text+="COMPLETED";
			}
		}
		
		return text;
    }
}
