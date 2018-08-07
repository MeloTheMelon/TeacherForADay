using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CustomTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    protected TrackableBehaviour mTrackableBehaviour;

    TrackableBehaviour.Status currentStatus;
    
    MarkerTrackingManager trackingManager;

    [SerializeField, Tooltip("ID of the marker, used for identification and position interpretation")]
    int markerID;
    
    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        currentStatus = TrackableBehaviour.Status.UNKNOWN;

        trackingManager = FindObjectOfType<MarkerTrackingManager>();

        if(trackingManager != null)
        {
            trackingManager.RegisterMarker(this, markerID);
        }
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        currentStatus = newStatus;
    }
    
    public TrackableBehaviour.Status GetStatus()
    {
        return currentStatus;
    }

    public Quaternion GetMarkerOffset()
    {
        return transform.localRotation;
        //return (transform.localToWorldMatrix * Matrix4x4.Rotate(transform.localRotation)).rotation;
    }
}
