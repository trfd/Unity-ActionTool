//
// GPActionUtils.cs
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
    public static class GPActionUtils
    {
        #region Static Members

        /// <summary>
        /// Name used to designate the GPActionObject (that is the object holding GPAction)
        /// </summary>
        public const string c_ActionObjectName = "__ActionObject__";

        #endregion

        #region Interface

        /// <summary>
        /// Creates a default GPActionObject if not existing.
        /// Returns the created GameObject or the existing GPActionObject
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GameObject CreateGPActionObject(GameObject parentObj)
        {
            GameObject gpaObj = GetGPActionObject(parentObj);

            if (gpaObj != null)
                return gpaObj;

            gpaObj = new GameObject(c_ActionObjectName);

            InitGPActionObject(parentObj, gpaObj);

            return gpaObj;
        }

        /// <summary>
        /// Init a GameObject owner GPAction (a so-called "GPActionObject").
        /// That is, the object is hid in the hierarchy and parent transform is 
        /// set to parent gameobject
        /// </summary>
        /// <param name="gpactionParent"></param>
        /// <param name="gpactionObj"></param>
        public static void InitGPActionObject(GameObject gpactionParent, GameObject gpactionObj)
        {
            gpactionObj.hideFlags = HideFlags.HideInHierarchy;

            gpactionObj.transform.parent = gpactionParent.transform;

            gpactionObj.AddComponent<GPActionObjectMapper>();
        }

        /// <summary>
        /// Destroys an GPActionObject
        /// </summary>
        /// <param name="parentObj"></param>
        public static void DestroyGPActionObject(GameObject parentObj)
        {
            GameObject gpaObj = GetGPActionObject(parentObj);

            if (gpaObj == null)
                return;

            GameObject.Destroy(gpaObj);
        }

        /// <summary>
        /// Returns whether or not the specified gameobject has a GPActionObject attached
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static bool HasGPActionObject(GameObject parentObj)
        {
            return (GetGPActionObject(parentObj) != null);
        }

        /// <summary>
        /// Returns the attached GPActionObject if any
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GameObject GetGPActionObject(GameObject parentObj)
        {
            GameObject gpactionObj = null;

            for (int i = 0; i < parentObj.transform.childCount; i++)
                if (parentObj.transform.GetChild(i).gameObject.name == c_ActionObjectName)
                    gpactionObj = parentObj.transform.GetChild(i).gameObject;

            return gpactionObj;
        }

        /// <summary>
        /// Returns the GPActionObject attached to the specified GameObject if any.
        /// Otherwise creates it and return the newly created GPActionObject;
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GameObject GetGPActionObjectOrCreate(GameObject parentObj)
        {
            GameObject obj = GetGPActionObject(parentObj);

            if (obj != null)
                return obj;

            return CreateGPActionObject(parentObj);
        }

        /// <summary>
        /// Returns the attached GPActionObjectMapper if any
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GPActionObjectMapper GetGPActionObjectMapper(GameObject parentObj)
        {
            GameObject gpactionObj = GetGPActionObject(parentObj);

            if (gpactionObj == null)
                return null;

            return gpactionObj.GetComponent<GPActionObjectMapper>();
        }

        /// <summary>
        /// Returns the GPActionObjectMapper attached to the specified GameObject if any.
        /// Otherwise creates a GPActionObject it and return the newly created GPActionObjectMapper;
        /// </summary>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static GPActionObjectMapper GetGPActionObjectMapperOrCreate(GameObject parentObj)
        {
            GPActionObjectMapper obj = GetGPActionObjectMapper(parentObj);

            if (obj != null)
                return obj;

            return CreateGPActionObject(parentObj).GetComponent<GPActionObjectMapper>();
        }

        #endregion
    }
}