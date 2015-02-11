using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
	[System.Serializable]
	[GPActionAlias("Animation/Color Component Animation")]
	public class GPActionColorComponentAnimation : GPAction 
	{
		#region Private Members

		private Timer m_timer;

		#endregion

		#region Public Members

        public ColorValueProvider _provider;

        public ColorComponent _component;

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

            Color comp = _provider.GetValue();

            float value = _curve.Evaluate(1f - m_timer.CurrentNormalized);

            switch(_component)
            {
                case ColorComponent.R: comp.r = value; break;
                case ColorComponent.G: comp.g = value; break;
                case ColorComponent.B: comp.b = value; break;
                case ColorComponent.A: comp.a = value; break;
            }

            _provider.SetValue(comp);
		}

		#endregion
	}
}