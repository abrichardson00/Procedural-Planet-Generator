using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    //public static string name;
    //public static Transform planetTransform;// = new Transform(10000,0,0);
    public static int faceSize = 5000;


    Vector3 localUp;
    public static float radius = (2*faceSize) / Mathf.PI;
    public static Vector3[] localUps = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

    public static int[,] adjacentFaces     = {{4,3,5,2},{4,2,5,3},{0,5,1,4},{0,4,1,5},{3,0,2,1},{3,1,2,0}};
    public static int[,] adjacentRotations = {{1,-1,1,1},{-1,1,-1,-1},{-1,1,-1,-1},{1,-1,1,1},{1,-1,1,1},{-1,1,-1,-1}}; // -1 = clockwise, 1 = anticlockwise
    //public static int[,] adjacentRotations = {{1,-1,1,1},{-1,1,-1,-1},{-1,1,-1,-1},{1,-1,1,1},{1,-1,1,1},{-1,1,-1,-1}}


    // Start is called before the first frame update
    void Start()
    {
      TerrainGenerators TerrainGenerator = gameObject.AddComponent<TerrainGenerators>() as TerrainGenerators;
      TerrainGenerator.size = faceSize;
      GenerateMeshes GM = gameObject.AddComponent<GenerateMeshes>();
      GM.faceSize = faceSize;


    }

    // Update is called once per frame
    void Update()
    {

    }
}
