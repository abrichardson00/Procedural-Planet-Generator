using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{

    public GameObject planet;
    public Planet planetGenerator;

    public Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
      // generate sun
      GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      MeshRenderer sunRenderer = sun.GetComponent<MeshRenderer>();
      sunRenderer.material = Resources.Load("Sun",typeof(Material)) as Material;
      sun.transform.position = new Vector3(-15000, 0, 0);
      sun.transform.localScale = new Vector3(5000,5000,5000);
      //Light sunlight = sun.AddComponent<Light>();
      //sunlight.type = LightType.Directional;

      // generate planet
      planet = new GameObject("planet1");
      planet.transform.position = new Vector3(4000,0,0);
      planetGenerator = planet.AddComponent<Planet>() as Planet;
      //planetGenerator.planetTransform = planet.transform;

      playerTransform = GameObject.Find("Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
