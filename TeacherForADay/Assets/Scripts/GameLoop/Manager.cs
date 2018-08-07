using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public static Manager current;

    // All Students
    public List<Student> students, freeStudents, talkingStudents;

    // Length of this Round
    public float time;

    // Timer for next Event
    public float eventTimer, speed;

    // Score System
    public float score;
    public float multiplier;

    public TextMesh scoreText, multiplierText;

    public UIManager ui;

    public AudioSource schoolBell, noise;

    public static bool paused;
    public bool pauseGame;

    // Use this for initialization
    void Start () {
        current = this;
        eventTimer = speed;
	}
	
	// Update is called once per frame
	void Update () {
        if (!paused)
        {
            CheckStatus();
            TimerControl();
            UpdateText();
        }
        paused = pauseGame;

        if (Input.GetKeyDown(KeyCode.A))
        {
            TriggerEvent();
        }
	}
    

    void CheckStatus()
    {
        freeStudents = new List<Student>();
        talkingStudents = new List<Student>();        
        int asleepCount = 0;

        foreach(Student student in students)
        {
            if (!student.blocked)
            {
                freeStudents.Add(student);
            }
            if (student.sleeping)
            {
                asleepCount++;
            }
            if (student.talking)
            {
                talkingStudents.Add(student);
            }
        }

        noise.volume = talkingStudents.Count / (float)students.Count;
        noise.volume /= 2;

        if(time <= 0)
        {
            Debug.Log("Your Score!");
            PlayerPrefs.Save();
            paused = true;
            pauseGame = true;
            schoolBell.gameObject.SetActive(true);
            ui.openEndScreen((int)score);
        }

        time -= Time.deltaTime;
    }
    

    // Controls The Timer
    void TimerControl()
    {
        eventTimer -= Time.deltaTime;

        if(eventTimer <= 0)
        {
            TriggerEvent();
            eventTimer = speed;
        }
    }

    // Triggers an Event
    void TriggerEvent()
    {
        //Debug.Log("Event!");
        bool blocked = true;
        int select;
        Student selectedStudent = null;

        int counter = 0;

        select = Random.Range(0, 3);

        while (blocked)
        {
            int selectS = Random.Range(0, students.Count);
            selectedStudent = students[selectS];
            blocked = selectedStudent.blocked;

            if (select == 2 && !blocked)
            {
                bool block1 = false;
                bool block2 = false;

                if(selectedStudent.neighbours.Count > 0 && selectedStudent.neighbours[0] != null && selectedStudent.neighbours[0].blocked) { block1 = true; }
                if(selectedStudent.neighbours.Count > 0 && selectedStudent.neighbours[0] == null) { block1 = true; }
                if (selectedStudent.neighbours.Count > 1 && selectedStudent.neighbours[1] != null && selectedStudent.neighbours[1].blocked) { block2 = true; }
                if (selectedStudent.neighbours.Count > 1 && selectedStudent.neighbours[1] == null) { block1 = true; }

                blocked = block1 && block2;
            }

            counter++;

            if(counter >= 50) { Debug.Log("Blocked!"); return; }
        }
        

        switch (select)
        {
            case 0:
                StartCoroutine(selectedStudent.Throw());
                break;
            case 1:
                StartCoroutine(selectedStudent.Sleep());
                break;
            case 2:
                StartCoroutine(selectedStudent.Talk());
                break;
            default:
                break;
        }
    }


    public Student GetRandomTalkingStudent()
    {
        if (talkingStudents.Count == 0)
        {
            return null;
        }

        int select = Random.Range(0, talkingStudents.Count);
        Student selectedStudent = talkingStudents[select];

        return selectedStudent;
    }

    public static void AddScore(float points)
    {
        // Add points to current score
        current.score += points * (int)current.multiplier * current.freeStudents.Count;        
    }

    public static void AddScoreSingle(float points)
    {
        // Add points to current score
        current.score += points * (int)current.multiplier;
    }

    public static void IncreaseMultiplier(float inc)
    {
        //Debug.Log(inc);
        // Increase multiplier
        current.multiplier += inc;
    }

    public static void KillMultiplier()
    {
        current.multiplier = 1;
    }

    void UpdateText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + (int)score;
        }
        if (multiplierText != null)
        {
            multiplierText.text = (int)multiplier + ".0X";
        }
    }
}
