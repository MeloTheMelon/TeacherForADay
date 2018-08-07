using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherController : BasicAvatarController
{

	// Use this for initialization
	public override void Start ()
    {
        // find transforms of model
        SpineBase = GameObject.Find("Hips").transform;
        //SpineMid = GameObject.Find("LowerBack").transform;
        Neck = GameObject.Find("Neck").transform;
        Head = GameObject.Find("Head").transform;
        ShoulderLeft = GameObject.Find("LeftArm").transform;
        ElbowLeft = GameObject.Find("LeftForeArm").transform;
        WristLeft = GameObject.Find("LeftHand").transform;
        //HandLeft = GameObject.Find("").transform;
        ShoulderRight = GameObject.Find("RightArm").transform;
        ElbowRight = GameObject.Find("RightForeArm").transform;
        WristRight = GameObject.Find("RightHand").transform;
        //HandRight = GameObject.Find("").transform;
        HipLeft = GameObject.Find("LeftUpLeg").transform;
        KneeLeft = GameObject.Find("LeftLeg").transform;
        //AnkleLeft = GameObject.Find("Character1_LeftFoot").transform;
        //FootLeft = GameObject.Find("Character1_LeftToeBase").transform;
        HipRight = GameObject.Find("RightUpLeg").transform;
        KneeRight = GameObject.Find("RightLeg").transform;
        //AnkleRight = GameObject.Find("Character1_RightFoot").transform;
        //FootRight = GameObject.Find("Character1_RightToeBase").transform;
        //SpineShoulder = GameObject.Find("Spine").transform;
        //HandTipLeft = GameObject.Find("Character1_LeftHandIndex1").transform;
        //ThumbLeft = GameObject.Find("Character1_LeftHandThumb1").transform;
        //HandTipRight = GameObject.Find("Character1_RightHandIndex1").transform;
        //ThumbRight = GameObject.Find("Character1_RightHandThumb1").transform;

        base.Start();
    }

    // Update is called once per frame
    public override void Update ()
    {
        // apply base Update function, which rotates all known standard joints
        base.Update();
    }
}
