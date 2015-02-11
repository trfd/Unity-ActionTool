using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
	[System.Serializable]
	[GPActionAlias("Animation/Vector2 Component Animation")]
	public class GPActionVector2ComponentAnimation : GPAction 
	{
		#region Private Members

		private Timer m_timer;

		#endregion

		#region Public Members

        public Vector2ValueProvider _provider;

        public Vector2Component _component;

		public float _duration;

		public AnimationCurve _curve;

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

            Vector2 comp = _provider.GetValue();

            float value = _curve.Evaluate(1f - m_timer.CurrentNormalized);

            switch(_component)
            {
                case Vector2Component.X: comp.x = value; break;
                case Vector2Component.Y: comp.y = value; break;
            }

            _provider.SetValue(comp);
		}

		#endregion
	}
}