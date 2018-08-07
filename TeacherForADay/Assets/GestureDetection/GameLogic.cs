using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public GestureController gc;

	// Use this for initialization
	void Start () {
        gc.GestureRecognizedInController += OnGestureRecognized;
        /**
        IRelativeGestureSegment[] swipeLeft = { new SwipeToLeftSegment1(), new SwipeToLeftSegment2(), new SwipeToLeftSegment3() };
        gc.AddGesture("SwipeLeft", swipeLeft);
        IRelativeGestureSegment[] waveLeft = { new WaveLeftSegment1(), new WaveLeftSegment2(),
                                             new WaveLeftSegment1(),new WaveLeftSegment2(),
                                             new WaveLeftSegment1(),new WaveLeftSegment2() };
        gc.AddGesture("WaveLeft", waveLeft);

        IRelativeGestureSegment[] pullLeft = { new PullToLeftSegment1(), new PullToLeftSegment2(), new PullToLeftSegment3(), };
        gc.AddGesture("PullLeft", pullLeft);
    */
        IRelativeGestureSegment[] throwLeft = { new ThrowLeftSegment1(), new ThrowLeftSegment2() };
        gc.AddGesture("ThrowLeft", throwLeft);

        IRelativeGestureSegment[] throwRight = { new ThrowRightSegment1(), new ThrowRightSegment2() };
        gc.AddGesture("ThrowRight", throwRight);
                
	}

    void OnGestureRecognized(object sender, GestureEventArgs e)
    {
        if (e.GestureName == "SwipeLeft")
        {
            Debug.Log("Swipe Recognized");

        }
        if (e.GestureName == "WaveLeft")
        {
            Debug.Log("Wave Recognized");
        }
        if (e.GestureName == "PullLeft")
        {
            Debug.Log("PullLeft Recognized");
        }
        if(e.GestureName == "ThrowLeft")
        {
            Debug.Log("ThrowLeft Recognized");
        }
        if(e.GestureName == "ThrowRight")
        {
            Debug.Log("ThrowRight Recognized");
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
