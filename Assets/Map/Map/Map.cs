using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[AddComponentMenu("Map/Map")]
public class Map : MonoBehaviour {
	
	public const int maxChunkY = 16;
	public const int maxBlockY = maxChunkY * Chunk.SIZE_Y;
	public static string SAVE_STRING = "Map";
	public static string CHUNK_SAVE_STRING = "Chunk";
	
	[SerializeField] private BlockSet blockSet;
	private Grid<ChunkData> chunks = new Grid<ChunkData>();
	private Lightmap lightmap = new Lightmap();

	private List<string> changeList;
	public static Map Instance;
	

	void Awake() {
		ChunkBuilder.Init( blockSet.GetMaterials().Length );

		if (Instance == null) {
			Instance = this;		
		}

		changeList = new List<string> ();
	}

	void Update(){
		if (Input.GetKey (KeyCode.P)) {
			Debug.Log ("ChunkData: " + chunks.GetStringBounds () + "\n" + lightmap.GetStringBounds ());
		}
	}

	public bool CanSave(){
		return chunks != null;
	}

	public bool HasReachedBounds(){
		return chunks.HasReachedEdge ();
	}

	public bool HasChangeString(){
		return changeList.Count != 0;
	}

	public string GetChangeString(){
		string changeString = "";

		foreach (string change in changeList) {
			changeString+= change+"\n";
		}

		changeList.Clear ();

		return changeString;
	}



	public void ClearList(){
		changeList.Clear ();
	}

	public int GetMinX(){
		return chunks.GetMinX ();
	}

	public int GetMinZ(){
		return chunks.GetMinZ ();
	}

	public int GetMaxX(){
		return chunks.GetMaxX ();
	}

	public int GetMaxZ(){
		return chunks.GetMaxZ ();
	}

	public string GetSaveString(){
		string saveString = SAVE_STRING+"\n";//Let's reader know we started the map section
		saveString += chunks.GetMinX () + " " + chunks.GetMinY () + " " + chunks.GetMinZ ()+" ";
		saveString += chunks.GetMaxX () + " " + chunks.GetMaxY () + " " + chunks.GetMaxZ (); //Get size of chunk grid

		saveString += "\n";
		for(int x = chunks.GetMinX(); x<=chunks.GetMaxX(); x++){
			for(int y = chunks.GetMinY(); y<=chunks.GetMaxY(); y++){
				for(int z = chunks.GetMinZ(); z<=chunks.GetMaxZ(); z++){

					ChunkData chunkData = GetChunkData(new Vector3i(x,y,z));
					saveString+=CHUNK_SAVE_STRING+" "+x+" "+y+" "+z+"\n"; //We are on a new chunk at this location

					if(chunkData!=default(ChunkData)){
						saveString+=chunkData.ToString();
					}
				}
			}
		}
	
		return saveString;
	}
	
	public void SetBlockAndRecompute(BlockData block, Vector3i pos) {
		SetBlock( block, pos );
		
		Build( Chunk.ToChunkPosition(pos) );
		foreach( Vector3i dir in Vector3i.directions ) {
			Build( Chunk.ToChunkPosition(pos + dir) );
		}
		LightComputer.RecomputeLightAtPosition(this, pos);
	}
	
	public void BuildColumn(int cx, int cz) {
		for(int cy=chunks.GetMinY(); cy<chunks.GetMaxY(); cy++) {
			Build( new Vector3i(cx, cy, cz) );
		}
	}

	private void Build(Vector3i pos) {
		ChunkData chunk = GetChunkData( pos );
		if(chunk != null) chunk.GetChunkInstance().SetDirty();
	}
	
	private ChunkData GetChunkDataInstance(Vector3i pos) {
		if(pos.y < 0) return null;
		ChunkData chunk = GetChunkData(pos);
		if(chunk == null) {
			chunk = new ChunkData(pos);
			chunks.AddOrReplace(chunk, pos);
		}
		return chunk;
	}

	public ChunkData GetChunkData(Vector3i pos) {
		return chunks.SafeGet(pos);
	}

	public void SetBlock(BlockData block, Vector3i pos) {
		SetBlock(block, pos.x, pos.y, pos.z);
	}

	public void SetBlock(BlockData block, int x, int y, int z) {
		ChunkData chunk = GetChunkDataInstance( Chunk.ToChunkPosition(x, y, z) );
		if (chunk != null) {
			chunk.SetBlock (block, Chunk.ToLocalPosition (x, y, z));
			if(block.block!=null){
				changeList.Add(""+x+" "+y+" "+z+" "+Array.IndexOf(TerrainGenerator.blockNames,block.block.GetName()));
			}
			else{
				changeList.Add(""+x+" "+y+" "+z+" "+"-1");
			}
		}
	}

	public void SetBlockNoSave(Block block, int x, int y, int z) {
		ChunkData chunk = GetChunkDataInstance( Chunk.ToChunkPosition(x, y, z) );
		if (chunk != null) {
			chunk.SetBlock (new BlockData(block), Chunk.ToLocalPosition (x, y, z));

			Chunk chunkObj =chunk.GetChunkInstance();
			chunkObj.SetDirty();
			//chunkObj.SetLightDirty();

		}
	}

	public void SetBlockAndRecomputeNoSave(int block, int x, int y, int z) {
		SetBlockNoSave( block, x,y,z);

		Vector3i pos = new Vector3i (x, y, z);

		Build( Chunk.ToChunkPosition(pos) );
		foreach( Vector3i dir in Vector3i.directions ) {
			Build( Chunk.ToChunkPosition(pos + dir) );
		}
		LightComputer.RecomputeLightAtPosition(this, pos);
	}

	public void SetBlockNoSave(int block, int x, int y, int z) {
		SetBlockNoSave (GetBlockByIndex (block), x, y, z);
	}




	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}

	public BlockData GetBlock(Vector3 pos) {
		return GetBlock((int)pos.x, (int)pos.y, (int)pos.z);
	} 

	public Block GetBlockByIndex(int index){
		return blockSet.GetBlock(TerrainGenerator.blockNames[index]);
	}

	public BlockData GetBlock(int x, int y, int z) {
		ChunkData chunk = GetChunkData( Chunk.ToChunkPosition(x, y, z) );
		if(chunk == null) return default(BlockData);
		return chunk.GetBlock( Chunk.ToLocalPosition(x, y, z) );
	}

	public bool IsPositionOpen(Vector3 pos){
		return (GetBlock (pos)).IsEmpty ();
	}

	public bool IsPositionOpen(Vector3i pos){
		return (GetBlock (pos)).IsEmpty ();
	}

	public Vector3 getEmptyLOC(Vector3 location){
		while (!IsPositionOpen(location) || ! IsPositionOpen(location-Vector3.up)) {
			location+=Vector3.up;
		}

		return location;
	}
	
	public Lightmap GetLightmap() {
		return lightmap;
	}
	
	public BlockSet GetBlockSet() {
		return blockSet;
	}
	
}

