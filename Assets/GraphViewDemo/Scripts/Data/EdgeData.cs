using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.GraphViewDemo
{
    [Serializable]
    public class EdgeData
    {
        public string sourceNodeGUID;
        public string sourcePort;
        public string targetNodeGUID;
        public string targetPort;
    }
}
