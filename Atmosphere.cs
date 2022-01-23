using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    Transform playerTransform;
    GameObject planet;
    //GameObject sky;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Camera").transform;
        planet = GameObject.Find("planet1");
        //GameObject sky = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //MeshRenderer renderer = sky.GetComponent<MeshRenderer>();
        //renderer.material = Resources.Load("Ultra Skybox Fog/Materials/rustig_koppie_4k",typeof(Material)) as Material;
        gameObject.transform.SetParent(planet.transform);
        gameObject.transform.localPosition = new Vector3(0,0,0);
        //gameObject.transform.position = playerTransform.position;
        //GameObject.Find("SolarSystemGenerator").GetComponent<GameObject>();


    }

    public Vector3 getUpDirection() {
        return (playerTransform.position - planet.transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
      //Vector3 up = getUpDirection();
      //gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(up.y,up.z,up.x), up);

    }
}
