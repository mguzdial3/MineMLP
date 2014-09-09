using UnityEngine;
using System.Collections;

public class NPCUnit : MonoBehaviour {
	public static string SAVE_STRING = "NPC";
	public NPCMovementController movementController;
	public NPCAppearanceHandler appearanceController;


	public string GetSaveString(){
		string saveString = SAVE_STRING+" " + gameObject.name+" ";
		saveString += movementController.GetSaveString () + " ";
		saveString += appearanceController.GetSaveString () + "\n";
		return saveString;
	}
}
