using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLDataLoader : XML_Loader
{
    public static void Read(XmlDatabase database)
    {
		var DOX=GetAllXmlDocuments("Data/");

		foreach (var Xdoc in DOX)
        {
            var root = Xdoc["Root"];
            if (root==null) continue;

            foreach (XmlNode node in root)
            {
				#region Player
				if (node.Name == "Player")
                {
                    int playerHealth = getAttInt(node, "Health");

                    float movementSpeed = getAttFlt(node, "MovementSpeed");
                    float turnSpeed = getAttFlt(node, "TurnSpeed");

                    PlayerXmlData newPlayer = new PlayerXmlData(playerHealth, movementSpeed, turnSpeed);
                    database.players.Add(newPlayer);
                    continue;
                }
				#endregion

				#region Enemy
				if (node.Name == "Enemy")
                {
                    string enemyType = getAttStr(node, "Type");

                    int health = getAttInt(node, "Health");
                    int damage = getAttInt(node, "Damage");

                    EnemyXmlData newEnemy = new EnemyXmlData(enemyType, health, damage);
                    database.enemies.Add(newEnemy);
                    continue;
                }
				#endregion
				#region Obstacle
                if (node.Name == "Obstacle")
                {
                    string obstacleType = getAttStr(node, "Type");

                    int health = getAttInt(node, "Health");

                    ObstacleXmlData newObstacle = new ObstacleXmlData(obstacleType, health);
                    database.obstacles.Add(newObstacle);
                    continue;
                }
				#endregion

                if (ReadWeapon(node,database))      continue;
                if (ReadQuestItem(node,database))   continue;
                if (ReadMission(node,database))     continue;
                if (ReadObjective(node,database))   continue;
                if (ReadAmmo(node,database))        continue;
            }
		}
    }

    static bool ReadWeapon (XmlNode node, XmlDatabase database)
	{
		if (node.Name == "Weapon")
		{
			InvBaseItem item=new InvBaseItem();
			item.name=getAttStr(node,"name");
			item.type=(InvBaseItem.Type)System.Enum.Parse(typeof(InvBaseItem.Type),getAttStr(node,"type"),true);
			item.description=getStr(node,"Description");
			item.iconName=getAttStr(node,"sprite");
            item.ammotype=getStr(node,"Ammo");

			foreach(var t in Subs.EnumValues<InvStat.Type>()){
				AddStat(node,item,t);
			}
			database.items.Add(item);
            return true;
		}
        return false;
	}

    static bool ReadAmmo(XmlNode node, XmlDatabase database)
    {
        if (node.Name == "Ammo")
        {
            var data=new AmmoXmlData();
            data.Type=getAttStr(node,"type");
            data.Name=getAttStr(node,"name");
            data.MaxAmount=getAttInt(node,"maxamount");

            data.StartAmount=getAttInt(node,"startamount",data.MaxAmount);
            data.ShowInGame=getAttBool(node,"showingame",true);

            database.AmmoTypes.Add(data.Type,data);
            return true;
        }
        return false;
    }

    static bool ReadQuestItem(XmlNode node, XmlDatabase database)
    {
        if (node.Name == "Item")
        {
            InvBaseItem item=new InvBaseItem();
            item.name=getAttStr(node,"name");
            item.type=(InvBaseItem.Type)System.Enum.Parse(typeof(InvBaseItem.Type),getAttStr(node,"type"),true);
            item.description=getStr(node,"Description");
            item.iconName=getAttStr(node,"sprite");
            
            database.QuestItems.Add(item.name,item);
            return true;
        }
        return false;
    }

    static bool ReadMission (XmlNode node, XmlDatabase database)
    {
        if (node.Name == "Mission")
        {
            var mission=new MissionXmlData();
            var type=(MissionObjData.Type)System.Enum.Parse(typeof(MissionObjData.Type),getAttStr(node,"type"),true);
            mission.Description=getStr(node,"Description").Replace("\\n","\n");

            string[] spl;

            var strs=node["PrimaryObjectives"];
            if (strs!=null){
                spl=Subs.SplitAndTrim(strs.InnerText,"\n");
                
                foreach(var n in spl){
                    mission.PrimaryObjectives.Add(n);
                }
            }

            strs=node["SecondaryObjectives"];
            if (strs!=null){
                spl=Subs.SplitAndTrim(strs.InnerText,"\n");
                
                foreach(var n in spl){
                    mission.SecondaryObjectives.Add(n);
                }
            }

            strs=node["Ships"];
            spl=Subs.SplitAndTrim(strs.InnerText,"\n");
            
            foreach(var n in spl){
                mission.ShipsTypes.Add(n);
            }

            database.Missions.Add(type,mission);
            return true;
        }
        return false;
    }

    
    static bool ReadObjective (XmlNode node, XmlDatabase database)
    {
        if (node.Name == "Objective")
        {
            var data=new ObjectiveXmlData();
            var type=(MissionObjData.Objective)System.Enum.Parse(typeof(MissionObjData.Objective),getAttStr(node,"type"),true);
            data.Name=getAttStr(node,"Name");

            data.Room=getStr(node,"Room","room");
            data.Item=getStr(node,"Item","");
            
            database.Objectives.Add(type,data);
            return true;
        }
        return false;
    }

	static void AddStat(XmlNode node,InvBaseItem item,InvStat.Type type){
		var n1=node[type.ToString()];
		if (n1!=null){
			var stat=new InvStat();
			stat.type=type;
            if (HasAtt(n1,"min")) stat.min_amount=getAttInt(n1,"min");
            if (HasAtt(n1,"max")) stat.max_amount=getAttInt(n1,"max");
            if (HasAtt(n1,"minmax")) stat.min_amount=stat.max_amount=getAttInt(n1,"minmax");
			item.stats.Add(stat);
		}
	}

    public static void ReadConstants()
    {
        readAutoFileStatic("Data","Constants",typeof(XmlDatabase),"Constants");

        var Doc=GetXmlDocument("Data","ShipGenerator");
        var root=Doc["RoomIndices"];
        ShipGenerator.RoomIndices=new Dictionary<string, RoomXmlIndex>();

        var tags=getChildrenByTag(root,"Tag");
        foreach(var tag in tags){

            var stats=new RoomXmlIndex();
            stats.index=tag.Attributes["index"].Value;
            stats.name=tag.Attributes["name"].Value;
            stats.type=tag.Attributes["type"].Value;

            stats.randomize_pos=getAttBool(tag,"random_pos",false);
            stats.randomize_doors=getAttBool(tag,"random_doors",false);

            ShipGenerator.RoomIndices.Add(stats.index,stats);
        }
    }
}
