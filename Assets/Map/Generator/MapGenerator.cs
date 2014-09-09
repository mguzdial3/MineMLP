using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


[AddComponentMenu("Map/MapGenerator")]
public class MapGenerator : MonoBehaviour {
	private Map map;
	private Grid<Chunk2D> map2D = new Grid<Chunk2D>();
	private TerrainGenerator terrainGenerator;
	
	void Awake() {
		map = GetComponent<Map>();
		terrainGenerator = new TerrainGenerator(map);
	}

	public void SpawnMap(){
		Vector3 pos = Camera.main.transform.position;
		Vector3i current = Chunk.ToChunkPosition( (int)pos.x, (int)pos.y, (int)pos.z );
		Vector3i? nearEmpty = FindNearestEmptyColumn(current.x, current.z, 7);
		
		if(nearEmpty.HasValue) {
			int cx = nearEmpty.Value.x;
			int cz = nearEmpty.Value.z;
			GenerateColumn(cx, cz);
			LightComputer.Smooth(map, cx, cz);
			BuildColumn(cx, cz);
		}


	}
	
	private Vector3i? FindNearestEmptyColumn(int cx, int cz, int rad) {
		Vector3i center = new Vector3i(cx, 0, cz);
		Vector3i? near = null;
		for(int z=cz-rad; z<=cz+rad; z++) {
			for(int x=cx-rad; x<=cx+rad; x++) {
				Vector3i current = new Vector3i(x, 0, z);
				int dis = center.DistanceSquared( current );
				if(dis > rad*rad) continue;
				if( GetChunk2D(x, z).built ) continue;
				if(!near.HasValue) {
					near = current;
				} else {
					int oldDis = center.DistanceSquared(near.Value);
					if(dis < oldDis) near = current;
				}
			}
		}
		return near;
	}
	
	
	private void GenerateColumn(int cx, int cz) {
		if( GetChunk2D(cx, cz).genereted ) return;
		terrainGenerator.Generate(cx, cz);
		LightComputer.ComputeSolarLighting(map, cx, cz);
		GetChunk2D(cx, cz).genereted = true;
	}

	public void CheckGeneration(int cx, int cz){
		if( GetChunk2D(cx, cz).genereted ) return;
		LightComputer.ComputeSolarLighting(map, cx, cz);
		GetChunk2D(cx, cz).genereted = true;

	}
	
	public void BuildColumn(int cx, int cz) {
		map.BuildColumn(cx, cz);
		GetChunk2D(cx, cz).built = true;
	}
	
	private Chunk2D GetChunk2D(int x, int z) {
		Chunk2D chunk = map2D.SafeGet(x, 0, z);
		if(chunk == null) {
			chunk = new Chunk2D();
			map2D.AddOrReplace(chunk, x, 0, z);
		}
		return chunk;
	}
	
}

public class Chunk2D {
	public bool genereted = false;
	public bool built = false;
}