using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MyUtils
{

	//hideously slow as it iterates all objects, so don't overuse!
	public static GameObject FindInChildrenIncludingInactive(GameObject go, string name)
	{

		for (int i = 0; i < go.transform.childCount; i++)
		{
			if (go.transform.GetChild(i).gameObject.name == name) return go.transform.GetChild(i).gameObject;
			GameObject found = FindInChildrenIncludingInactive(go.transform.GetChild(i).gameObject, name);
			if (found != null) return found;
		}

		return null;  //couldn't find crap
	}

	//hideously slow as it iterates all objects, so don't overuse!
	public static GameObject FindIncludingInactive(string name)
	{
		Scene scene = SceneManager.GetActiveScene();
		var game_objects = new List<GameObject>();
		scene.GetRootGameObjects(game_objects);

		foreach (GameObject obj in game_objects)
		{
			GameObject found = FindInChildrenIncludingInactive(obj, name);
			if (found) return found;
		}

		return null;
	}

	//Breadth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c.name == aName)
				return c;
			foreach (Transform t in c)
				queue.Enqueue(t);
		}
		return null;
	}

	/*
	//Depth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		 foreach(Transform child in aParent)
		 {
			  if(child.name == aName )
					return child;
			  var result = child.FindDeepChild(aName);
			  if (result != null)
					return result;
		 }
		 return null;
	}
	*/
}

