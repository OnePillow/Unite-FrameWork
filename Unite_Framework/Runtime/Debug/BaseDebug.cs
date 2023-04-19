using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDebug : MonoBehaviour
{
	public  Canvas canvas;
    private BaseDebug instance;

	public void Awake()
	{
		instance = this;
	}
}
