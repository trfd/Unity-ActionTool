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

        public FloatValueProvider _provider;

		public float _duration;

		public RandomAnimationCurve _curve;

		#endregion

		#region GPAction Override

		protected override void OnTrigger()
		{
			m_timer = new Timer(_duration);
		}

		protected override void OnUpdate()
		{
			if(m_timer.IsElapsedLoop)
			{
				End();
				return;
			}

            _provider.SetValue(_curve.Evaluate(1f - m_timer.CurrentNormalized));
		}

		#endregion
	}
}