//
// GPActionMoveTo.cs
//
// Author(s):
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//       Fabien Ziebel <nexibaf@gmail.com>
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

namespace ActionTool
{	
	/// <summary>
	/// Move the parent object according to start/end point. 
	/// This impl. is based on MoveBehaviour.
	/// </summary>
	[GPActionAlias("Basic/Move To")]
	public class GPActionMoveTo : GPAction
	{
		#region Private Members

		private float m_moveCurrentTime = 0f;

		#endregion

		#region Public Members
		
		public Transform _startPoint;
		public Transform _endPoint;
		public bool _toEnd = false;
		public bool _backNForward = true;
		
		public float _moveDuration = 1f;
		
		public AnimationCurve _moveCurve;
		
		#endregion

		#region Internal State

		private bool IsPathSet ()
		{
			if (_startPoint == null)
				return false;
			if (_endPoint == null)
				return false;
			return true;
		}

		#endregion

		#region GPAction Override
		
		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
		protected override void OnTrigger()
		{
			if (!IsPathSet())
				Debug.LogError("Error : A move Path lacks either a start or an end");
		}

		/// <summary>
		/// Raised each frame while action is running.
		/// Calling GPAction.End or GPAction.Stop will stop updates.
		/// </summary>
		/// <param name="dt">Dt.</param>
		protected override void OnUpdate()
		{
			if (!IsPathSet())
				return;
			
			if (_toEnd && m_moveCurrentTime != _moveDuration)
			{
				m_moveCurrentTime += Time.deltaTime ;
				if( m_moveCurrentTime >= _moveDuration)
				{
					m_moveCurrentTime = _moveDuration ;
					if (_backNForward) _toEnd = false;
				}
				
			}
			else if (!_toEnd && m_moveCurrentTime != 0)
			{
				m_moveCurrentTime -= Time.deltaTime ;
				if( m_moveCurrentTime <= 0)
				{
					m_moveCurrentTime = 0 ;
					if (_backNForward) _toEnd = true;
				}
				
			}
			else
			{
				return;
			}
			
			float ratioWay = m_moveCurrentTime / _moveDuration;
	
			this.ParentGameObject.transform.position = 
				Vector3.Lerp (_startPoint.position, _endPoint.position, _moveCurve.Evaluate(ratioWay));

			if(m_moveCurrentTime >= _moveDuration)
				End();
		}

		#endregion

		#region Gizmos

		public override void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Render ();
		}
		
		public override void OnDrawGizmos() 
		{
			Gizmos.color = Color.green;
			Render ();
		}

		/// <summary>
		/// Render this instance.
		/// </summary>
		protected virtual void Render()
		{
			if (!IsPathSet())
				return;
			float sphereRayon = 0.2f;
			
			Gizmos.DrawSphere(_startPoint.position + (_endPoint.position - _startPoint.position)/2,0.1f);
			
			Gizmos.DrawWireSphere(_startPoint.position,sphereRayon);
			Gizmos.DrawWireCube(_endPoint.position,new Vector3(sphereRayon,sphereRayon,sphereRayon));
			Gizmos.DrawLine (_endPoint.position, _startPoint.position);
			
		}

		#endregion
	}
}