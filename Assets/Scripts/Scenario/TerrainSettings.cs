using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSettings : MonoBehaviour
{
	//referência do terreno
	private Terrain terrain;
	
	//qualidade gráfica do terreno
	[SerializeField]
	private int terrain_quality;
	
	//o que a qualidade gráfica do terreno muda
	[SerializeField]
	private float[] pixel_error, baseMap_dist, detail_dist, detail_dens,
					tree_dist, billb_start, fade_length, max_mesh_tree;
	[SerializeField]
	private bool[] bakeLP_tree;
	
    void Start()
    {
		//config qualidade gráfica
		if(PlayerPrefs.HasKey("GameQuality"))
		{
			int qual = PlayerPrefs.GetInt("GameQuality");
			
			QualitySettings.SetQualityLevel(qual);
			
			//config qualidade terreno
			terrain_quality = qual;
		}
		
		//pega o componente de terreno
        terrain = GetComponent<Terrain>();
		
		//muda as configurações
		terrain.heightmapPixelError = pixel_error[terrain_quality];
		terrain.basemapDistance = baseMap_dist[terrain_quality];
		terrain.bakeLightProbesForTrees = bakeLP_tree[terrain_quality];
		terrain.detailObjectDistance = detail_dist[terrain_quality];
		terrain.detailObjectDensity = detail_dens[terrain_quality];
		terrain.treeDistance = tree_dist[terrain_quality];
		terrain.treeBillboardDistance = billb_start[terrain_quality];
		terrain.treeCrossFadeLength = fade_length[terrain_quality];
		terrain.treeMaximumFullLODCount = (int)max_mesh_tree[terrain_quality];
	}
}
