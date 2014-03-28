using System;
using System.Collections.Generic;

public class PoolXmlData
{
	string PoolName;
	public Dictionary<string,List<PoolItemXmlData>> Pool=new Dictionary<string, List<PoolItemXmlData>>();

	public PoolXmlData(string name){
		PoolName=name;
	}
	
	public PoolItemXmlData GetRandomItem (string pool)
	{
		if (!Pool.ContainsKey(pool)){
			UnityEngine.Debug.Log("Pool not found: "+pool + " in "+PoolName);
			return null;
		}
		int pool_size=0;
		foreach (var i in Pool[pool]){
			pool_size+=i.weight;
		}

		int r=Subs.GetRandom(pool_size);
		int t=0;
		foreach (var i in Pool[pool]){
			if (r<t+i.weight){
				return i;
			}
			t+=i.weight;
		}
		return null;
	}
}


