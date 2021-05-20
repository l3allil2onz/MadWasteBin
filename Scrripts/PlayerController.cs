using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	void Update ()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 1f;

        transform.Translate(x, 0, 0);
	}
}
