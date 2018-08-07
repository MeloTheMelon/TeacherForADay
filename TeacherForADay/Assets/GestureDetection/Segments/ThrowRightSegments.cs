using UnityEngine;
using Windows.Kinect;

/// <summary>
/// The first part of the throw right gesture
/// </summary>
public class ThrowRightSegment1 : IRelativeGestureSegment
{
    /// <summary>
    /// Checks the gesture.
    /// </summary>
    /// <param name="skeleton">The skeleton.</param>
    /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
    public GesturePartResult CheckGesture(BasicAvatarModel skeleton)
    {

        // right hand above right shoulder
        Vector3 handRight = skeleton.getRawWorldPosition(JointType.HandRight);
        Vector3 shoulderCenter = skeleton.getRawWorldPosition(JointType.SpineShoulder);
        Vector3 elbowRight = skeleton.getRawWorldPosition(JointType.ElbowRight);

        if (handRight.y > shoulderCenter.y)
        {
            // right hand above right elbow

            if (handRight.y > elbowRight.y)
            {
                // right hand behind right elbow

                if(handRight.z < elbowRight.z)
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
/// The second part of the throw right gesture
/// </summary>
public class ThrowRightSegment2 : IRelativeGestureSegment
{
    /// <summary>
    /// Checks the gesture.
    /// </summary>
    /// <param name="skeleton">The skeleton.</param>
    /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
    public GesturePartResult CheckGesture(BasicAvatarModel skeleton)
    {
        // right hand below right shoulder
        Vector3 handRight = skeleton.getRawWorldPosition(JointType.HandRight);
        Vector3 shoulderCenter = skeleton.getRawWorldPosition(JointType.SpineShoulder);
        Vector3 elbowRight = skeleton.getRawWorldPosition(JointType.ElbowRight);

        if (handRight.y > shoulderCenter.y)
        {
            // right hand in front right elbow

            if (handRight.z > elbowRight.z)
            {
                // right elbow in front of right shouder
                Vector3 shoulderRight = skeleton.getRawWorldPosition(JointType.ShoulderRight);

                if (elbowRight.z > shoulderRight.z)
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