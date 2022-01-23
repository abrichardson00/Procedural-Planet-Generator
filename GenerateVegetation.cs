using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateVegetation : MonoBehaviour
{

    public GameObject tree1;
    public GameObject grass1;
    public GameObject grass2;

    public int xSize;
    public int zSize;

    public string planetName;
    public static TerrainGenerators TerrainGenerator;







    public void generate(int offset_x, int offset_z){
      // get Terrain Generator
      TerrainGenerator = GameObject.Find(planetName).GetComponent<TerrainGenerators>();
      xSize = TerrainGenerator.size;
      zSize = TerrainGenerator.size;
      generateTrees(offset_x,offset_z);
      generateGrass(offset_x,offset_z);

    }

    public void generateTrees(int offset_x, int offset_z){
      for (int x = 0; x < xSize; x++){
        for (int z = 0; z < zSize; z++){

          float perlin = Mathf.PerlinNoise((x+offset_x)/20f, (z+offset_z)/20f);
          //Debug.Log(perlin);
          if (x%3==0 && z%3==0 && TerrainGenerator.isGrass[0,x+offset_x,z+offset_z] && perlin >.7f){
            //int seed = (int) Random.value*1000;
            //Random.InitState(1000);
            int rand_x = (int) Random.Range(0, 3);
            if (rand_x+offset_x > xSize){
              rand_x -= 3;
             }
            int rand_z = (int) Random.Range(0, 3);
            if (rand_z+offset_z > zSize){
              rand_z -= 3;
            }
            int tree_x = x+offset_x+rand_x;
            int tree_z = z+offset_z+rand_z;
            Vector3 treePos = new Vector3(tree_x,TerrainGenerator.mainTerrain[0,tree_x,tree_z]-1,tree_z);
            Instantiate(tree1,treePos,Quaternion.Euler(270, 0, 0));
          }

        }
      }
    }

    public void generateGrass(int offset_x, int offset_z){
      for (int x = 0; x < xSize; x++){
        for (int z = 0; z <zSize; z++){

          float perlin = Mathf.PerlinNoise((x+offset_x)/4f, (z+offset_z)/4f);
          //Debug.Log(perlin);
          if (x%2==0 && z%2==0 && TerrainGenerator.isGrass[0,x+offset_x,z+offset_z] && perlin >.7f){
            //int seed = (int) Random.value*1000;
            //Random.InitState(1000);
            float rand_x = Random.Range(0,2);
            if (rand_x+offset_x > xSize){
              rand_x -= 2;
             }
            float rand_z = Random.Range(0,2);
            if (rand_z+offset_z > zSize){
              rand_z -= 2;
            }
            float grass_x = x+offset_x+rand_x;
            float grass_z = z+offset_z+rand_z;
            Vector3 grassPos = new Vector3(grass_x,TerrainGenerator.mainTerrain[0,(int) grass_x,(int) grass_z]-.1f,grass_z);
            if (Random.Range(0,2)>1){
              Instantiate(grass1,grassPos,Quaternion.Euler(270, 0, 0));
            } else {
              Instantiate(grass2,grassPos,Quaternion.Euler(270, 0, 0));
            }

          }

        }
      }

    }

    // make a destroy instances function for infinite terrain ------



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
