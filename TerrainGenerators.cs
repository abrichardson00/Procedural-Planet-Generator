using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerators : MonoBehaviour
{
    //make size variables controlled only here, to change the other scripts
    public int size;// = Planet.faceSize;
    public static int xSize;// = Planet.faceSize;
    public static int zSize;// = Planet.faceSize;
    //public static int chunkSize = 100;
    public static int waterLevel = -20;
    public static int height =  80;//(int) (Planet.radius * (3f/20f));
    public float[,,] mainTerrain;
    public float[,,] water;

    // ground types -----------------
    public bool[,,] isGrass;
    public float[,,] grass;
    public float[,,] ice;
    public bool[,,] isIce;
    public bool[,,] isBeach;


    float grassOffset = 0;
    int seed;

    Noise noise;
    float noise_scale;

    /*
    public TerrainGenerators(int faceSize){
      size = faceSize;
      xSize = faceSize;
      zSize = faceSize;
    }
    */

    // Start is called before the first frame update

    void Start()
    {
      //size = gameObject.Planet.faceSize;
      xSize = size;
      zSize = size;


      int seed = (int) System.DateTime.Now.Ticks;
      noise = new Noise(seed);
      noise_scale = Planet.radius/750f;//((float) 1.5f*size)/(1000);
      mainTerrain = new float[6,xSize+1,zSize+1];
      water = new float[6,xSize+1,zSize+1];
      ice = new float[6,xSize+1,zSize+1];
      isIce = new bool[6,xSize+1,zSize+1];
      isGrass = new bool[6,xSize+1,zSize+1];
      isBeach = new bool[6,xSize+1,zSize+1];
      //seed = (int) Time.time*1000;
      for (int i = 0; i < 6; i++){

        CreateMainTerrain(i);
        CreateWater(i);
        erosion(i);
        CreateIce(i);
        //erosion(i);
        //erosion(i);
        CreateGrass(i);
        //CreateBeaches(i);

      }



    }

    void CreateMainTerrain(int face){
      //mainTerrain = new float[(xSize+1)*(zSize+1)];
      //mainTerrain = new float[xSize+1,zSize+1];
      //int seed = (int) Random.value*1000;
      //Random.InitState(1000);
      /*
      int offset = (int) Random.Range(0, 100000);
      for (int z = 0; z <= zSize; z++){

        for (int x = 0; x <= xSize; x++){
          mainTerrain[x,z] = ((Mathf.PerlinNoise(((x + offset) * .2f * noise_scale), ((z + offset) * .2f * noise_scale))-.5f) * 2);
          mainTerrain[x,z] +=((Mathf.PerlinNoise(((x + offset) * noise_scale), ((z + offset) * noise_scale))-.5f));
          mainTerrain[x,z] +=((Mathf.PerlinNoise(((x + offset) * 2 * noise_scale), ((z + offset) * 2 * noise_scale))-.5f)/2f);
          mainTerrain[x,z] +=((Mathf.PerlinNoise(((x + offset) * 4 * noise_scale), ((z + offset) * 4 * noise_scale))-.5f)/4f);
          mainTerrain[x,z] *= height;

        }

      }// + Random.value*.02f - .01f
      */

      Vector3 localUp = Planet.localUps[face];
      Vector3 axisB = new Vector3(localUp.y, localUp.z, localUp.x);
      Vector3 axisA = Vector3.Cross(localUp, axisB);

      for (int z = 0; z <= zSize; z++){
        for (int x = 0; x <= xSize; x++){
          //float y = Mathf.PerlinNoise(x * .07f, z * .07f)* 20 + Random.value;
          //float y = TerrainGenerators.mainTerrain[x*step + offset_x,z*step + offset_z];

          Vector2 percent = new Vector2(x, z) / xSize;
          Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
          Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

          //vertices[i] = pointOnUnitSphere * (planetRadius + y);
          mainTerrain[face,x,z] = noise.Evaluate(pointOnUnitSphere*noise_scale)/1f;
          mainTerrain[face,x,z] += noise.Evaluate(pointOnUnitSphere*8*noise_scale)/4f;
          mainTerrain[face,x,z] += noise.Evaluate(pointOnUnitSphere*16*noise_scale)/16f;
          mainTerrain[face,x,z] += noise.Evaluate(pointOnUnitSphere*24*noise_scale)/20f;


          mainTerrain[face,x,z] *= height;
          //vertices[i] = new Vector3(x*step + offset_x,y,z*step + offset_z);

        }
      }



    }
    void CreateWater(int face){
      //water = new float[xSize+1,zSize+1];
      for (int z = 0; z <= zSize; z++){

        for (int x = 0; x <= xSize; x++){
          water[face,x,z] = waterLevel;
        }
      }
    }
    void erosion(int face){
      int n = 20*size;
      for (int i = 0; i < n; i++){
        waterDrop(face);
      }


    }


    //glacier erosion - use original terrain before erosion??


    void waterDrop(int face){
      int x = (int) Random.Range(2,xSize-1);
      int z = (int) Random.Range(2,zSize-1);
      bool stuck = false;
      int waterAccuracy = 3;

      //if rain lands on sea
      if (mainTerrain[face,x,z] <= waterLevel) stuck = true;
      //
      //loop many times until water gets stuck
      while (!stuck) {
        float[,] points = getSurroundingPoints(face,x,z);
        float min = mainTerrain[face,x,z];
        int minIndex = -1;

        //need to add randomness to direction choosing

        for (int i = 0; i < points.GetLength(0); i++){
          if (points[i,1] < min) {
            min = points[i,1];
            minIndex = i;
          }
        }

        //if water is stuck or reaching a border
        if (minIndex == -1 || (points[minIndex,0] <= 2 || points[minIndex,0] >= xSize-2 || points[minIndex,2] <= 2 || points[minIndex,2] >= zSize-2)) {
          if (minIndex == -1) {
            mainTerrain[face,x,z] += 1f; // raindrop deposits material
            /*
            water[x,z] = mainTerrain[x,z] + 1;
            water[x+1,z] = mainTerrain[x,z] + 1;
            water[x-1,z] = mainTerrain[x,z] + 1;
            water[x,z+1] = mainTerrain[x,z] + 1;
            water[x,z-1] = mainTerrain[x,z] + 1;
            */
          }
          stuck = true;

          break;
        } else {

          //add randomness to water path
          if (Random.Range(0, waterAccuracy) < 1) {
            minIndex = (minIndex-1 + points.GetLength(0))%(points.GetLength(0));
            min = points[minIndex,1];
          } else if (Random.Range(0, waterAccuracy) > waterAccuracy-1) {
            minIndex = (minIndex+1 + points.GetLength(0))%(points.GetLength(0));
            min = points[minIndex,1];
          }

          //we now have min point and index, erode and then move raindrop
          float erosionAmount = (mainTerrain[face,x,z]-min)/5f;
          // erode
          mainTerrain[face,x,z] -= erosionAmount;
          mainTerrain[face,x+1,z] -= erosionAmount/2;
          mainTerrain[face,x-1,z] -= erosionAmount/2;
          mainTerrain[face,x,z+1] -= erosionAmount/2;
          mainTerrain[face,x,z-1] -= erosionAmount/2;
          mainTerrain[face,x+1,z+1] -= erosionAmount/2;
          mainTerrain[face,x-1,z+1] -= erosionAmount/2;
          mainTerrain[face,x+1,z-1] -= erosionAmount/2;
          mainTerrain[face,x-1,z-1] -= erosionAmount/2;

          mainTerrain[face,x+2,z] -= erosionAmount/2;
          mainTerrain[face,x-2,z] -= erosionAmount/2;
          mainTerrain[face,x,z+2] -= erosionAmount/2;
          mainTerrain[face,x,z-2] -= erosionAmount/2;



          x = (int) points[minIndex,0];
          z = (int) points[minIndex,2];
        }
      }
    }

    //must check coordinates are not borders first
    float[,] getSurroundingPoints(int face,int x,int z){
      float[,] points = new float[8,3];
      int[] xs = new int[8] {x-1,x-1,x-1,x,x+1,x+1,x+1,x};//{x-1, x-1, x-1, x, x, x+1, x+1, x+1};
      int[] zs = new int[8] {z-1,z,z+1,z+1,z+1,z,z-1,z-1};//{z-1, z, z+1, z-1, z+1, z-1, z, z+1};
      for (int i = 0; i < 8; i++){
        points[i,0] = xs[i];
        points[i,1] = mainTerrain[face,xs[i],zs[i]];
        points[i,2] = zs[i];
      }
      return points;
    }





    void CreateGrass(int face){
      //isGrass = new bool[(xSize+1),(zSize+1)];
      //grass = new float[(xSize+1),(zSize+1)];
      for (int z = 0; z <= zSize; z++){
        for (int x = 0; x <= xSize; x++){
          //grass[x,z] = mainTerrain[x,z] + grassOffset - 1;
          if (x > 0 && x < xSize && z > 0 && z < zSize) {
            float mid = mainTerrain[face,x,z];
            float average = (Mathf.Abs(mainTerrain[face,x - 1,z]-mid) + Mathf.Abs(mainTerrain[face,x + 1,z]-mid) + Mathf.Abs(mainTerrain[face,x,z - 1]-mid) + Mathf.Abs(mainTerrain[face,x,z + 1]-mid))/4f;
            bool isBordering = isGrass[face,x - 1,z] || isGrass[face,x + 1,z] || isGrass[face,x,z + 1] || isGrass[face,x,z + 1];
            bool correctHeight = (mainTerrain[face,x,z] < (waterLevel+height*.25f)) && (mainTerrain[face,x,z] > (waterLevel)); // < (height/2f - height/2f));
            //grass[x,z] = mainTerrain[x,z] - 1;
            //isGrass[x,z] = true;
            /*
            if (isBordering) {
              if (Mathf.Abs(mainTerrain[x,z] - average) < 50f){
                 //grass[x,z] = mainTerrain[x,z] + grassOffset + .2f;
                 if (Random.Range(0,2)<1){
                   isGrass[x,z] = true;
                 }

              }
            }
            */
            if (correctHeight){
              if (average < .4f) {
                //grass[x,z] = mainTerrain[x,z] + grassOffset + .2f;
                isGrass[face,x,z] = true;
              }
            }
            }


        }
      }


    }

    void CreateBeaches(int face){
      // beaches where grass can meet water
      //isBeach = new bool[(xSize+1),(zSize+1)];
      for (int z = 0; z <= zSize; z++){
        for (int x = 0; x <= xSize; x++){

          if ((isGrass[face,x,z]) && (mainTerrain[face,x,z] - waterLevel)<.4f){
            // (x,z) should be a beach
            isGrass[face,x,z] = false;
            isBeach[face,x,z] = true;

            int off_i = 0;
            int off_j = 0;
            for (int n = 0; n < 20; n++){
              int min_i = 0;
              int min_j = 0;
              Flatten(face,x+off_i,z+off_j);
              for (int i = -1; i <= 1; i++){
                for (int j = -1; j <= 1; j++){
                  int cx = x + i + off_i;
                  int cz = z + j + off_j;
                  if (i != 0 && j != 0 && (cx) <= xSize && (cz) <= zSize && (cx)>=0 && (cz)>=0){
                    // all surrounding points in our terrain
                    if (mainTerrain[face,(cx),(cz)] < mainTerrain[face,x+off_i,z+off_j]){
                      // get min i,j vals
                      if (mainTerrain[face,cx,cz] < mainTerrain[face,x+off_i+min_i,z+off_j+min_j]){
                        min_i = i;
                        min_j = j;
                      }

                      //mainTerrain[(cx),(cz)] = mainTerrain[x+off_i,z+off_j];//+= (mainTerrain[x+off_i,z+off_j] - mainTerrain[(cx),(cz)])/1.2f;
                      isBeach[face,(cx),(cz)] = true;
                    }
                  }

                }
              }
            off_i = min_i;
            off_j = min_j;
            }

          }

        }
      }

    }


    void Flatten(int face,int x,int z){
      for (int i = -1; i <= 1; i++){
        for (int j = -1; j <= 1; j++){
          if (i != 0 && j != 0 && (x+i) <= xSize && (z+j) <= zSize && (x+i)>=0 && (z+j)>=0){
            mainTerrain[face,x+i,z+j] += (mainTerrain[face,x,z] - mainTerrain[face,(x+i),(z+j)])/2f;// = mainTerrain[x,z]-.1f;
            mainTerrain[face,x+i,z+j] += .1f;
            isBeach[face,x+i,z+j] = true;
          }


        }
      }
    }

    void CreateIce(int face){

      //isIce = new bool[(xSize+1),(zSize+1)];
      //ice = new float[(xSize+1),(zSize+1)];
      int h = (int) (height*.75f);

      //initial snow above height 'h'
      for (int z = 0; z <= zSize; z++){
        for (int x = 0; x <= xSize; x++){
          if (mainTerrain[face,x,z] > h) {
            ice[face,x,z] = mainTerrain[face,x,z] + .2f;
            isIce[face,x,z] = true;
          } else {
            ice[face,x,z] = mainTerrain[face,x,z] - (h - mainTerrain[face,x,z]);
            isIce[face,x,z] = false;
          }
        }
      }

      //snow spread
      for (int i = 0; i < 10; i++){

      //////do 'i' times
        for (int z = 2; z <= (zSize - 2); z+=4){
          for (int x = 2; x <= (xSize - 2); x+=4){
            bool isBorderingIce = isIce[face,x - 1,z] || isIce[face,x + 1,z] || isIce[face,x,z + 1] || isIce[face,x,z + 1];
            if (isBorderingIce) {
              float gradient = ((Mathf.Abs(mainTerrain[face,x - 1,z]) + Mathf.Abs(mainTerrain[face,x + 1,z]) + Mathf.Abs(mainTerrain[face,x,z - 1])
                              + Mathf.Abs(mainTerrain[face,x,z + 1]))/4f) - Mathf.Abs(mainTerrain[face,x,z]);
              if (gradient > 0) {
                ice[face,x,z] = mainTerrain[face,x,z] + .2f;
                isIce[face,x,z] = true;
                ice[face,x+1,z] = mainTerrain[face,x+1,z] + .2f;
                isIce[face,x+1,z] = true;
                ice[face,x-1,z] = mainTerrain[face,x-1,z] + .2f;
                isIce[face,x-1,z] = true;
                ice[face,x,z+1] = mainTerrain[face,x,z+1] + .2f;
                isIce[face,x,z+1] = true;
                ice[face,x,z-1] = mainTerrain[face,x,z-1] + .2f;
                isIce[face,x,z-1] = true;

                ice[face,x+1,z+1] = mainTerrain[face,x+1,z+1] + .2f;
                isIce[face,x+1,z+1] = true;
                ice[face,x-1,z+1] = mainTerrain[face,x-1,z+1] + .2f;
                isIce[face,x-1,z+1] = true;
                ice[face,x+1,z-1] = mainTerrain[face,x+1,z-1] + .2f;
                isIce[face,x+1,z-1] = true;
                ice[face,x-1,z-1] = mainTerrain[face,x-1,z-1] + .2f;
                isIce[face,x-1,z-1] = true;

                ice[face,x+2,z] = mainTerrain[face,x+2,z] + .2f;
                isIce[face,x+2,z] = true;
                ice[face,x-2,z] = mainTerrain[face,x-2,z] + .2f;
                isIce[face,x-2,z] = true;
                ice[face,x,z+2] = mainTerrain[face,x,z+2] + .2f;
                isIce[face,x,z+2] = true;
                ice[face,x,z-2] = mainTerrain[face,x,z-2] + .2f;
                isIce[face,x,z-2] = true;

              }

            }
          }
        }
      //////
      }
    }


}
