using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

  Mesh mesh;
  Vector3[] vertices;
  int[] triangles;
  Color[] colours;
  Color green = new Vector4(180f/255,240f/255,40f/255, 1);//new Vector4(200f/255,112f/25,34f/255,1);//
  Color gray = new Vector4(.4f,.4f,.4f,1);
  Color sand = new Vector4(255f/255,255f/255,250f/255, 1);
  //Color gray = new Vector4(150/255,150f/255,150f/255,1);

  public int offset_x = 0;
  public int offset_z = 0;

  public string planetName;
  public static TerrainGenerators TerrainGenerator;


  public static int xSize = GenerateMeshes.chunkSize;//250;
  public static int zSize = GenerateMeshes.chunkSize;//250;

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

//  public static float[] terrain;


  // Start is called before the first frame update
  public void Start()
  {
    // get Terrain Generator
    TerrainGenerator = GameObject.Find(planetName).GetComponent<TerrainGenerators>();

    mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = mesh;

    // add renderer and material
    gameObject.AddComponent<MeshRenderer>();
    MeshRenderer renderer = GetComponent<MeshRenderer>();
    renderer.material = Resources.Load("Ground1",typeof(Material)) as Material;

    localUp = Planet.localUps[face];//Vector3.down;//Vector3.up;
    axisB = new Vector3(localUp.y, localUp.z, localUp.x);
    axisA = Vector3.Cross(localUp, axisB);
    //axisA = new Vector3(localUp.y, localUp.z, localUp.x);
    //axisB = Vector3.Cross(localUp, axisA);
    planetRadius = Planet.radius;
    faceSize = TerrainGenerator.size;
    //Create Terrain --------------
    // LOD info
    step = LOD_step[LOD-1];
    xNum = xSize/step;
    zNum = zSize/step;

    CreateShape();
    UpdateMesh();

    // add collisions
    gameObject.AddComponent<MeshCollider>();

  }


  void CreateShape(){
    vertices = new Vector3[(xNum+1)*(zNum+1)];
    for (int i = 0, z = 0; z <= zNum; z++){
      for (int x = 0; x <= xNum; x++){
        //float y = Mathf.PerlinNoise(x * .07f, z * .07f)* 20 + Random.value;
        float y = TerrainGenerator.mainTerrain[face,x*step + offset_x,z*step + offset_z];

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

    colours = new Color[vertices.Length];

    for(int i = 0, z = 0; z <= zNum; z++)
    {
      for(int x = 0; x <= xNum; x++)
      {
        if (TerrainGenerator.isGrass[face,x*step+offset_x,z*step+offset_z]){
          colours[i] = green;
        }
        else if(TerrainGenerator.isBeach[face,x*step+offset_x,z*step+offset_z]) {
          colours[i] = sand;
        }
        else {
          colours[i] = gray;
        }



        i++;
      }
    }
  }



  void Update(){
    //UpdateMesh();
    //UpdateMesh();
  }


  void UpdateMesh(){
    mesh.Clear();
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.colors = colours;
    mesh.RecalculateNormals();
  }

  //void UpdateMeshes(){
  //    for


  //}


}
