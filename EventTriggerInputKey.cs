using UnityEngine;
using System.Collections;

namespace ActionTool
{
	[AddComponentMenu("EventTrigger/Input/EventTriggerInputKey")]
	public class EventTriggerInputKey : MonoBehaviour
	{
		#region Public Members

		public KeyCode _key;

		public GPEventID _eventID;

		#endregion

		#region MonoBehaviour Override

		void Update () 
		{
			if(Input.GetKeyDown(_key))
				EventManager.Instance.PostEvent(_eventID.Name);
		}

		#endregion
	}
}
