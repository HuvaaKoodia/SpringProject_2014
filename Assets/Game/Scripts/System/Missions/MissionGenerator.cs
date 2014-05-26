using UnityEngine;
using System.Collections;

public class MissionGenerator{

    public static MissionObjData GenerateMission(string missionpool){

		var type=XmlDatabase.MissionPool.GetRandomItem(missionpool);
        var mission=new MissionObjData();
		mission.MissionPoolIndex=missionpool;

		mission.MissionType=type;
		
        //DEV.TEMP force type
        //mission.MissionType= MissionObjData.Type.RetrieveCargo;

		var xml=mission.XmlData;

		//mission status
		var enemy_types=xml.StatusContainer.GetRandomItem("EnemyTypes");
		var aliens=xml.StatusContainer.GetRandomItem("Aliens");
		var security=xml.StatusContainer.GetRandomItem("Security");
		var power=xml.StatusContainer.GetRandomItem("Power");
		var condition=xml.StatusContainer.GetRandomItem("Condition");

		mission.MissionEnemyTypes=    	(MissionObjData.EnemyTypes)int.Parse(enemy_types);
		mission.MissionAlienAmount=     (MissionObjData.AlienAmount)int.Parse(aliens);
		mission.MissionSecuritySystem=  (MissionObjData.SecuritySystems)int.Parse(security);
		mission.MissionShipPower=       (MissionObjData.ShipPower)int.Parse(power);
		mission.MissionShipConditions=  (MissionObjData.ShipCondition)int.Parse(condition);

		//reset status
		if (mission.MissionEnemyTypes==MissionObjData.EnemyTypes.Aliens) 
			mission.MissionSecuritySystem=MissionObjData.SecuritySystems.None;

		if (mission.MissionEnemyTypes==MissionObjData.EnemyTypes.Security) 
			mission.MissionAlienAmount=MissionObjData.AlienAmount.None;


		mission.LootQuality=xml.LootQualityPool.GetRandomItem();

		//known status info
		bool has_info=true;
		if (Subs.RandomPercent()<XmlDatabase.MissionCatastrophicIntelFailureChance) has_info=false;
		mission.NoInfo=!has_info;

        if (has_info){
			mission.InfoAlienAmount=GetRandomInfo();
        	mission.InfoSecuritySystem=GetRandomInfo();
        	mission.InfoShipPower=GetRandomInfo();
        	mission.InfoShipConditions=GetRandomInfo();
		}

		mission.MissionRisk=CalculateRisk(mission);

        //objectives

        foreach(var o in mission.XmlData.PrimaryObjectives){
            mission.AddPrimaryObjective(o);
        }

        foreach(var o in mission.XmlData.SecondaryObjectives){
            mission.AddSecondaryObjective(o);
        }

		//time 
		mission.TravelTime=Subs.GetRandom(xml.TravelTimeMin,xml.TravelTimeMax+1);
		mission.ExpirationTime=Subs.GetRandom(xml.ExpirationTimeMin,xml.ExpirationTimeMax+1);

		var rc=XmlDatabase.RewardClasses[xml.RewardClass];

		mission.Reward=(int)Mathf.Round(Subs.GetRandom(rc.min,rc.max)/(float)XmlDatabase.MissionRewardRounding)*XmlDatabase.MissionRewardRounding;

		//texts
        mission.Briefing=MissionBriefingText(mission);
        mission.Objectives=MissionObjectivesText(mission);
        return mission;
    }

    static MissionObjData.InformationRating GetRandomInfo(){
        var ScanningRating= Subs.RandomPercent();
        if (ScanningRating<XmlDatabase.MissionInfoFailRating) return MissionObjData.InformationRating.None;
        if (ScanningRating>=XmlDatabase.MissionInfoSuccessRating) return MissionObjData.InformationRating.Everything;
        return MissionObjData.InformationRating.Something;
    }
	
	static MissionObjData.Risk CalculateRisk(MissionObjData mission)
	{
		int risk=0,alien_risk=0,security_risk=0;
		//aliens
		if (mission.InfoAlienAmount>=MissionObjData.InformationRating.Something){
			alien_risk=(int)mission.MissionAlienAmount;

			if (mission.InfoAlienAmount==MissionObjData.InformationRating.Everything){
				alien_risk*=2;
			}
		}
		risk+=alien_risk;

		//security
		if (mission.InfoSecuritySystem>=MissionObjData.InformationRating.Something){
			security_risk=(int)mission.MissionSecuritySystem;
			
			if (mission.InfoSecuritySystem==MissionObjData.InformationRating.Everything){
				security_risk*=2;
			}
		}
		risk+=security_risk;

		//output
		return GetRiskEnum(risk); 
	}

	static MissionObjData.Risk GetRiskEnum(int risk)
	{
		//output
		if (risk<=1){
			return MissionObjData.Risk.Unknown;
		}
		else if (risk<2){
			return MissionObjData.Risk.Low;
		}
		else if (risk<4){
			return MissionObjData.Risk.Medium;
		}
		else if (risk<7){
			return MissionObjData.Risk.High;
		}
		return MissionObjData.Risk.Extreme;
	}

    /// <summary>
    /// Switch case from hell!
    /// </summary>
    static string MissionBriefingText(MissionObjData mission){

        string base_text=mission.XmlData.Description;

		string info_text="\n\n";

		if (mission.NoInfo){
			info_text+="No Additional info available:";
		}
		else{
			info_text+="Additional Info:\n\n";
			info_text+="- ";
			switch(mission.InfoAlienAmount){
	            case MissionObjData.InformationRating.None:
				info_text+="Organic presence: No data available";
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
	                info_text+="System security status: No data available";
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
	                info_text+="Ship power status: No data available";
	                break;
	            case MissionObjData.InformationRating.Everything:
	                info_text+=MissionPowerInfoEverything(mission);
	                break;
	        }
	        info_text+="\n- ";
	        switch(mission.InfoShipConditions){
	            case MissionObjData.InformationRating.None:
	                info_text+="Ship condition: No data available";
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
                return "Organic presence: Unconfirmed";
            case MissionObjData.AlienAmount.Medium:
            case MissionObjData.AlienAmount.Large:
                return "Organic presence: Confirmed - Amount unknown";
        }
        return "";
    }

    static string MissionAlienInfoEverything(MissionObjData mission){
        switch(mission.MissionAlienAmount){
            case MissionObjData.AlienAmount.None:
                return "Organic presence: None";
            case MissionObjData.AlienAmount.Small:
                return "Organic presence: Small";
            case MissionObjData.AlienAmount.Medium:
                return "Organic presence: Medium";
            case MissionObjData.AlienAmount.Large:
                return "Organic presence: High";
        }
        return "";
    }
    //security
    static string MissionSecurityInfoSomething(MissionObjData mission){
        switch(mission.MissionSecuritySystem){
            case MissionObjData.SecuritySystems.None:
            case MissionObjData.SecuritySystems.Small:
                return "System security status: Unconfirmed";
            case MissionObjData.SecuritySystems.Medium:
            case MissionObjData.SecuritySystems.Large:
                return "System security status: Confirmed - Amount unknown";
        }
        return "";
    }
    
    static string MissionSecurityInfoEverything(MissionObjData mission){
        switch(mission.MissionSecuritySystem){
            case MissionObjData.SecuritySystems.None:
				return "System security status: Offline";
            case MissionObjData.SecuritySystems.Small:
                return "System security status: Online - Weak";
            case MissionObjData.SecuritySystems.Medium:
                return "System security status: Online - Medium";
            case MissionObjData.SecuritySystems.Large:
                return "System security status: Online - Strong";
        }
        return "";
    }
    //power
    static string MissionPowerInfoEverything(MissionObjData mission){
        switch(mission.MissionShipPower){
            case MissionObjData.ShipPower.On:
                return "Ship power status: Online";
            case MissionObjData.ShipPower.Off:
                return "Ship power status: Offline";
        }
                
        return "";
    }
    //condition
    static string MissionConditionInfoSomething(MissionObjData mission){
		string t1="Ship condition: Unconfirmed",t2="Ship condition: Damages of uncertain scale";
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
                return "Ship condition: No visible damage";
            case MissionObjData.ShipCondition.Damaged:
                return "Ship condition: Damaged";
            case MissionObjData.ShipCondition.BadlyDamaged:
                return "Ship condition: Critically damaged";
        }
        return "";
    }

    static string MissionObjectivesText(MissionObjData mission)
    {
        var text="Primary:\n";
        foreach (var o in mission.PrimaryObjectives){
            text+=XmlDatabase.Objectives[o.Objective].Name+"\n";
        }
        if (mission.PrimaryObjectives.Count==0){
            text+="NONE\n";
        }
        
        text+="\n\nSecondary:\n";
        foreach (var o in mission.SecondaryObjectives){
			text+=XmlDatabase.Objectives[o.Objective].Name+"\n";
        }
        return text;
    }
	
	public static void UpdateMissionObjectiveStatus(MissionObjData mission,PlayerObjData player,GameController GC){
		var quest_items=player.Items.GetItems(item=>{return item.baseItem.type==InvBaseItem.Type.QuestItem;});

		foreach (var o in mission.PrimaryObjectives){
			o.status=0;
			bool complete=false;

			var xml=XmlDatabase.Objectives[o.Objective];

			if (xml.Item!=""){
				complete=false;
				//has specified item
				foreach(var qi in quest_items){
					if (qi.baseItem.name==xml.Item){
						complete=true;
						break;
					}
				}
				if (!complete) continue;
			}

			if (xml.Data.Count>0){
				complete=true;
				//has all required data
				foreach(var d in xml.Data){
					if (!player.HasDownloadData(d)){
						complete=false;
						break;
					}
				}
				if (!complete) continue;
			}

			if (xml.KillTypes.Count>0){
				complete=true;

				foreach (var t in xml.KillTypes){
					foreach (var f in GC.Floors){
						foreach (var e in f.Enemies){
							if (e.MyType==t) {
								complete=false;
								break;
							}
						}	
						if (!complete) break;
					}
					if (!complete) break;
				}

				if (!complete) continue;
			}

			if (complete) o.status=1;
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
			text+="\n";
		}
		
		return text;
    }

	public static string MissionShortDescription(MissionObjData mission){
		var text="";
		
		text+="Reward\n"+mission.Reward+" "+XmlDatabase.MoneyUnit;
		text+="\n\n";

		text+="Available for\n"+(mission.ExpirationTime)+" days";
		text+="\n\n";
		text+="Travel time\n"+mission.TravelTime +" days";

		text+="\n\nRisk\n";
		var risk=mission.MissionRisk;

		Color color=Color.grey;

		if (risk==MissionObjData.Risk.Low){
			color=Color.cyan;
		}
		
		if (risk==MissionObjData.Risk.Medium){
			color=Color.yellow;
		}
		
		if (risk==MissionObjData.Risk.High){
			color=new Color(250/255f,105/255f,0,1f);
		}
		
		if (risk==MissionObjData.Risk.Extreme){
			color=Color.red;
		}

		text+="["+NGUIText.EncodeColor(color)+"]"+risk+"[-]";

		return text;
	}

	//DEV. temp
	public static string MissionName (MissionObjData mission)
	{
		return mission.XmlData.Name;
	}
}
