using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.GraphViewDemo
{
    [Serializable]
    public class EasyGraphAsset : ScriptableObject
    {
        public List<CommonNodeData> commonNodeData = new List<CommonNodeData>();
        public List<EdgeData> edgeData = new List<EdgeData>();
        public List<CommentGroupData> commentGroupData = new List<CommentGroupData>();
    }
}
