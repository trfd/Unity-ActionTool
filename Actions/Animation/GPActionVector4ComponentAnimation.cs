using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
	[System.Serializable]
	[GPActionAlias("Animation/Vector4 Component Animation")]
	public class GPActionVector4ComponentAnimation : GPAction 
	{
		#region Private Members

		private Timer m_timer;

		#endregion

		#region Public Members

        public Vector4ValueProvider _provider;

        public Vector4Component _component;

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

            Vector4 comp = _provider.GetValue();

            float value = _curve.Evaluate(1f - m_timer.CurrentNormalized);

            switch(_component)
            {
                case Vector4Component.X: comp.x = value; break;
                case Vector4Component.Y: comp.y = value; break;
                case Vector4Component.Z: comp.z = value; break;
                case Vector4Component.W: comp.w = value; break;
            }

            _provider.SetValue(comp);
		}

		#endregion
	}
}