using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamMirror : MonoBehaviour {

    

	// Use this for initialization
	void Start ()
    {
        RawImage rawImage = GetComponent<RawImage>();

        WebCamTexture webcamTexture = new WebCamTexture();
        //webcamTexture.deviceName = WebCamTexture.devices[0].name;
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Num Devices: " + devices.Length);
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

        webcamTexture.deviceName = "Logitech HD Webcam C615";
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
