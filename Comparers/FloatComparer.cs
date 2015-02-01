//
// FloatComparer.cs
//
// Author:
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
    [System.Serializable]
    [ValueComparerHide]
    public class FloatComparer : ValueComparer<float>
    {}

    [System.Serializable]
    [ValueComparerAlias("Float/Greater")]
    public class FloatGreater : FloatComparer
    { public override bool Compare(float a, float b) { return a > b; } }

    [System.Serializable]
    [ValueComparerAlias("Float/Greater Or Equal")]
    public class FloatGreaterOrEqual : FloatComparer
    { public override bool Compare(float a, float b) { return a >= b; } }

	[System.Serializable]
	[ValueComparerAlias("Float/Less")]
	public class FloatLess : FloatComparer
	{ public override bool Compare(float a, float b) { return a < b; } }
	
	[System.Serializable]
	[ValueComparerAlias("Float/Less Or Equal")]
	public class FloatLessOrEqual : FloatComparer
	{ public override bool Compare(float a, float b) { return a <= b; } }

    [System.Serializable]
    [ValueComparerAlias("Float/Equal")]
    public class FloatrEqual : FloatComparer
    { public override bool Compare(float a, float b) { return a == b; } }
}