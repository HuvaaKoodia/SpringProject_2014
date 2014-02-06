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

				ReadWeapon(node,database);

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
    }
}
