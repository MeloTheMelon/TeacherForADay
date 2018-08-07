using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour {

    private Animator anim;

    //private Material material;

    public Teacher teacher;
    public GameObject ballPrefab;

    public List<Student> neighbours;

    public bool blocked, sleeping, talking, hit;

    private float throwHeight = 3, throwRandomness = 0f, throwSpeed = 1;

    public AudioSource snoring, paperCrumbling, ouch;

	// Use this for initialization
	void Start () {
        //material = GetComponent<Renderer>().material;
        anim = GetComponent<Animator>();
        snoring.gameObject.SetActive(false);
        paperCrumbling.loop = true;
        paperCrumbling.pitch = Random.Range(0.9f, 1.1f);
        paperCrumbling.volume = 0;
	}
	
	// Update is called once per frame
	void Update () {

	}


    // Interactions
    public IEnumerator Throw()
    {
        if (!blocked)
        {
            blocked = true;

            paperCrumbling.volume = 1;

            yield return new WaitForSeconds(2);

            paperCrumbling.volume = 0;

            anim.SetTrigger("Throw");

            //material.color = Color.red;

            yield return new WaitForSeconds(1f);

            SpawnBall();

            yield return new WaitForSeconds(1);

            //material.color = Color.white;

            blocked = false;
        }
    }

    void SpawnBall()
    {
        Projectile ball = Instantiate(ballPrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();

        Debug.Log(transform.ToString());
        ball.start = this.transform;
        Debug.Log(ball.start.ToString());
        ball.end = teacher.transform;
        ball.height = throwHeight;
        ball.randomness = throwRandomness;
        ball.targetTag = "Teacher";

        ball.transform.LookAt(teacher.transform);
    }

    public IEnumerator Sleep()
    {
        if (!blocked)
        {
            blocked = true;
            sleeping = true;
            anim.SetBool("Sleep", true);
            snoring.gameObject.SetActive(true);
            //material.color = Color.cyan;

            yield return new WaitUntil(() => teacher.micLevel > teacher.wakeUpLevel || Manager.paused);

            //material.color = Color.white;
            snoring.gameObject.SetActive(false);
            anim.SetBool("Sleep", false);
            sleeping = false;
            blocked = false;
        }
    }

    public IEnumerator Talk()
    {
        if (!blocked)
        {
            blocked = true;
            talking = true;

            // Select Neighbour
            bool found = false;
            int select = 0;

            while (!found)
            {
                select = Random.Range(0, 2);

                if (neighbours[select] != null && !neighbours[select].blocked) { found = true; }
            }

            if (select == 0)
            {
                anim.SetBool("TalkLeft", true);
                StartCoroutine(neighbours[select].Talk(1));
            }
            else
            {
                anim.SetBool("TalkRight", true);
                StartCoroutine(neighbours[select].Talk(0));
            }
            //material.color = Color.magenta;

            yield return new WaitUntil(() => hit || Manager.paused);

            ouch.Play();

            //material.color = Color.white;
            if (select == 0)
            {
                anim.SetBool("TalkLeft", false);
                neighbours[select].hit = neighbours[select].talking;
            }
            else
            {
                anim.SetBool("TalkRight", false);
                neighbours[select].hit = neighbours[select].talking;
            }
            hit = false;
            talking = false;
            blocked = false;
        }
    }

    public IEnumerator Talk(int direction)
    {
        if (!blocked)
        {
            blocked = true;
            talking = true;
            int select = direction;
            if (select == 0) { anim.SetBool("TalkLeft", true); }
            else { anim.SetBool("TalkRight", true); }
            //material.color = Color.magenta;

            yield return new WaitUntil(() => hit || Manager.paused);

            //material.color = Color.white;
            if (select == 0)
            {
                anim.SetBool("TalkLeft", false);
                neighbours[select].hit = neighbours[select].talking;
            }
            else
            {
                anim.SetBool("TalkRight", false);
                neighbours[select].hit = neighbours[select].talking;
            }
            hit = false;
            talking = false;
            blocked = false;
        }
    }
}
