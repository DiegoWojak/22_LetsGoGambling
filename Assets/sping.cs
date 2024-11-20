using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sping : MonoBehaviour
{
    public Transform[] objects;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(anim());
    }

    IEnumerator anim()
    {
        for(int i = 0; i < objects.Length; i++)
        {
            LeanTween.value(objects[i].gameObject, 5, -5, 1).setLoopCount(-1);
        }

        yield return new WaitForFixedUpdate();
    }
}
