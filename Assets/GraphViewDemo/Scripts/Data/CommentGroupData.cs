using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.GraphViewDemo
{
    [Serializable]
    public class CommentGroupData
    {
        public List<string> managedNodes = new List<string>();
        public Vector2 position;
        public string title= "Comment Here";
    }
}
