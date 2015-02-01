//
// GPConditionCompareDistance.cs
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

using Utils;

namespace ActionTool
{
	[GPConditionAlias("Basic/Distance")]
	public class GPConditionCompareDistance : GPCondition
	{
		#region Private Members
		
		private FloatComparer m_comparer;
		
		#endregion
		
		#region Public Members
		
		public TypeWrapper _comparerType;
		public GameObjectValueProvider _providerA;
		public GameObjectValueProvider _providerB;

		public FloatValueProvider _distance;
		
		#endregion
		
		public override bool Evaluate ()
		{
			if(m_comparer == null)
			{
				if(_comparerType != null && _comparerType.IsValid)
					m_comparer = (FloatComparer) System.Activator.CreateInstance(_comparerType.Type);
				else
					throw new System.NullReferenceException("Null comparer");
			}

			GameObject a = _providerA.GetValue();
			GameObject b = _providerB.GetValue();

			if(a == null || b == null)
				return false;

			float relative = Vector3.Distance(a.transform.position, b.transform.position);

			return m_comparer.Compare(relative,_distance.GetValue());
		}
	}
}
