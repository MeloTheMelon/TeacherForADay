using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Windows.Kinect;

public class Teacher : MonoBehaviour {

    public GestureController gc;
    public BasicAvatarModel AvatarModel;
    public UnityChanController chanController;

    //Marker Tracking
    public MarkerTrackingManager trackingManager;

    // Material attached
    private Material material;

    public RectTransform screamOMeter;

    // Game Manager Reference
    public Manager manager;
    public GameObject ballPrefab;
    
    public float throwHeight, throwRandomness;
    
    private bool isWriting, blocked, dodging;

    public float loudness;

    public float micLevel, wakeUpLevel, micAmplify;

    //GestureRecognition
    private bool throwingLeft, throwingRight;

    public bool debugEnabled;
    public UnityEngine.AudioSource grunt, chalkboard;

    // Use this for initialization
    void Start () {
        material = GetComponent<Renderer>().material;

        //Initiate Gesture Recognition
        gc.GestureRecognizedInController += OnGestureRecognized;

        IRelativeGestureSegment[] throwLeft = { new ThrowLeftSegment1(), new ThrowLeftSegment2() };
        gc.AddGesture("ThrowLeft", throwLeft);
        IRelativeGestureSegment[] throwRight = { new ThrowRightSegment1(), new ThrowRightSegment2() };
        gc.AddGesture("ThrowRight", throwRight);

        throwingLeft = false;
        throwingRight = false;

        chalkboard.volume = 0;

        micAmplify = PlayerPrefs.GetFloat("Amplify");
        wakeUpLevel = PlayerPrefs.GetFloat("Threshold");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Manager.paused) { return; }

        micLevel = MicInput.MicLoudness*micAmplify;

        micLevel = Mathf.Max(Mathf.Min(micLevel, 100),0);
        screamOMeter.anchorMax = new Vector2(micLevel / 100f, 0.01f);

        Control();
    }

    void OnGestureRecognized(object sender, GestureEventArgs e)
    {
        if (e.GestureName == "ThrowLeft")
        {
            Debug.Log("ThrowLeft Recognized");

            throwingLeft = true;
        }
        if (e.GestureName == "ThrowRight")
        {
            Debug.Log("ThrowRight Recognized");

            throwingRight = true;
        }
    }

    void OnGUI()
    {
        if (debugEnabled)
        {
            GUI.Label(new Rect(10, 10, 200, 200), "Amplify: " + micAmplify);
            GUI.Label(new Rect(10, 40, 200, 200), "Threshold: " + wakeUpLevel);
            GUI.Label(new Rect(10, 70, 200, 200), "Level: " + micLevel);
        }
    }

    void Control()
    {
        bool isBackMarkerTracked = trackingManager.IsBackMarkerTracked();
        bool isFrontmarkerTracked = trackingManager.IsFrontMarkerTracked();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            micAmplify += 100;
            PlayerPrefs.SetFloat("Amplify", micAmplify);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            micAmplify -= 100;
            PlayerPrefs.SetFloat("Amplify", micAmplify);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            wakeUpLevel -= 1;
            PlayerPrefs.SetFloat("Threshold", wakeUpLevel);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            wakeUpLevel += 1;
            PlayerPrefs.SetFloat("Threshold", wakeUpLevel);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            debugEnabled = !debugEnabled;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            micLevel = wakeUpLevel + 1;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            manager.speed += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            manager.speed -= 0.5f;
        }

        //Debug.Log("" + isFrontmarkerTracked + "   " + isBackMarkerTracked);

        //Set useFixedSkeleton in UnityChanController if backMarker is tracked
        if (isBackMarkerTracked)
        {
            chanController.useFixedSkeleton = true;
        }
        else
        {
            chanController.useFixedSkeleton = false;
        }

        // Throw
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Throw());
        }    
        
        if(isFrontmarkerTracked)
        {
            if (throwingLeft)
            {
                StartCoroutine(Throw(false));
                throwingLeft = false;
            }
            if (throwingRight)
            {
                StartCoroutine(Throw(true));
                throwingRight = false;
            }
        }
        else
        {
            throwingLeft = false;
            throwingRight = false;
        }
    

        // Write
        isWriting = false;
        if (!blocked)
        {
            material.color = Color.white;
        }        
        if (isBackMarkerTracked)
        {
            if(!blocked)
            {
                
                //At least one hand needs to be above shoulder to count as writing
                Vector3 handRight = AvatarModel.getRawWorldPosition(JointType.HandRight);
                Vector3 handLeft = AvatarModel.getRawWorldPosition(JointType.HandLeft);
                Vector3 shoulderCenter = AvatarModel.getRawWorldPosition(JointType.HandRight);
                if (handRight.y > shoulderCenter.y || handLeft.y > shoulderCenter.y)
                {
                    isWriting = true;
                    Manager.AddScore(1 * Time.deltaTime);
                    material.color = Color.green;
                }
                else
                {
                    //Always count as writing while looking towards blackboard. Use this together with fixed character skeleton
                    isWriting = true;
                    Manager.AddScore(1 * Time.deltaTime);
                    Manager.IncreaseMultiplier(Time.deltaTime);
                    material.color = Color.green;
                }

                
            }
        }    
        if (Input.GetKey(KeyCode.W))
        {
            if (!blocked)
            {
                isWriting = true;
                Manager.AddScore(1*Time.deltaTime);
                Manager.IncreaseMultiplier(Time.deltaTime);
                material.color = Color.green;
            }
        }

        if (isWriting)
        {
            chalkboard.volume = 1;
        }
        else
        {
            chalkboard.volume = 0;
        }        
    }

    // Interactions
    IEnumerator Throw(bool rightHand = true)
    {
        if (!blocked)
        {
            blocked = true;

            material.color = Color.red;

            Student target = manager.GetRandomTalkingStudent();

            if (target != null)
            {
                Vector3 spawnPos = AvatarModel.getRawWorldPosition(rightHand ? JointType.HandRight : JointType.HandLeft);

                SpawnBall(target, spawnPos);

                yield return new WaitForSeconds(0.5f);

                HitStudent(target);

                material.color = Color.white;
            }

            blocked = false;
        }
    }

    void HitStudent(Student target)
    {
        if (target != null) {
            target.hit = true;

            Manager.AddScoreSingle(50);
        }
    }

    void SpawnBall(Student target, Vector3 position)
    {
        Projectile ball = Instantiate(ballPrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();

        ball.start = transform;
        ball.end = target.transform;
        ball.height = throwHeight;
        ball.randomness = throwRandomness;
        ball.targetTag = "Student";
    }
}
