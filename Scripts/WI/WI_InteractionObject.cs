using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WI {

public class WI_InteractionObject : MonoBehaviour {


	protected Transform cachedTransform; //1
	[HideInInspector] //2
	public WI_InteractionController currentController; //3



	public virtual void OnTriggerWasPressed(WI_InteractionController controller)
	{
		currentController = controller;
	}


	public virtual void OnTriggerIsBeingPressed(WI_InteractionController controller)
	{
	}

	public virtual void OnTriggerWasReleased(WI_InteractionController controller)
	{
		currentController = null;
	}

	public virtual void OnButton3Pressed(WI_InteractionController controller)
	{
		currentController = controller;
	}

	public virtual void OnPressDown(WI_InteractionController controller)
	{
		currentController = controller;
	}

	public virtual void OnPressUp(WI_InteractionController controller)
	{
		currentController = controller;
	}

	public virtual void Awake()
	{
		cachedTransform = transform; //1
		if ( !gameObject.CompareTag("InteractionObject")) //
		{
			Debug.LogWarning ("This InteractionObject does not have the correct tag, setting it now.", gameObject); //3
			gameObject.tag = "InteractionObject"; //4
	}
}

	public bool IsFree()
	{
		return currentController == null;
	}

	public virtual void OnDestroy()
	{
		if (currentController) {
			OnTriggerWasReleased (currentController);
		}
	}
}
}