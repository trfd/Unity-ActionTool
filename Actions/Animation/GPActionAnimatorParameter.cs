using UnityEngine;
using System.Collections;

namespace ActionTool
{
	[GPActionAlias("Animation/AnimatorParameter")]
	public class GPActionAnimatorParameter : GPAction
	{
		public enum Kind
		{
			TRIGGER,
			BOOL,
			INTEGER,
			FLOAT
		}

		#region Public Members

		public Kind _kind;

		public string _parameter;

		public bool _boolValue;
		public int _intValue;
		public float _floatValue;

		#endregion

		#region GPAction Override

		protected override void OnTrigger()
		{
			Animator animator = ParentGameObject.GetComponent<Animator>();

			if(animator == null)
			{
				End ();
				return;
			}

			switch(_kind)
			{
			case Kind.TRIGGER: animator.SetTrigger(_parameter);
				break;
			case Kind.BOOL: animator.SetBool(_parameter,_boolValue);
				break;
			case Kind.INTEGER: animator.SetInteger(_parameter,_intValue);
				break;
			case Kind.FLOAT: animator.SetFloat(_parameter,_floatValue);
				break;
			}

			End ();
		}

		#endregion
	}
}