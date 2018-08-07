using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class UnityChanController : BasicAvatarController
{
    public bool useFixedSkeleton = false;

	public override void Start(){
        
        /**
        // find transforms of model
        SpineBase = GameObject.Find("Character1_Hips").transform;
        //SpineMid = GameObject.Find("Character1_Spine2").transform;
        Neck = GameObject.Find("Character1_Neck").transform;
        Head = GameObject.Find("Character1_Head").transform;
        ShoulderLeft = GameObject.Find("Character1_RightArm").transform;
        ElbowLeft = GameObject.Find("Character1_RightForeArm").transform;
        WristLeft = GameObject.Find("Character1_RightHand").transform;
        //HandLeft = GameObject.Find("").transform;
        ShoulderRight = GameObject.Find("Character1_LeftArm").transform;
        ElbowRight = GameObject.Find("Character1_LeftForeArm").transform;
        WristRight = GameObject.Find("Character1_LeftHand").transform;
        //HandRight = GameObject.Find("").transform;
        HipLeft = GameObject.Find("Character1_RightUpLeg").transform;
        KneeLeft = GameObject.Find("Character1_RightLeg").transform;
        AnkleLeft = GameObject.Find("Character1_RightFoot").transform;
        FootLeft = GameObject.Find("Character1_RightToeBase").transform;
        HipRight = GameObject.Find("Character1_LeftUpLeg").transform;
        KneeRight = GameObject.Find("Character1_LeftLeg").transform;
        AnkleRight = GameObject.Find("Character1_LeftFoot").transform;
        FootRight = GameObject.Find("Character1_LeftToeBase").transform;
        SpineShoulder = GameObject.Find("Character1_Spine2").transform;
        //HandTipLeft = GameObject.Find("Character1_LeftHandIndex1").transform;
        //ThumbLeft = GameObject.Find("Character1_LeftHandThumb1").transform;
        //HandTipRight = GameObject.Find("Character1_RightHandIndex1").transform;
        //ThumbRight = GameObject.Find("Character1_RightHandThumb1").transform;
    */
    
        // find transforms of model
        SpineBase = GameObject.Find("Character1_Hips").transform;
        //SpineMid = GameObject.Find("Character1_Spine2").transform;
        Neck = GameObject.Find("Character1_Neck").transform;
        Head = GameObject.Find("Character1_Head").transform;
        ShoulderLeft = GameObject.Find("Character1_LeftArm").transform;
        ElbowLeft = GameObject.Find("Character1_LeftForeArm").transform;
        WristLeft = GameObject.Find("Character1_LeftHand").transform;
        //HandLeft = GameObject.Find("").transform;
        ShoulderRight = GameObject.Find("Character1_RightArm").transform;
        ElbowRight = GameObject.Find("Character1_RightForeArm").transform;
        WristRight = GameObject.Find("Character1_RightHand").transform;
        //HandRight = GameObject.Find("").transform;
        HipLeft = GameObject.Find("Character1_LeftUpLeg").transform;
        //KneeLeft = GameObject.Find("Character1_LeftLeg").transform;
        //AnkleLeft = GameObject.Find("Character1_LeftFoot").transform;
        //FootLeft = GameObject.Find("Character1_LeftToeBase").transform;
        HipRight = GameObject.Find("Character1_RightUpLeg").transform;
        //KneeRight = GameObject.Find("Character1_RightLeg").transform;
        //AnkleRight = GameObject.Find("Character1_RightFoot").transform;
        //FootRight = GameObject.Find("Character1_RightToeBase").transform;
        //SpineShoulder = GameObject.Find("Character1_Spine2").transform;
        //HandTipLeft = GameObject.Find("Character1_LeftHandIndex1").transform;
        //ThumbLeft = GameObject.Find("Character1_LeftHandThumb1").transform;
        //HandTipRight = GameObject.Find("Character1_RightHandIndex1").transform;
        //ThumbRight = GameObject.Find("Character1_RightHandThumb1").transform;


        base.Start();
    }

    public virtual void Update()
    {
        if(useFixedSkeleton)
        {
            foreach (JointType jt in knownJoints.Keys)
            {
                //Set initial rotation for all joints
                knownJoints[jt].rotation = RootTransform.rotation * initialModelJointRotations[jt];
            }

            //Set custom rotation for joints that should be displayed differently
            //Eyeballed values taken from editor
            knownJoints[JointType.ShoulderRight].localEulerAngles = new Vector3(24.9f, 11.1f, -59.6f);
            knownJoints[JointType.ElbowRight].localEulerAngles = new Vector3(23.7f, -4.4f, -71.55f);
            knownJoints[JointType.ShoulderLeft].localEulerAngles = new Vector3(-5.5f, 74.6f, -7.2f);
            knownJoints[JointType.ElbowLeft].localEulerAngles = new Vector3(-3f, 13.4f, -16.6f);

        }
        else
        {
            // apply base Update function, which rotates all known standard joints
            base.Update();
        }
    }

    
	
	
}
