using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLDataLoader : XML_Loader
{
    public static void Read()
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
					if (XmlDatabase.Player!=null){
						Debug.LogWarning("XmlDatabase PlayerData redefinition.");
					}
					else{
						var newPlayer = new PlayerXmlData();
						newPlayer.Health= getAttInt(node, "Health");

						newPlayer.MovementSpeed = getAttFlt(node, "MovementSpeed");
						newPlayer.TurnSpeed = getAttFlt(node, "TurnSpeed");

						newPlayer.StartingWeaponAmount=getAttFlt(node, "StartingWeaponAmount");
						newPlayer.StartingUtilityAmount=getAttFlt(node, "StartingUtilityAmount");
						XmlDatabase.Player=newPlayer;
	                    continue;
					}
                }
				#endregion

				#region Enemy
				if (node.Name == "Enemy")
                {
                    string enemyType = getAttStr(node, "Type");

                    int health = getAttInt(node, "Health");
                    int damage = getAttInt(node, "Damage");

                    EnemyXmlData newEnemy = new EnemyXmlData(enemyType, health, damage);
					XmlDatabase.enemies.Add(newEnemy);
                    continue;
                }
				#endregion
				#region Obstacle
                if (node.Name == "Obstacle")
                {
                    string obstacleType = getAttStr(node, "Type");

                    int health = getAttInt(node, "Health");

                    ObstacleXmlData newObstacle = new ObstacleXmlData(obstacleType, health);
					XmlDatabase.obstacles.Add(newObstacle);
                    continue;
                }
				#endregion

                if (ReadItem(node))       	continue;
                if (ReadMission(node))      continue;
                if (ReadObjective(node))    continue;
                if (ReadAmmo(node))         continue;
				if (ReadPools(node))		continue;
				if (ReadRewardClass(node))	continue;
            }
		}
    }

    static bool ReadItem (XmlNode node)
	{
		if (node.Name == "Item"||node.Name=="Weapon") //DEV. BWComp option
		{
			InvBaseItem item=new InvBaseItem();

			item.name=getAttStr(node,"name");
			item.type=(InvBaseItem.Type)System.Enum.Parse(typeof(InvBaseItem.Type),getAttStr(node,"type"),true);
			item.mesh=getAttStr(node,"mesh","none");
			item.description=getStr(node,"Description");
			item.iconName=getAttStr(node,"sprite");

            item.ammotype=getStr(node,"Ammo");
			foreach(var t in Subs.EnumValues<InvStat.Type>()){
				AddStat(node,item,t);
			}
			if (item.type==InvBaseItem.Type.QuestItem){
				XmlDatabase.AddQuestItem(item);
			}
			else{
				XmlDatabase.AddItem(item);
			}
            return true;
		}
        return false;
	}

    static bool ReadAmmo(XmlNode node)
    {
        if (node.Name == "Ammo")
        {
            var data=new AmmoXmlData();
            data.Type=getAttStr(node,"type");
            data.Name=getAttStr(node,"name");
            data.MaxAmount=getAttInt(node,"maxamount");

            data.StartAmount=getAttInt(node,"startamount",data.MaxAmount);
            data.ShowInGame=getAttBool(node,"showingame",true);

			XmlDatabase.AddAmmoType(data.Type,data);
            return true;
        }
        return false;
    }

    static bool ReadMission (XmlNode node)
    {
        if (node.Name == "Mission")
        {
            var mission=new MissionXmlData();
			var type=getAttStr(node,"type");
            mission.Description=getStr(node,"Description").Replace("\\n","\n");
            mission.RewardClass=getAttStr(node,"rewardClass");
			mission.LootPool=getAttStr(node,"lootPool");

            string[] spl;

            var temp=node["PrimaryObjectives"];
            if (temp!=null){
                spl=Subs.SplitAndTrim(temp.InnerText,"\n");
                
                foreach(var n in spl){
                    mission.PrimaryObjectives.Add(n);
                }
            }

            temp=node["SecondaryObjectives"];
            if (temp!=null){
                spl=Subs.SplitAndTrim(temp.InnerText,"\n");
                
                foreach(var n in spl){
                    mission.SecondaryObjectives.Add(n);
                }
            }

            temp=node["Ships"];
            spl=Subs.SplitAndTrim(temp.InnerText,"\n");
            
            foreach(var n in spl){
                mission.ShipsTypes.Add(n);
            }

			temp=node["TravelTime"];
			mission.TravelTimeMin=getAttInt(temp,"min");
			mission.TravelTimeMax=getAttInt(temp,"max");

			temp=node["ExpirationTime"];
			mission.ExpirationTimeMin=getAttInt(temp,"min");
			mission.ExpirationTimeMax=getAttInt(temp,"max");

			//status containers
			foreach(XmlNode n in node){
				loadPoolContainer(n,mission.StatusContainer);
				loadPool("LootQualityPool",n,mission.LootQualityPool);
			}

			XmlDatabase.AddMission(type,mission);
			return true;
        }
        return false;
    }
	
    static bool ReadObjective (XmlNode node)
    {
        if (node.Name == "Objective")
        {
            var data=new ObjectiveXmlData();
            var type=getAttStr(node,"type");
            data.Name=getAttStr(node,"name");

            data.Room=getStr(node,"Room","room");
            data.Item=getStr(node,"Item","");

			foreach(XmlNode n in node){
				if (n.Name=="Data"){
					data.Data.Add(n.InnerText);
				}
			}

			XmlDatabase.AddObjective(type,data);
            return true;
        }
        return false;
    }

	private static void AddStat(XmlNode node,InvBaseItem item,InvStat.Type type){
		var n1=node[type.ToString()];
		if (n1!=null){
			var stat=new InvStat();
			stat.type=type;
            if (HasAtt(n1,"min")) stat.min_amount=getAttInt(n1,"min");
            if (HasAtt(n1,"max")) stat.max_amount=getAttInt(n1,"max");
            if (HasAtt(n1,"minmax")) stat.min_amount=stat.max_amount=getAttInt(n1,"minmax");
			item.Stats.Add(stat);
		}
	}

    public static void ReadDataFiles()
    {
        ReadConstants();
        ReadObjectIndices();
        ReadRoomIndices();
    }

    static void ReadConstants()
    {
        readAutoFileStatic("Data","Constants",typeof(XmlDatabase),"Constants");
    }

    static void ReadRoomIndices()
    {
        var Doc=GetXmlDocument("Data","ShipGenerator");
        var root=Doc["ShipGenerator"]["RoomIndices"];
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

    static void ReadObjectIndices()
    {
        var Doc=GetXmlDocument("Data","ShipGenerator");

        var root=Doc["ShipGenerator"]["ObjectIndices"];
        ShipGenerator.ObjectIndices=new Dictionary<string, ObjectXmlIndex>();
        
        var tags=getChildrenByTag(root,"Tag");
        foreach(var tag in tags){
            var stats=new ObjectXmlIndex();
            stats.index=tag.Attributes["index"].Value;
            stats.type=(TileObjData.Obj)System.Enum.Parse(typeof(TileObjData.Obj),tag.Attributes["type"].Value);
			stats.rotation=getAttInt(tag,"rotation",0);
            
            ShipGenerator.ObjectIndices.Add(stats.index,stats);
        }
    }

	static bool ReadPools (XmlNode node)
	{
		if (loadPoolContainer(node,XmlDatabase.LootPool)) return true;
		if (loadPoolContainer(node,XmlDatabase.MissionPool)) return true;
		if (loadPoolContainer(node,XmlDatabase.LootQualityPool)) return true;
		return false;
	}
	
	private static bool loadPoolContainer(XmlNode node,PoolContainerXmlData container){
		if (node.Name == container.Name)
		{
			string pool=getAttStr(node,"name");
			if (!container.Pools.ContainsKey(pool)){
				container.AddPool(pool);
			}
			
			loadPool(node,container.Pools[pool]);
			return true;
		}
		return false;
	}

	private static void loadPool(XmlNode node,PoolXmlData pool){
		foreach(XmlNode n in node){
			if (n.Name=="Item"){
				pool.AddItem(new PoolItemXmlData(getAttStr(n,"name"),getAttInt(n,"weight",1)));
			}
		}
	}

	private static void loadPool(string Name,XmlNode node,PoolXmlData pool){
		if (node.Name==Name){
			loadPool(node,pool);
		}
	}

	static bool ReadRewardClass (XmlNode node)
	{
		if (node.Name == "RewardClass")
		{
			string name=getAttStr(node,"name");
			var rc=new RewardClassXmlData();
			rc.min=getAttInt(node,"min");
			rc.max=getAttInt(node,"max");
			XmlDatabase.AddRewardClass(name,rc);
			return true;
		}
		
		return false;
	}
}
