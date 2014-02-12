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
                }
				#endregion
				#region Obstacle
                if (node.Name == "Obstacle")
                {
                    string obstacleType = getAttStr(node, "Type");

                    int health = getAttInt(node, "Health");

                    ObstacleXmlData newObstacle = new ObstacleXmlData(obstacleType, health);
                    database.obstacles.Add(newObstacle);
                }
				#endregion

                
                ReadWeapon(node,database);
                ReadMission(node,database);
            }
		}
    }

    static void ReadWeapon (XmlNode node, XmlDatabase database)
	{
		if (node.Name == "Weapon")
		{
			InvBaseItem item=new InvBaseItem();
			item.name=getAttStr(node,"name");
			item.type=(InvBaseItem.Type)System.Enum.Parse(typeof(InvBaseItem.Type),getAttStr(node,"type"),true);
			item.description=getStr(node,"Description");
			item.iconName=getAttStr(node,"sprite");

			foreach(var t in Subs.EnumValues<InvStat.Type>()){
				AddStat(node,item,t);
			}
			/*string weaponType = getAttStr(node, "Type");
                    string weaponName = getAttStr(node, "Name");

                    int damage = getAttInt(node, "Damage");
                    int accuracy = getAttInt(node, "Accuracy");
                    int heat = getAttInt(node, "Heat");

                    WeaponXmlData newWeapon = new WeaponXmlData(weaponType, weaponName, damage, accuracy, heat);
                    database.weapons.Add(newWeapon);
			 */
			database.items.Add(item);
		}

	}

    static void ReadMission (XmlNode node, XmlDatabase database)
    {
        if (node.Name == "Mission")
        {
            var mission=new MissionXmlData();
            var type=(MissionObjData.Type)System.Enum.Parse(typeof(MissionObjData.Type),getAttStr(node,"type"),true);
            mission.Description=getStr(node,"Description").Replace("\\n","\n");

            if (node["ObjectiveRoom"]==null)
                mission.ObjectiveRoom="room";//DEFAULT
            else
                mission.ObjectiveRoom=getStr(node,"ObjectiveRoom");

            var ships=node["Ships"];
            var text=ships.InnerText.Trim().Replace("\t","").Replace("\r","");
            var spl=Subs.Split(text,"\n");
            
           foreach(var s in spl){
                Debug.Log(s);
                mission.ShipsTypes.Add(s);
            }

            database.Missions.Add(type,mission);
        }
    }

	static void AddStat(XmlNode node,InvBaseItem item,InvStat.Type type){
		var n1=node[type.ToString()];
		if (n1!=null){
			var stat=new InvStat();
			stat.type=type;
			stat.min_amount=getAttInt(n1,"min");
			stat.max_amount=getAttInt(n1,"max");
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

            if (tag.Attributes["random_pos"]!=null){
                stats.randomize_pos=getAttBool(tag,"random_pos");
            }
            if (tag.Attributes["random_doors"]!=null){
                stats.randomize_doors=getAttBool(tag,"random_doors");
            }

            ShipGenerator.RoomIndices.Add(stats.index,stats);
        }
    }
}
