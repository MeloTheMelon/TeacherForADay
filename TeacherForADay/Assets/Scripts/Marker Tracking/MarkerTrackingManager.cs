using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MarkerTrackingManager : MonoBehaviour {

    public GameObject PlayerCharacter;

    Dictionary<int, CustomTrackableEventHandler> registeredMarkers = new Dictionary<int, CustomTrackableEventHandler>();

    //Rotation smoothing
    public float lerpFactor;
    public float maxAutoRotation;
    public int bufferSize;
    float[] buffer;
    int currentIndex = 0;
    
	// Use this for initialization
	void Start ()
    {
        if (lerpFactor <= 0 || lerpFactor > 1)
            lerpFactor = 1;
        if (maxAutoRotation <= 0)
            maxAutoRotation = Mathf.Infinity;
        if (bufferSize <= 0)
            bufferSize = 1;
        buffer = new float[bufferSize];
	}
    

    // Update is called once per frame
    void Update ()
    {        
        Quaternion markerOrientation;

        if(CalculateCurrentMarkerOrientation(out markerOrientation))
        {
            //Save previous Y-Rotation
            float rotOld = PlayerCharacter.transform.rotation.eulerAngles.y;

            //Set Player Character to the tracked marker orientation     
            PlayerCharacter.transform.rotation = Quaternion.Lerp(PlayerCharacter.transform.rotation, markerOrientation, lerpFactor);

            //Get new Y-Rotation
            float rotNew = PlayerCharacter.transform.rotation.eulerAngles.y;

            //Calculate angular velocity and save that to buffer
            float diff = rotNew - rotOld;
            float min = Mathf.Abs(diff) <= Mathf.Abs(diff + 360) ? diff : diff + 360;//Always use the smaller of the 2 possible rotations
            float angVel = min / Time.deltaTime;
            angVel = Mathf.Clamp(angVel, -1 * maxAutoRotation, maxAutoRotation);
            AddItem(angVel);
            /** 
            //Some Debug outputs
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Old: " + rotOld);
                Debug.Log("New: " + rotNew);
                Debug.Log("diff: " + diff);
                Debug.Log("min: " + min);
                Debug.Log("Adding item: " + angVel);
                Debug.Log("Average: " + GetBufferAverage());
            }
            */
        }
        else
        {
            //Calculate new Player Character rotation by using smoothed rotation speed from previous frames
            //Debug.Log(GetBufferAverage());
            PlayerCharacter.transform.Rotate(0, GetBufferAverage() * Time.deltaTime, 0);
        }
	}

    public bool IsBackMarkerTracked()
    {
        return registeredMarkers.Count > 1 && registeredMarkers[1].GetStatus() == TrackableBehaviour.Status.TRACKED;
    }

    public bool IsFrontMarkerTracked()
    {
        return registeredMarkers.Count > 0 && registeredMarkers[0].GetStatus() == TrackableBehaviour.Status.TRACKED;
    }

    //returns if markers did provide a valid orientation
    private bool CalculateCurrentMarkerOrientation(out Quaternion orientation)
    {
        Vector4 componentSums = new Vector4(0, 0, 0, 0);
        int numValidMarkers = 0;
        Quaternion firstRotation = Quaternion.identity;

        foreach (int key in registeredMarkers.Keys)
        {
            if (registeredMarkers[key].GetStatus() == TrackableBehaviour.Status.TRACKED)
            {
                numValidMarkers++;
                Quaternion markerRotation = registeredMarkers[key].GetMarkerOffset();
                Vector3 markerRotEuler = markerRotation.eulerAngles;

                //Magic transformation to negate the initial differences in orientation between Marker and UnityChan
                //sumMarkerRotations += new Vector3(markerRotEuler.x * -1, markerRotEuler.z, markerRotEuler.y);//Use this for all 3 rotation axis
                Quaternion newRot = Quaternion.Euler(0, markerRotEuler.z, 0);

                //Calculate Quaternion Average. Originalcode siehe #region am Ende des files
                if (numValidMarkers == 1)
                {
                    firstRotation = newRot;
                    componentSums.w += newRot.w;
                    componentSums.x += newRot.x;
                    componentSums.y += newRot.y;
                    componentSums.z += newRot.z;
                }
                else
                {
                    if (!AreQuaternionsClose(newRot, firstRotation))
                    {
                        newRot = InverseSignQuaternion(newRot);
                    }
                    componentSums.w += newRot.w;
                    componentSums.x += newRot.x;
                    componentSums.y += newRot.y;
                    componentSums.z += newRot.z;
                }
            }
        }

        if (numValidMarkers > 0)
        {
            float numInv = 1 / (float)numValidMarkers;
            float w = componentSums.w * numInv;
            float x = componentSums.x * numInv;
            float y = componentSums.y * numInv;
            float z = componentSums.z * numInv;

            Quaternion averagedRotation = NormalizeQuaternion(x, y, z, w);

            orientation = averagedRotation;
            return true;
        }

        orientation = Quaternion.identity;
        return false;
    }

    public void RegisterMarker(CustomTrackableEventHandler marker, int markerID)
    {
        if(registeredMarkers.ContainsKey(markerID))
        {
            Debug.LogWarning("Duplicate marker ID!");
        }
        else
        {
            registeredMarkers.Add(markerID, marker);
        }
    }

    #region CIRCULAR_BUFFER

    private void AddItem(float item)
    {
        buffer[currentIndex] = item;
        currentIndex++;
        currentIndex %= bufferSize;
    }

    private float GetBufferAverage()
    {
        float sum = 0;
        foreach (float f in buffer)
        {
            sum += f;
        }

        return sum / bufferSize;
    }

    #endregion //CIRCULAR_BUFFER

    //Code taken from http://wiki.unity3d.com/index.php/Averaging_Quaternions_and_Vectors
    #region Quaternion_Average


    //Get an average (mean) from more then two quaternions (with two, slerp would be used).
    //Note: this only works if all the quaternions are relatively close together.
    //Usage: 
    //-Cumulative is an external Vector4 which holds all the added x y z and w components.
    //-newRotation is the next rotation to be added to the average pool
    //-firstRotation is the first quaternion of the array to be averaged
    //-addAmount holds the total amount of quaternions which are currently added
    //This function returns the current average quaternion
    public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
    {

        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
        //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
        if (!AreQuaternionsClose(newRotation, firstRotation))
        {
            newRotation = InverseSignQuaternion(newRotation);
        }

        //Average the values
        float addDet = 1f / (float)addAmount;
        cumulative.w += newRotation.w;
        w = cumulative.w * addDet;
        cumulative.x += newRotation.x;
        x = cumulative.x * addDet;
        cumulative.y += newRotation.y;
        y = cumulative.y * addDet;
        cumulative.z += newRotation.z;
        z = cumulative.z * addDet;

        //note: if speed is an issue, you can skip the normalization step
        return NormalizeQuaternion(x, y, z, w);
    }

    public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
    {

        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    //Changes the sign of the quaternion components. This is not the same as the inverse.
    public static Quaternion InverseSignQuaternion(Quaternion q)
    {

        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    //Returns true if the two input quaternions are close to each other. This can
    //be used to check whether or not one of two quaternions which are supposed to
    //be very similar but has its component signs reversed (q has the same rotation as
    //-q)
    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
    {

        float dot = Quaternion.Dot(q1, q2);

        if (dot < 0.0f)
        {

            return false;
        }

        else
        {

            return true;
        }
    }

    #endregion //Quaternion_Average
}
