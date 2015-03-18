using UnityEngine;
using System.Collections;

namespace ActionTool
{
	[AddComponentMenu("EventTrigger/Input/EventTriggerInputKey")]
	public class EventTriggerInputKey : MonoBehaviour
	{
		#region Public Members
        public enum InputType
        {
            KEYCODE,
            INPUTNAME
        }

        public InputType _inputType = InputType.KEYCODE;

		public KeyCode _keyCode;
        public string _inputName;

		public GPEventID _eventID;

		#endregion

		#region MonoBehaviour Override

		void Update () 
		{
            bool eventFlag = false;

            switch (_inputType)
            {
                case InputType.KEYCODE:
                    eventFlag = Input.GetKeyDown(_keyCode);
                    break;
                case InputType.INPUTNAME:
                    eventFlag = Input.GetButtonDown(_inputName);
                    break;
            }

            if (eventFlag)
            {
                EventManager.Instance.PostEvent(_eventID.Name);
            }
			
		}

		#endregion
	}
}
