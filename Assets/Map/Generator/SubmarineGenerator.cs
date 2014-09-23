using UnityEngine;
using System.Collections;

public class SubmarineGenerator {
	private Map map;

	//Name
	public static string[] blockNames;
	public static Block[] blocks;
	
	private const int floorBlock = 0;
	private const int wallBlock1 = 2;
	private const int wallBlock2 = 2;
	private const int radioactive = 4;
	private const int radioactivity = 5;


	private int radioactiveRadius = 3;
	public SubmarineGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet();
		blockNames = blockSet.GetStringArray ();
		blocks = new Block[blockNames.Length];
		
		for (int i = 0; i<blockNames.Length; i++) {
			blocks[i] = blockSet.GetBlock(i);		
		}
	}
	
	public void GenerateFloor(int cx, int cz) {
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				int worldX = cx*Chunk.SIZE_X+x;
				int worldZ = cz*Chunk.SIZE_Z+z;
				map.SetBlock (new BlockData (blocks [floorBlock]), new Vector3i (worldX, 1, worldZ));
		
			}		
		}
	}

	public void GenerateWall(int cx, int cz) {
		map.SetBlock (new BlockData (blocks [wallBlock1]), new Vector3i (cx, 2, cz));
		map.SetBlock (new BlockData (blocks [wallBlock2]), new Vector3i (cx, 3, cz));
		map.SetBlock (new BlockData (blocks [wallBlock2]), new Vector3i (cx, 4, cz));
	}




	public void GenerateRadioactive(int x, int y, int z, int deep=0) {
		if(deep > 3) return;
		deep++;
		if(map.GetBlock(x, y, z).IsEmpty()) {
			if(deep==1){
				map.SetBlock(new BlockData(blocks[radioactive]), x, y, z);
			}
			else{
				if(map.IsPositionOpen(new Vector3i(x,y,z)) && y<4){
					map.SetBlock(new BlockData(blocks[radioactivity]), x, y, z);
				}
				else{
					return;
				}
			}
			GenerateRadioactive(x-1, y, z, deep);
			GenerateRadioactive(x+1, y, z, deep);
			

			GenerateRadioactive(x, y-1, z, deep);
			GenerateRadioactive(x, y+1, z, deep);
			GenerateRadioactive(x, y+2, z, deep);
			
			GenerateRadioactive(x, y, z-1, deep);
			GenerateRadioactive(x, y, z+1, deep);
		}
	}

	
}

