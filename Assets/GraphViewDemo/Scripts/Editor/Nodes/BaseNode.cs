using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public abstract class BaseNode : Node
    {
        protected BaseNode()
        {
            AddToClassList("BaseNode");
            StyleSheetUtils.AddStyleSheets(this, "Node");
            title = this.DisplayName;
        }
        public string GUID { get; set; }

        public abstract string DisplayName { get; }

        public virtual Vector2 Size => new Vector2(200, 150);

        public abstract void ConstructView();

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public Port AddPort(Direction direction, Port.Capacity capacity, string portName)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(string));
            port.portName = portName;
            port.name = portName;
            switch (direction)
            {
            case Direction.Input:
                this.inputContainer.Add(port);
                break;
            case Direction.Output:
                this.outputContainer.Add(port);
                break;
            }
            return port;
        }

        public static BaseNode CreateNode<T>() where T : BaseNode, new()
        {
            var node = new T()
            {
                GUID = Guid.NewGuid().ToString()
            };
            node.ConstructView();
            return node;
        }

        public static BaseNode LoadNode(CommonNodeData commonNodeData)
        {
            string typeName = commonNodeData.nodeTypeId;
            Type type = ReflectionUtils.GetType(typeName);
            BaseNode node = default;
            try
            {
                node = JsonUtility.FromJson(commonNodeData.serializedData, type) as BaseNode;
            }
            catch
            {
                Debug.LogError($"deserialize failed from json: {commonNodeData.serializedData}");
            }
            finally
            {
                if (node == null)
                {
                    node = Activator.CreateInstance(type) as BaseNode;
                }
            }

            node.GUID = commonNodeData.nodeGUID;
            node.ConstructView();
            return node;
        }
    }
}
