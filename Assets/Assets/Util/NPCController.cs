using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {
	public static string SAVE_STRING = "NPC Enemies List ";

	public GameObject npcPrefab;
	public int numToSpawn=30;
	
	private Map _map;

	private NPCUnit[] npcs;
	
	// Use this for initialization
	public void Init (Map map) {
		_map = map;
		npcs = null;

		npcs= new NPCUnit[numToSpawn];
		for (int i = 0; i<numToSpawn; i++) {
			GameObject go = (GameObject)Instantiate(npcPrefab);
			
			NPCMovementController npc = go.GetComponent<NPCMovementController>();
			NPCAppearanceHandler npcA = go.GetComponent<NPCAppearanceHandler>();
			
			if(npcA!=null){
				npcA.Init();
			}
			
			if(npc!=null){
				npc.SetMap(_map);
				npc.Init();
				
				//Naming for ease
				npc.gameObject.name = i.ToString("0000"); //assuming we have less than 10,000
				npcs[i] = npc.GetComponent<NPCUnit>();
			}
		}
	}

	//Load Info Stuff//
	public void CreateNPCArray(int length){
		npcs = new NPCUnit[length];
		numToSpawn = length;
	}

	public void SetMap(Map map){
		_map = map;
	}

	public void AddNPC(int npcIndex, Vector3 position, Vector3[] path, int colorIndex){
		GameObject go = (GameObject)Instantiate(npcPrefab);
		NPCMovementController npc = go.GetComponent<NPCMovementController>();

		if(npc!=null){
			npc.SetMap(_map);
			
			npc.InitFromSave(position,path);

			//Naming for ease
			npc.gameObject.name = npcIndex.ToString("0000"); //assuming we have less than 10,000
			npcs[npcIndex] = npc.GetComponent<NPCUnit>();
		}

		NPCAppearanceHandler npcA = go.GetComponent<NPCAppearanceHandler>();

		if (npcA != null) {
			npcA.InitFromSave(colorIndex);		
		}
	}

	//Public Save String Stuff//

	public bool CanSave(){
		return npcs != null;
	}

	public string GetSaveString(){
		string saveString = "";
		if(npcs!=null){
			saveString+=SAVE_STRING+ numToSpawn+"\n";

			for (int i = 0; i<numToSpawn; i++) {
				saveString+=npcs[i].GetSaveString();		
			
			}
		}
		return saveString;
	}
}
