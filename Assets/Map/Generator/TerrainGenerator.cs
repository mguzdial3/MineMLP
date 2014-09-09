using UnityEngine;
using System.Collections;

public class TerrainGenerator {
	
	private const int WATER_LEVEL = 1;
	private const int ICE_LEVEL = 22;
	
	private PerlinNoise2D noise1 = new PerlinNoise2D(1/150f).SetOctaves(5);
	private PerlinNoise2D noise2 = new PerlinNoise2D(1/150f).SetOctaves(2);
	private PerlinNoise3D noise3d = new PerlinNoise3D(1/30f);
	
	private Map map;
	
	private Block lava;
	
	private Block grass;
	private Block dirt;
	private Block sand;
	private Block stone;
	
	private Block snow;

	//Name
	public static readonly string[] blockNames = new string[]{"Lava", "Grass", "Dirt", "Sand", "Stone", "Snow"};



	public TerrainGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet();
		
		lava = blockSet.GetBlock(blockNames[0]);
		
		grass = blockSet.GetBlock(blockNames[1]);
		dirt = blockSet.GetBlock(blockNames[2]);
		sand = blockSet.GetBlock(blockNames[3]);
		stone = blockSet.GetBlock(blockNames[4]);
		
		snow = blockSet.GetBlock(blockNames[5]);
	}


	
	public void Generate(int cx, int cz) {
		/*float[,] map1 = new float[Chunk.SIZE_X+2, Chunk.SIZE_Z+2];
		noise1.Noise(map1, cx*Chunk.SIZE_X-1, cz*Chunk.SIZE_Z-1);
		
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				int worldX = cx*Chunk.SIZE_X+x;
				int worldZ = cz*Chunk.SIZE_Z+z;
				int worldY = (int) (map1[x+1, z+1]*30 + 50);
				
				
				for(int y=WATER_LEVEL; y>worldY; y--) {
					map.SetBlock(new BlockData(water), worldX, y, worldZ);
				}
				
				int deep = 0;
				for(; worldY>=0; worldY--) {
					GenerateBlock(worldX, worldY, worldZ, deep);
					deep++;
				}
				
			}
		}*/
		
		
		
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				int worldX = cx*Chunk.SIZE_X+x;
				int worldZ = cz*Chunk.SIZE_Z+z;
				
				int h1 = (int) (noise1.Noise(worldX, worldZ)*70);//Change this to constant to make a constant map
				h1 = Mathf.Clamp(Mathf.Abs(h1), 5, 200);
				int h2 = (int) (noise2.Noise(worldX, worldZ)*40);//Change this to constant to make a constant map
				h2 = Mathf.Clamp(h2, 0, 200);
				h2 += h1;
				
				int deep = 0;
				int worldY = h2;
				for(; worldY>h1; worldY--) {
					if(noise3d.Noise(worldX, worldY, worldZ) < 0) {
						GenerateBlock(worldX, worldY, worldZ, deep);
						deep++;
					} else {
						deep = 0;
					}
				}
				
				//int down = h1 - Chunk.SIZE_Y;
				for(; worldY>=0; worldY--) {
					GenerateBlock(worldX, worldY, worldZ, deep);
					deep++;
				}
				
			}
		}
	}
	
	private void GenerateBlock(int worldX, int worldY, int worldZ, int deep) {
		Block block = GetBlock(worldX, worldY, worldZ, deep);
		if(block != null) map.SetBlock(new BlockData(block), worldX, worldY, worldZ);
	}
	
	private Block GetBlock(int worldX, int worldY, int worldZ, int deep) {
		if(deep == 0) return grass;
		if(deep <= 5) return dirt;
		return stone;
	}

	private Block GetBlockFancy(int worldX, int worldY, int worldZ, int deep) {
		if(worldY == WATER_LEVEL+1) return sand;
		if (worldY <= WATER_LEVEL) return lava;
		if(worldY>ICE_LEVEL && deep==0) return snow;
		if(deep == 0) return grass;
		if(deep <= 5 && deep>0) return dirt;
		return stone;
	}
	
}

