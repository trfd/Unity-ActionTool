//
// ValueProvider.cs
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

using System.Reflection;
using Utils.Reflection;

using Utils;

namespace ActionTool
{
    public enum ProviderKind
    {
        OBJECT_MEMBER,
        // OBJECT_METHOD,
        ACTION_VARIABLE,
        CONSTANT_VALUE
    }

    [System.Serializable]
    public class ValueProvider<T>
    {
       
        #region Public Members

        public ProviderKind _kind;
        public GameObject _object;
        public ComponentNestedDataMemberWrapper _nestedDataMember;
        public GPActionVariable _actionVariable;
        public T _constValue;

        #endregion


        public T GetValue()
        {
           switch(_kind)
           {
               case ProviderKind.ACTION_VARIABLE: return (T) _actionVariable.GetValue();
               case ProviderKind.OBJECT_MEMBER: return (T) _nestedDataMember.GetValue();
               case ProviderKind.CONSTANT_VALUE: return _constValue;
           }

           return default(T);
        }

        #region Static Interface 

        #region ActionVariable

        public static GPActionVariable[] ActionVariablesInGameObject(GameObject obj)
        {
            List<GPActionVariable> vars = new List<GPActionVariable>();

            if (obj == null)
                return vars.ToArray();

            EventHandler[] handlers = obj.GetComponents<EventHandler>();

            if (handlers.Length == 0)
                return vars.ToArray();

            foreach(EventHandler handler in handlers)
            {
                GPAction[] actions = handler.GetGPActionObjectMapperOrCreate().GetAllActions(handler);

                foreach(GPAction action in actions)
                    if(action is GPActionVariable)
                        vars.Add((GPActionVariable)action);
            }

            return vars.ToArray();
        }

        #endregion

        #endregion
    }

    [System.Serializable]
    public class IntValueProvider : ValueProvider<int>
    {}

    [System.Serializable]
    public class FloatValueProvider : ValueProvider<float>
    {}

    [System.Serializable]
    public class StringValueProvider : ValueProvider<string>
    {}

    [System.Serializable]
    public class BoolValueProvider : ValueProvider<bool>
    {}

	[System.Serializable]
	public class GameObjectValueProvider : ValueProvider<GameObject>
	{}
}
