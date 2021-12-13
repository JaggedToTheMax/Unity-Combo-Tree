using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace ComboTree.Editor
{
    public class StateView : Node
    {
        public SerializedState State { get; protected set; }
        public Port Input { get; private set; }
        public Port Output { get; private set; }

        public StateView(SerializedState state)
        {
            State = state;
            style.left = State.position.x;
            style.top = State.position.y;

            this.Q<Label>("title-label").bindingPath = "m_Name";
            this.Bind(new SerializedObject(state));

            switch (state.name)
            {
                case SerializedState.EntryName:
                    InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(SerializedTransition));
                    break;
                case SerializedState.AnyName:
                    InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(SerializedTransition));
                    break;
                case SerializedState.ExitName:
                    InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(SerializedTransition));
                    break;
                default:
                    InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(SerializedTransition));
                    InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(SerializedTransition));
                    break;
            }
            AddToClassList(state.name);

            RefreshPorts();
        }
        public StateView() { }

        public override bool IsRenamable()
        {
            return true;
        }
        public override bool IsSnappable()
        {
            return true;
        }
        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            var port = Port.Create<TransitionView>(orientation, direction, capacity, type);

            switch (direction)
            {
                case Direction.Input:
                    inputContainer.Add(port);
                    Input = port;
                    break;
                case Direction.Output:
                    outputContainer.Add(port);
                    Output = port;
                    break;
                default:
                    break;
            }

            port.portName = "";

            return port;
        }
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            State.position = newPos.position;
            EditorUtility.SetDirty(State);
        }
        public override void OnSelected()
        {
            if (State is null || State.IsDefault)
                return;

            Selection.activeObject = State;
        }

        public new class UxmlFactory : UxmlFactory<StateView> { }
    }
}
