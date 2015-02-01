//
// GPActionTimer.cs
//
// Author(s):
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//
// Copyright (c) 2014
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
	[GPActionAlias("Variable/Float/Timer")]
	public class GPActionTimer : GPActionVariable
	{
		public enum Kind
		{
			START_STOP, // Start with the first trigger and stop with the second
			RESTART     // Restart timer every time action triggered
		}

		#region Private Members

		/// <summary>
		/// Time elapsed since last action trigger
		/// </summary>
		private float m_timeElapsed;

		/// <summary>
		/// timeSinceLevelLoad when action is trigger
		/// </summary>
		private float m_startTime;

		/// <summary>
		/// Holds whether or not the timer is running
		/// </summary>
		private bool m_running;

		#endregion

		#region Public Members

		public Kind m_timerKind;

		#endregion

		#region Properties

		public float TimeElapsed
		{
			get{ return m_timeElapsed; }
		}

		#endregion

		#region GPAction Override

		protected override void OnTrigger ()
		{
			if(!m_running)
				m_timeElapsed = Time.timeSinceLevelLoad;
			else
			{
				if(m_timerKind == Kind.START_STOP)
					m_running = false;
				else
					m_timeElapsed = Time.timeSinceLevelLoad;
			}

			End();
		}


#if UNITY_EDITOR
		
		public override void DrawWindowContent()
		{
			base.DrawWindowContent();

			GetValue();

			GUILayout.Label(m_timeElapsed.ToString());
		}
		
#endif

		#endregion

		#region GPActionVariable Override

		public override object GetValue ()
		{
			if(m_running)
				m_timeElapsed = Time.timeSinceLevelLoad - m_timeElapsed;

			return m_timeElapsed;
		}

		#endregion
	}
}
