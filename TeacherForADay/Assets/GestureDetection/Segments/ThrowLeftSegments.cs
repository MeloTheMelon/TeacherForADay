using UnityEngine;
using Windows.Kinect;

/// <summary>
/// The first part of the throw left gesture
/// </summary>
public class ThrowLeftSegment1 : IRelativeGestureSegment
{
    /// <summary>
    /// Checks the gesture.
    /// </summary>
    /// <param name="skeleton">The skeleton.</param>
    /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
    public GesturePartResult CheckGesture(BasicAvatarModel skeleton)
    {

        // left hand above left shoulder
        Vector3 handLeft = skeleton.getRawWorldPosition(JointType.HandLeft);
        Vector3 shoulderCenter = skeleton.getRawWorldPosition(JointType.SpineShoulder);
        Vector3 elbowLeft = skeleton.getRawWorldPosition(JointType.ElbowLeft);
        

        if (handLeft.y > shoulderCenter.y)
        {
            // left hand above left elbow

            if (handLeft.y > elbowLeft.y)
            {
                // left hand behind left elbow
                if (handLeft.z < elbowLeft.z)
                {
                    //Debug.Log("Segment1 Success");
                    return GesturePartResult.Succeed;
                }
                return GesturePartResult.Pausing;
            }
            return GesturePartResult.Fail;
        }
        return GesturePartResult.Fail;
    }
}

/// <summary>
/// The second part of the throw left gesture
/// </summary>
public class ThrowLeftSegment2 : IRelativeGestureSegment
{
    /// <summary>
    /// Checks the gesture.
    /// </summary>
    /// <param name="skeleton">The skeleton.</param>
    /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
    public GesturePartResult CheckGesture(BasicAvatarModel skeleton)
    {
        // left hand stays above left elbow
        Vector3 handLeft = skeleton.getRawWorldPosition(JointType.HandLeft);
        Vector3 shoulderCenter = skeleton.getRawWorldPosition(JointType.SpineShoulder);
        Vector3 elbowLeft = skeleton.getRawWorldPosition(JointType.ElbowLeft);

        if (handLeft.y > elbowLeft.y)
        {
            // left hand in front of left elbow
            
            if (handLeft.z > elbowLeft.z)
            {
                // left elbow in front of left shoulder
                Vector3 shoulderLeft = skeleton.getRawWorldPosition(JointType.ShoulderLeft);
                
                if (elbowLeft.z > shoulderLeft.z)
                {
                    //Debug.Log("Segment2 Success");
                    return GesturePartResult.Succeed;
                }
                return GesturePartResult.Pausing;
            }
            return GesturePartResult.Pausing;
        }
        return GesturePartResult.Fail;
    }
}