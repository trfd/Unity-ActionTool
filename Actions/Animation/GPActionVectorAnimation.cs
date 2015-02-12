using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
	[System.Serializable]
	[GPActionAlias("Animation/Vector")]
	public class GPActionVectorAnimation : GPAction 
	{
		#region Private Members

		private Timer m_timer;

		#endregion

		#region Public Members

        public Vector4ValueProvider _provider;

		public float _duration;

		public RandomAnimationCurve _curveX;
        public RandomAnimationCurve _curveY;
        public RandomAnimationCurve _curveZ;
        public RandomAnimationCurve _curveW;

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

            Vector4 v = _provider.GetValue();

            v.x = _curveX.Evaluate(1f - m_timer.CurrentNormalized);
            v.y = _curveY.Evaluate(1f - m_timer.CurrentNormalized);
            v.z = _curveZ.Evaluate(1f - m_timer.CurrentNormalized);
            v.w = _curveW.Evaluate(1f - m_timer.CurrentNormalized);

            _provider.SetValue(v);
		}

		#endregion
	}
}