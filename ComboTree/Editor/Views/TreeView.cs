using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;

namespace ComboTree.Editor
{
    public class TreeView : GraphView
    {
        ComboTreeController comboTree;

        public TreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            graphViewChanged += OnGraphViewChanged;
        }

        public void BindTree(ComboTreeController comboTree)
        {
            this.comboTree = comboTree;

            graphViewChanged -= OnGraphViewChanged;

            foreach (var state in comboTree.States)
            {
                OnAddState(state);
            }

            var nodes = this.nodes.ToList();

            foreach (var node in nodes)
            {
                if (node is StateView stateView && stateView.State.transitions is List<SerializedTransition> transitions)
                {
                    foreach (var transition in transitions)
                    {
                        var targetNode = (StateView)nodes.Find(node =>
                        {
                            if (node is StateView state)
                                return state.State == transition.Target;

                            return false;
                        });

                        if (targetNode is null)
                            continue;

                        if (targetNode.Input is Port to)
                        {
                            var edge = stateView.Output.ConnectTo<TransitionView>(to);
                            edge.transition = transition;
                            AddElement(edge);
                        }
                    }
                }
            }


            graphViewChanged += OnGraphViewChanged;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if(!selection.Exists(selected => selected is StateView stateView && stateView.State.IsDefault) && selection.Count > 0)
            {
                evt.menu.AppendAction("Delete", action => DeleteSelection());
                evt.menu.AppendSeparator();
            }

            evt.menu.AppendAction("New State", action => AddNewState("New AnimationState", action.eventInfo.mousePosition));
            evt.menu.AppendAction("New Exit", action => AddNewState(serializedState.ExitName, action.eventInfo.mousePosition));
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();
            ports.ForEach(port =>
            {
                if (port.direction != startPort.direction && port.node != startPort.node && !startPort.connections.Any(edge => edge.input == port))
                    result.Add(port);
            });
            return result;
        }

        void AddNewState(string name, Vector2 position)
        {
            var state = comboTree.AddState(name);
            OnAddState(state).SetPosition(new Rect(position, Vector2.zero));
        }
        StateView OnAddState(serializedState state)
        {
            var stateView = new StateView(state);
            AddElement(stateView);

            return stateView;
        }
        void OnRemoveState(StateView stateView)
        {
            if(stateView.State is serializedState scriptableState)
                comboTree.RemoveState(scriptableState);
        }
        void OnAddEdge(Edge edge)
        {
            if (edge.input.node is StateView input && edge.output.node is StateView output)
            {
                var transition = new SerializedTransition(output.State, input.State);
                if (edge is TransitionView edgeView)
                    edgeView.transition = transition;

                EditorUtility.SetDirty(input.State);
            }
        }
        void OnRemoveEdge(Edge edge)
        {
            if (edge is TransitionView edgeView && edgeView.transition is SerializedTransition transition && transition.Owner is serializedState owner)
            {
                owner.transitions.Remove(edgeView.transition);
                EditorUtility.SetDirty(owner);
            }
        }
        GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if(change.edgesToCreate is List<Edge> edges)
                foreach (var edge in edges)
                {
                    OnAddEdge(edge);
                }
            if(change.elementsToRemove is List<GraphElement> elements)
                foreach (var element in elements)
                {
                    if (element is Edge edge)
                        OnRemoveEdge(edge);

                    if (element is StateView stateView)
                        OnRemoveState(stateView);
                }

            return change;
        }

        public new class UxmlFactory : UxmlFactory<TreeView, TreeView.UxmlTraits> { }
    }
}
