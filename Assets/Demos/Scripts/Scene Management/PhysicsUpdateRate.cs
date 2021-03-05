using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUpdateRate : MonoBehaviour {

    public int updatesPerSecond = 60;

	void Start ()
    {
        Time.fixedDeltaTime = 1f / updatesPerSecond;
	}
}
