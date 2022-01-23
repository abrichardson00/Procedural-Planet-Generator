using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshes : MonoBehaviour
{
    /*
    GameObject[,] meshes;
    GameObject[,] water_meshes;
    GameObject[,] ice_meshes;
    GenerateVegetation Vegetation_Generator;
    */
    public static int chunkSize;//250;
    public int faceSize;

    //public Transform planetTransform;
    //public TerrainGenerators TerrainGenerator = gameObject.TerrainGenerator;//gameObject.GetComponent<TerrainGenerators>();

    public Vector2 midPoint;
    Vector3 localUp;
    Vector3[] localUps = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
    Vector3 axisA;
    Vector3 axisB;
    float planetRadius;

    Planet planet;
    string planetName;
    //public int zSize = TerrainGenerators.chunkSize;

    int num_x;
    int num_z;

    //////////

    public const float maxViewDst = 1600;
    public Transform planetTransform;
	  public Transform playerTransform;
	  public Material mapMaterial;

	  public static Vector2 viewerPosition;
	  //static MapGenerator mapGenerator;
	//int chunkSize;
	  int chunksVisibleInViewDst;

	  //Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	  List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    //List<Vector2> terrainCoordsVisibleLastUpdate = new List<Vector2>();
    //List<Vector2> terrainCoordsVisible = new List<Vector2>();

    Dictionary<Vector3, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector3, TerrainChunk>();
    List<Vector3> terrainKeysVisibleLastUpdate = new List<Vector3>();
    List<Vector3> terrainKeysVisible = new List<Vector3>();

    /*
    public GenerateMeshes(int size,float radius){
      faceSize = size;
      planetRadius = radius;
    }
    */

	  void Start() {
		    //mapGenerator = FindObjectOfType<MapGenerator> ();
        planet = this.gameObject.GetComponent<Planet>();
        planetName = this.gameObject.name;
        planetTransform = this.gameObject.transform;
        playerTransform = GameObject.Find("Camera").transform;
		    chunkSize = 200;
        //faceSize = planet.faceSize;
        //faceSize = TerrainGenerators.size;
        midPoint = new Vector2( (((faceSize/chunkSize) -1)*.5f),(((faceSize/chunkSize) -1)*.5f));
        //localUp = Vector3.down;//Vector3.up;
        //axisB = new Vector3(localUp.y, localUp.z, localUp.x);
        //axisA = Vector3.Cross(localUp, axisB);
        //axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        //axisB = Vector3.Cross(localUp, axisA);
        planetRadius = Planet.radius;
        Debug.Log(planetRadius);


		    chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        StartCoroutine("UpdateVisibleChunks");
	  }

    int interval = 1;
    float nextTime = 0;
	  void Update() {

	 }


   public bool checkChunkFace(Vector2 viewedChunkCoord){
     // if chunkcoordx and chunkcoordy are both outwith face, we can disregard it.
     // if chunkcoordx > 2 * facewidth or chunkcoordy > 2 * faceheight, we can disregard it
     int numChunks = faceSize/chunkSize;
     /*
     if ((viewedChunkCoord.x >= numChunks && viewedChunkCoord.y >= numChunks) || (viewedChunkCoord.x < 0 && viewedChunkCoord.y < 0)){
       return true;
     } else if (viewedChunkCoord.x >= 2*numChunks || viewedChunkCoord.y >= 2*numChunks || viewedChunkCoord.x <= -1*numChunks || viewedChunkCoord.y <= -1*numChunks){
       return false;
     } else {
       return true;
     }
     */
     if ((viewedChunkCoord.x >= 0 && viewedChunkCoord.x < numChunks) || (viewedChunkCoord.y >= 0 && viewedChunkCoord.y < numChunks)){
       if (viewedChunkCoord.x < 2*numChunks && viewedChunkCoord.y < 2*numChunks && viewedChunkCoord.x > -1*numChunks && viewedChunkCoord.y > -1*numChunks){
         return true;
       }
     }
     return false;
   }

   public Vector3 getChunkFaceAndOffset(int currFace, Vector2 viewedChunkCoord){
     // our chunk will have been checked by checkChunkFace()
     int numChunks = faceSize/chunkSize;
     int newFace;
     int rotation;
     if (viewedChunkCoord.x < 0) {
          newFace = Planet.adjacentFaces[currFace,0];
          rotation = Planet.adjacentRotations[currFace,0];
     } else if (viewedChunkCoord.y >= numChunks) {
          newFace = Planet.adjacentFaces[currFace,1];
          rotation = Planet.adjacentRotations[currFace,1];
     } else if (viewedChunkCoord.x >= numChunks) {
          newFace = Planet.adjacentFaces[currFace,2];
          rotation = Planet.adjacentRotations[currFace,2];
     } else if (viewedChunkCoord.y < 0) {
          newFace = Planet.adjacentFaces[currFace,3];
          rotation = Planet.adjacentRotations[currFace,3];
     } else {
          // viewed chunk is in the current face
          //Debug.Log("viewing chunks in current face");
          return new Vector3(currFace,viewedChunkCoord.x,viewedChunkCoord.y);
     }
     //Debug.Log("viewing chunks in another face");
     // get coordinate of chunk in the new face
     viewedChunkCoord = new Vector2(((viewedChunkCoord.x % numChunks)+numChunks)%numChunks, ((viewedChunkCoord.y % numChunks)+numChunks)%numChunks);
     ////Debug.Log(viewedChunkCoord);
     //Vector2 midPoint = new Vector2((int) numChunks/2, (int) numChunks/2);

     // rotate 'rotation' amount round the midPoint

     viewedChunkCoord = viewedChunkCoord - midPoint;
     // now rotate around origin
     Vector2 newViewedChunkCoord = new Vector2(-viewedChunkCoord.y * rotation, viewedChunkCoord.x*rotation);
     newViewedChunkCoord = newViewedChunkCoord + midPoint;
     return new Vector3(newFace,newViewedChunkCoord.x,newViewedChunkCoord.y);


   }


   public int getFaceNumber(Vector3 position) { // or get local up?
     // get angles between position and localUp directions
     // direction with min angle to position is the current localUp
     float minAngle = Vector3.Angle(position, Planet.localUps[0]);
     int faceNumber = 0;
     for (int i = 1; i < 6; i++){
       float angle = Vector3.Angle(position, Planet.localUps[i]);
        if (angle < minAngle) {
          minAngle = angle;
          faceNumber = i;
        }
     }
     Debug.Log(faceNumber);
     return faceNumber;
   }


   public Vector2 getCurrentChunkCoord(int currFace, Vector3 position){
     localUp = Planet.localUps[currFace];
     axisB = new Vector3(localUp.y, localUp.z, localUp.x);
     axisA = Vector3.Cross(localUp, axisB);
     Vector3 projectionOntoUnitSphere = position;//position.normalized; // + planet position
     float angle = Vector3.Angle(localUp, projectionOntoUnitSphere)*(Mathf.PI/180f);
     //distAxis = Vector3.Cross(localUp, projectionOntoUnitSphere);
     //Vector3 absDistAxis = Vector3.Scale(distAxis,distAxis);
     //float angle = Vector3.SignedAngle(localUp, projectionOntoUnitSphere, )*(Mathf.PI/180f);
     //Debug.Log(localUp);

     float distance = Mathf.Tan(angle)*faceSize*.5f;//planetRadius; //Mathf.Sin(angle)*planetRadius;//

     Vector3 notLocalUp = (Vector3.one - Vector3.Scale(localUp,localUp));
     //Debug.Log(distance);


     Vector3 absAxisA = axisA;//Vector3.Scale(axisA,axisA);
     //float angle2 = Vector3.Angle(axisA,Vector3.Scale(projectionOntoUnitSphere, notLocalUp))*(Mathf.PI/180f);
     float angle2 = Vector3.SignedAngle(Vector3.Scale(projectionOntoUnitSphere, notLocalUp),absAxisA,localUp)*(Mathf.PI/180f);
     //Debug.Log(angle2);

     /*
     // reduce axisA, axisB to 2D
     Vector2 axisA2D = Vector2.zero;
     Vector2 axisB2D = Vector2.zero;

     for (int i = 0, n = 0; i < 3; i++) {
        if (localUp[i] == 0) {
           axisA2D[n] = absAxisA[i];
           axisB2D[n] = axisB[i];
           n++;
        }
     }
     */
     //Vector2 coord2D = new Vector2((axisA2D.x*Mathf.Cos(angle2) - axisA2D.y*Mathf.Sin(angle2)), axisA2D.x*Mathf.Sin(angle2) + axisA2D.y*Mathf.Cos(angle2)); // rotation matrix approach
     //coord2D = coord2D*distance;

     //Vector2 coord2D = ((axisB2D * (Mathf.Cos(angle2)*distance)) + (axisA2D * (-Mathf.Sin(angle2)*distance))); // add axis vectors approach

     // following works cause our axisA happens to always point 'upwards' - with chunk 0,0 at 'top left'
     Vector2 coord2D = new Vector2((Mathf.Cos(angle2)*distance), (Mathf.Sin(angle2)*distance));

     //Debug.Log(coord2D);
     return coord2D;

   }




	  IEnumerator UpdateVisibleChunks() {
      int offset = (int) ((faceSize/chunkSize)*.5f);
      //float offset = (float) ((faceSize/chunkSize)*.5f);
      for (;;){
        viewerPosition = new Vector2 (playerTransform.position.x, playerTransform.position.z);

        Vector3 relativePosition = playerTransform.position - planetTransform.position;
  		  //int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
  		  //int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);
        int currFace =  getFaceNumber(relativePosition);
        //Debug.Log(midPoint);
        yield return null;
        Vector2 coord2D = getCurrentChunkCoord(currFace, relativePosition);
        yield return null;


        //int currentChunkCoordX = Mathf.RoundToInt(coord2D.x/chunkSize) + offset;
        //int currentChunkCoordY = Mathf.RoundToInt(coord2D.y/chunkSize) + offset;

        int currentChunkCoordX = (int)((coord2D.x/chunkSize) + offset);// + offset;
        int currentChunkCoordY = (int)((coord2D.y/chunkSize) + offset);// + offset;
        Debug.Log(coord2D);
        Debug.Log(new Vector2(currentChunkCoordX,currentChunkCoordY));
        //Debug.Log(currentChunkCoordY);
  		  for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
  			  for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {

            // get the LOD we want this chunk to be
            int desired_LOD;
            if (Mathf.Abs(yOffset)<=1 && Mathf.Abs(xOffset)<=1) {
              desired_LOD = 1;
            } else if (Mathf.Abs(yOffset)<=2 && Mathf.Abs(xOffset)<=2) {
              desired_LOD = 2;
            } else {
              desired_LOD = 3;
            }


  				   Vector2 gridChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

             int face;
             Vector2 viewedChunkCoord;

             if (!checkChunkFace(gridChunkCoord)){
              // Debug.Log("skipping chunk");
              //Debug.Log(gridChunkCoord);
               continue;//break; // if we dont need this chunk, we skip handling it
             }
             ////Debug.Log(" ");

             Vector3 key = getChunkFaceAndOffset(currFace,gridChunkCoord); //new Tuple<int,Vector2>(face,viewedChunkCoord);
             face = (int) key.x;
             //////Debug.Log(face);
             viewedChunkCoord = new Vector2(key.y,key.z);
             //Debug.Log("");
             //Debug.Log(gridChunkCoord);
             //Debug.Log(viewedChunkCoord);
             if (terrainChunkDictionary.ContainsKey (key)) {
               // we already have a chunk here
               // handle changing of LOD if necessary ----------------------------------------------------------------------
               //yield return null;
               if (terrainChunkDictionary[key].LOD != desired_LOD){
                 terrainChunkDictionary[key].SetVisible(false);
                 terrainChunkDictionary[key] = new TerrainChunk(face,viewedChunkCoord, chunkSize, planetTransform, desired_LOD, planetName);
                 yield return null;
               }
               terrainChunkDictionary [key].SetVisible(true);
             } else {
               // generate chunk for the first time
               terrainChunkDictionary.Add (key, new TerrainChunk (face,viewedChunkCoord, chunkSize, planetTransform, desired_LOD, planetName));
               //terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
               yield return null;
             }
             // list visible chunks
             terrainKeysVisible.Add(key);

  			}
  		}
      yield return null;

      // set not 'visible' for appropriate previously viewed chunks
      for (int i = 0; i < terrainKeysVisibleLastUpdate.Count; i++) {
          if (!(terrainKeysVisible.Contains(terrainKeysVisibleLastUpdate[i]))){
              terrainChunkDictionary[terrainKeysVisibleLastUpdate[i]].SetVisible(false);
              yield return null;
          }
      }
      terrainKeysVisibleLastUpdate.Clear();
      terrainKeysVisibleLastUpdate.AddRange(terrainKeysVisible);
      //terrainCoordsVisibleLastUpdate = terrainCoordsVisible;
      yield return null;
      terrainKeysVisible.Clear();

    }
	}


    public class TerrainChunk {

     //GameObject meshObject;
     GameObject mesh;
     GameObject water;
     GameObject ice;

     public int LOD;
     public int face;
     public string planetName;
     Vector2 position;
     Bounds bounds;

     //MeshRenderer meshRenderer;
     //MeshFilter meshFilter;


     public TerrainChunk(int f, Vector2 coord, int size, Transform parent, int l, string name) {
       face = f;
       LOD = l;
       planetName = name;
       int chunkSize = size;
       position = coord * size;
       bounds = new Bounds(position,Vector2.one * size);
       Vector3 positionV3 = new Vector3(position.x,0,position.y);



       //Debug.Log("generating");


       // generate ground ------------------------------------------------
       mesh = new GameObject();
       mesh.transform.SetParent(parent);
       mesh.transform.localPosition = new Vector3(0,0,0);
       mesh.AddComponent<MeshGenerator>();

       MeshGenerator mG = mesh.GetComponent<MeshGenerator>();
       mG.planetName = planetName;
       mG.face = face;
       mG.LOD = LOD;
       //Debug.Log(position);
       mG.offset_x = (int) position.x;//chunkSize*x;
       mG.offset_z = (int) position.y;//chunkSize*z;



       // generate water -------------------------------------------------
       water = new GameObject();
       water.transform.SetParent(parent);
       water.transform.localPosition = new Vector3(0,0,0);
       water.AddComponent<WaterMeshGenerator>();

       WaterMeshGenerator water_mG = water.GetComponent<WaterMeshGenerator>();
       water_mG.planetName = planetName;
       water_mG.face = face;
       water_mG.LOD = LOD;
       water_mG.offset_x = (int) position.x;//chunkSize*x;
       water_mG.offset_z = (int) position.y;//chunkSize*z;



       // generate ice ---------------------------------------------------
       ice = new GameObject();
       ice.transform.SetParent(parent);
       ice.transform.localPosition = new Vector3(0,0,0);
       ice.AddComponent<IceMeshGenerator>();

       IceMeshGenerator ice_mG = ice.GetComponent<IceMeshGenerator>();
       ice_mG.planetName = planetName;
       ice_mG.face = face;
       ice_mG.LOD = LOD;
       ice_mG.offset_x = (int) position.x;//chunkSize*x;
       ice_mG.offset_z = (int) position.y;//chunkSize*z;



       // generate vegetation --------------------------------------------
       //Vegetation_Generator.generate(chunkSize*x,chunkSize*z);



       /*
       meshObject = new GameObject("Terrain Chunk");
       meshRenderer = meshObject.AddComponent<MeshRenderer>();
       meshFilter = meshObject.AddComponent<MeshFilter>();
       meshRenderer.material = material;

       meshObject.transform.position = positionV3;
       meshObject.transform.parent = parent;
       SetVisible(false);
       */
       //mapGenerator.RequestMapData(OnMapDataReceived);
     }

     /*
     void OnMapDataReceived(MapData mapData) {
       mapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
     }

     void OnMeshDataReceived(MeshData meshData) {
       meshFilter.mesh = meshData.CreateMesh ();
     }
     */

     public void UpdateTerrainChunk() {

       float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewerPosition));
       bool visible = viewerDstFromNearestEdge <= maxViewDst;
       SetVisible (visible);
     }

     public void SetVisible(bool visible) {
       mesh.SetActive (visible);
       water.SetActive (visible);
       ice.SetActive (visible);
     }

     public bool IsVisible() {
       return mesh.activeSelf;
     }

     //public bool getLOD(){
    //   return LOD;
     //}

    }
}




///////
