using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Transform start, end;

    public float diffX, diffZ;
    public float time, height, speed, randomness, startPosY, offset;

    public string targetTag;

	// Use this for initialization
	void Start ()
    {
        diffX = end.position.x + Random.Range(-randomness, randomness) - start.position.x;
        diffZ = end.position.z + Random.Range(-randomness, randomness) - start.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        CalculatePath();
	}

    void CalculatePath()
    {

        float startX = start.position.x;
        float endX = end.position.x;

        // fx = ax^2 + bx +c
        // f'x = 2*a*x + b
        // 0 = (0 - x)*(1 - x)
        // 0 = startx*endx - endx*x - startxx + x^2
        
        float currentY = time * time - time;
        currentY *= -1 * height;

        float currentOffset = Mathf.Lerp(0, offset, time);

        transform.position = start.position + new Vector3(time*diffX, currentY + startPosY + currentOffset, time*diffZ);

        time += Time.deltaTime * speed;

        if(time > 1.5f) { Destroy(gameObject); }
        
        time += Time.deltaTime * speed;

        if (time > 1.5f) { Destroy(gameObject); }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag))
        {
            if (other.tag.Equals("Teacher"))
            {
                Manager.KillMultiplier();
                Teacher teacher = GameObject.Find("Teacher").GetComponent<Teacher>();
                teacher.grunt.PlayOneShot(teacher.grunt.clip);
            }

            Destroy(gameObject);
        }
    }
}
