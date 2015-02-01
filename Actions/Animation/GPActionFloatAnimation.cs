using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
	[System.Serializable]
	[GPActionAlias("Animation/Float")]
	public class GPActionFloatAnimation : GPAction 
	{
		#region Private Members

		private Timer m_timer;

		#endregion

		#region Public Members

		public Component _component;

		public DataMemberWrapper _member;

		public float _duration;

		public RandomAnimationCurve _curve;

		#endregion

		#region GPAction Override

		protected override void OnTrigger()
		{
			if(_component == null || _member == null || _member.GetMember() == null)
			{
				End();
				Debug.LogError("Null Component or Member");
				return;
			}

			m_timer = new Timer(_duration);
		}

		protected override void OnUpdate()
		{
			if(m_timer.IsElapsedLoop)
			{
				End();
				return;
			}

			_member.SetValue(_component, _curve.Evaluate(1f - m_timer.CurrentNormalized));
		}

		#endregion
	}
}