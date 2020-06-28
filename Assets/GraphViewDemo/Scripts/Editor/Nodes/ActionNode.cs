using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public class ActionNode : BaseNode
    {
        public string data;

        public override string DisplayName => "ActionNode";

        public override void ConstructView()
        {
            AddPort(Direction.Input, Port.Capacity.Multi, "Input");
            AddPort(Direction.Output, Port.Capacity.Single, "Output");

            this.RefreshPorts();

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                this.data = evt.newValue;
            });
            textField.SetValueWithoutNotify(this.data);
            this.mainContainer.Add(textField);

            this.RefreshExpandedState();
        }
    }
}
