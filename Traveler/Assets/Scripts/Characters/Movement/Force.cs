using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force {
	public Vector3 MyForce;
	public float Duration;

	public Force (Vector2 force, float dur)
	{
        MyForce = force;
		Duration = dur;
	}

	public Force()
	{
        MyForce = new Vector2(0,0);
		Duration = 0;
	}
}
