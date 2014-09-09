using UnityEngine;
using System.Collections;
using System.Text;
using System.IO; 

public class SnapshotHandler : MonoBehaviour {
	public NPCController npcController;//We need reference to this for saving info
	public Map map;
	public MapGenerator mapGenerator;

	//Cheat reset
	public bool resetSnapshot=true; //Reset the snapshot every time for now
	//Cheat for special load
	public bool specialSnapshotLoad;
	//Cheat to specify special load
	public string specialSnapshotString; //If not specified defaults to value stored in SNAPSHOT_TEXT PlayerPref

	private const string SNAPSHOT_NUMBER = "SNAPSHOT";//For saving via playerprefs
	private const string SNAPSHOT_TEXT = "SNAPSHOTSPECIAL";
	private const string MAP_SAVE = "MapSave";
	private const string MAP_UPDATE_SAVE = "MapUpdateSave";
	private const string MAP_UPDATE_SAVE_NUMBER = "MapUpdateSaveNumber";

	private const string START_OF_TEXT ="/Users/matthewguzdial/MinecraftClone/Snapshots/snapshot";
	private const string END_OF_TEXT =".txt";

	private int frames=0;
	private int frameToSave = 30;

	private string mapString;

	// Use this for initialization
	void Start () {
		if (resetSnapshot) {//We don't have a save
			PlayerPrefs.SetInt(SNAPSHOT_NUMBER,0);
			string mapSaveString="";
			while(!map.HasReachedBounds()){ //Generate the whole map

				mapGenerator.SpawnMap();
				mapSaveString += map.GetChangeString();
				
			}
			SpecialSave(MAP_SAVE,mapSaveString);
			npcController.Init (map);

		}
		else{
			//Load the base of the map
			LoadMap();

			//Load all changes to the map


			//Lighting
			for(int i = map.GetMinX(); i<map.GetMaxX(); i++){
				for(int j = map.GetMinZ(); j<map.GetMaxZ(); j++){
					LightComputer.ComputeSolarLighting(map,i,j);
					LightComputer.SetLightDirty(map,i,0,j);
				}
			
			}



			//Load Enemy Data
			if(specialSnapshotLoad){
				if(string.IsNullOrEmpty(specialSnapshotString)){
					Debug.LogWarning("[SnapshotHandler] Did not specify a special snapshot string");

					string toLoad = GetSavedSpecialString();

					if(string.IsNullOrEmpty(toLoad)){
						Debug.LogError("[SnapshotHandler] No special snapshot string saved");
					}
					else{
						Load (toLoad);
					}
				}
				else{
					Load (specialSnapshotString);
				}
			}
			else{
				Load (GetCurrentSnapshotNumber().ToString());
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (npcController.CanSave () && map.CanSave() && frames>=frameToSave) { //For now save once per frame while we can
			DoSave();
			frames=0;
		}

		frames++;
	}

	void DoSave(){

		IncrementSnapshotNumber();

		if (map.HasChangeString ()) {
			string textToSave = map.GetChangeString();


		}
		RegularSave();

		
	}

	private void RegularSave(){
		SaveSnapshot (GetCurrentSnapshotNumber ().ToString ());
	}

	private void SaveSnapshot(string snapshot, string textToSave=null){
		if(textToSave==null){
			textToSave = GetSaveText();
		}

		System.IO.File.WriteAllText(START_OF_TEXT+snapshot+END_OF_TEXT, textToSave);
	}

	private string GetSaveText(){
		string toSaveText = "";

		toSaveText += npcController.GetSaveString();
		toSaveText += "\n";

		return toSaveText;
	}

	private bool Load(string snapshotName){
		string fileName = START_OF_TEXT + snapshotName + END_OF_TEXT;

		// Handle any problems that might arise when reading the text
		try{
			string line;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader){

				// While there's lines left in the text file, do this:
				do{
					line = theReader.ReadLine();
					
					if (!string.IsNullOrEmpty(line)){// Do whatever you need to do with the text line, it's a string now
						if(line.Contains(NPCController.SAVE_STRING)){
							string numOfEnemies = line.Substring(NPCController.SAVE_STRING.Length);

							npcController.CreateNPCArray(int.Parse(numOfEnemies));
							npcController.SetMap(map);
						}
						else if(line.Contains(NPCUnit.SAVE_STRING)){ //Its an NPC! Let's deal with that
							int npcIndex=0;
							Vector3 position= Vector3.zero;
							Vector3[] path = null;
							int appearanceIndex=0;

							//Appearance deal
							int appearanceStringIndex = line.IndexOf(NPCAppearanceHandler.SAVE_STRING);
							string appearanceString = line.Substring(appearanceStringIndex+NPCAppearanceHandler.SAVE_STRING.Length);
							appearanceIndex = int.Parse(appearanceString);

							//Index
							string npcIndexString = line.Substring(0, NPCUnit.SAVE_STRING.Length+6);
							npcIndexString = npcIndexString.Substring(NPCUnit.SAVE_STRING.Length+1);
							npcIndex = int.Parse(npcIndexString);

							//Position
							string movementString = line.Substring(0, appearanceStringIndex);
							int movementStringIndex = movementString.IndexOf(NPCMovementController.SAVE_STRING);
							movementString = movementString.Substring(movementStringIndex+NPCMovementController.SAVE_STRING.Length);
							string[] result = movementString.Split(new string[]{" "}, System.StringSplitOptions.RemoveEmptyEntries);
							position.x=float.Parse(result[0]);
							position.y=float.Parse(result[1]);
							position.z=float.Parse(result[2]);

							if(result.Length>3){
								path= new Vector3[(result.Length-3)/3];
								for(int i = 3; i<result.Length; i+=3){
									path[((i-3)/3)] = new Vector3(float.Parse(result[i]),float.Parse(result[i+1]), float.Parse(result[i+2]));
								}
							}

							npcController.AddNPC(npcIndex,position,path,appearanceIndex);

						}
					}
				}
				while (line != null);
				
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return true;
			}
		}
		
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (System.IO.IOException e)
		{
			Debug.LogError("SOMETHING BROKE: "+e.Data);
			return false;
		}
	}

	private bool LoadMap(){
		string fileName = START_OF_TEXT + MAP_SAVE + END_OF_TEXT;
		
		// Handle any problems that might arise when reading the text
		try{
			string line;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);

			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader){
				// While there's lines left in the text file, do this:
				do{
					line = theReader.ReadLine();
					
					if (!string.IsNullOrEmpty(line)){// Do whatever you need to do with the text line, it's a string now

						string[] index = line.Split(new string[]{" "}, System.StringSplitOptions.RemoveEmptyEntries);

						if(index.Length>2){
							int x = int.Parse(index[0]);
							int y = int.Parse(index[1]);
							int z = int.Parse(index[2]);
							int block = int.Parse(index[3]);
						
							if(block!=-1){
								map.SetBlockNoSave(block,x,y,z);
							}
							else{
								//map.SetBlockNoSave(null,x,y,z);
							}

						}

					}
				}
				while (line != null);
				
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return true;
			}
		}
		
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (System.IO.IOException e)
		{
			Debug.LogError("SOMETHING BROKE IN MAPS: "+ e.Data);
			return false;
		}
	}

	//Public Methods//
	public int GetCurrentSnapshotNumber(){
		return PlayerPrefs.GetInt (SNAPSHOT_NUMBER);
	}

	public int GetCurrentMapNumber(){
		return PlayerPrefs.GetInt (MAP_UPDATE_SAVE_NUMBER);
	}



	public void IncrementSnapshotNumber(){
		PlayerPrefs.SetInt(SNAPSHOT_NUMBER, PlayerPrefs.GetInt (SNAPSHOT_NUMBER)+1);
	}

	public void IncrementMapNumber(){
		PlayerPrefs.SetInt(SNAPSHOT_NUMBER, PlayerPrefs.GetInt (MAP_UPDATE_SAVE_NUMBER);
	}

	public void AlterCurrentSnapshot(int alter){
		int currSnapshot = PlayerPrefs.GetInt (SNAPSHOT_NUMBER);
		PlayerPrefs.SetInt (SNAPSHOT_NUMBER, currSnapshot + alter);
	}

	public void SpecialSave(string specialSaveString){
		PlayerPrefs.SetString (SNAPSHOT_TEXT, specialSaveString);
		SaveSnapshot (specialSaveString);
	}

	public void SpecialSave(string specialSaveString, string text){
		SaveSnapshot (specialSaveString, text);
	}

	public string GetSavedSpecialString(){
		return PlayerPrefs.GetString(SNAPSHOT_TEXT);
	}
}
