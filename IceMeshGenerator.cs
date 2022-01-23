using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class IceMeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;


    public int xSize = GenerateMeshes.chunkSize;//250;
    public int zSize = GenerateMeshes.chunkSize;//250;
    //public float[] terrain;

    public string planetName;
    public static TerrainGenerators TerrainGenerator;

    public int offset_x = 0;
    public int offset_z = 0;

    public static int[] LOD_step = new int[] {1,5,20};

    public int LOD = 3;
    int step;
    int xNum;
    int zNum;

    Vector3 localUp;
    public int face;
    Vector3 axisA;
    Vector3 axisB;
    float planetRadius;
    int faceSize;

    //TerrainGenerator mainTerrain;

    // Start is called before the first frame update
    void Start()
    {

      TerrainGenerator = GameObject.Find(planetName).GetComponent<TerrainGenerators>();

      mesh = new Mesh();
      GetComponent<MeshFilter>().mesh = mesh;

      gameObject.AddComponent<MeshRenderer>();
      MeshRenderer renderer = GetComponent<MeshRenderer>();
      renderer.material = Resources.Load("Ice",typeof(Material)) as Material;

      localUp = Planet.localUps[face];//Vector3.down;//Vector3.up;
      axisB = new Vector3(localUp.y, localUp.z, localUp.x);
      axisA = Vector3.Cross(localUp, axisB);
      //axisA = new Vector3(localUp.y, localUp.z, localUp.x);
      //axisB = Vector3.Cross(localUp, axisA);
      planetRadius = Planet.radius;
      faceSize = TerrainGenerator.size;

      //CreateTerrain();

      step = LOD_step[LOD-1];
      xNum = xSize/step;
      zNum = zSize/step;

      CreateShape();
      //UpdateMesh();

      // add collisions
      gameObject.AddComponent<MeshCollider>();
    }

    //is createTerrain needed now?
    /*
    void CreateTerrain(){
      terrain = new float[(xSize+1)*(zSize+1)];
      for (int z = 0; z <= zSize; z++){
        for (int x = 0; x <= xSize; x++){
          terrain[z*xSize + x] = TerrainGenerators.grass[z*xSize + x];
        }
      }

    }
    */

    void CreateShape(){
      vertices = new Vector3[(xNum+1)*(zNum+1)];
      for (int i = 0, z = 0; z <= zNum; z++){
        for (int x = 0; x <= xNum; x++){
          float y = TerrainGenerator.ice[face,x*step + offset_x,z*step + offset_z];

          Vector2 percent = new Vector2(x*step + offset_x, z*step + offset_z) / faceSize;
          Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
          Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

          vertices[i] = pointOnUnitSphere * (planetRadius + y);
          //vertices[i] = new Vector3(x*step + offset_x,y,z*step + offset_z);
          i++;
        }
      }
      triangles = new int[xNum*zNum*6];
      int v = 0;
      int t = 0;
      for (int z = 0; z < zNum; z++){
        for (int x = 0; x < xNum; x++){
          triangles[t] = v;
          triangles[t + 1] = v + xNum + 1;
          triangles[t + 2] = v + 1;
          triangles[t + 3] = v + 1;
          triangles[t + 4] = v + xNum + 1;
          triangles[t + 5] = v + xNum + 2;
          v++;
          t += 6;
        }
        v++;
      }
    }

    void UpdateMesh(){
      mesh.Clear();
      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();
    }



}
