using System;
using System.Collections.Generic;

public class PoolXmlData
{
	public Dictionary<string,List<PoolItemXmlData>> Pool=new Dictionary<string, List<PoolItemXmlData>>();

	public PoolItemXmlData GetRandomItem (string pool)
	{
		int pool_size=0;
		foreach (var i in Pool[pool]){
			pool_size+=i.weight;
		}

		int r=Subs.GetRandom(pool_size);
		
		foreach (var i in Pool[pool]){
			if (r<i.weight){
				return i;
			}
		}
		return null;
	}
}


