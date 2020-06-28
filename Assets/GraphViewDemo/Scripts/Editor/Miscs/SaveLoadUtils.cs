using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public static class SaveLoadUtils
    {

        private static IEnumerable<BaseNode> ExtractNodes(EasyGraphView easyGraphView)
        {
            return easyGraphView.nodes.ToList().Where(n => n is BaseNode).Cast<BaseNode>();
        }

        private static IEnumerable<Edge> ExtractEdges(EasyGraphView easyGraphView)
        {
            return easyGraphView.edges.ToList();
        }

        private static IEnumerable<Group> ExtractGroups(EasyGraphView easyGraphView)
        {
            return easyGraphView.graphElements.ToList().Where(e => e is Group).Cast<Group>();
        }

        public static void SaveGraphView(EasyGraphView easyGraphView, string guid)
        {
            string assetPath;
            if (string.IsNullOrEmpty(guid))
            {
                string filePath = EditorUtility.SaveFilePanel("where to save", ".", "new EasyGraph", "asset");

                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                if (File.Exists(filePath))
                {
                    Debug.Log("will overwrite");
                }
                assetPath = FileUtil.GetProjectRelativePath(filePath);
            }
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                assetPath = path;
            }

            var newAsset = ScriptableObject.CreateInstance<EasyGraphAsset>();

            SaveGraphView(easyGraphView, newAsset);

            AssetDatabase.CreateAsset(newAsset, assetPath);
            AssetDatabase.SaveAssets();
        }

        private static void SaveGraphView(EasyGraphView easyGraphView, EasyGraphAsset easyGraphAsset)
        {
            SaveNodesAndEdges(easyGraphView, easyGraphAsset);
            SaveCommentGroups(easyGraphView, easyGraphAsset);
        }

        private static void SaveNodesAndEdges(EasyGraphView easyGraphView, EasyGraphAsset easyGraphAsset)
        {
            var edges = ExtractEdges(easyGraphView).Where(x => x.input.node != null && x.output.node != null).ToArray();
            for (var i = 0; i < edges.Count(); i++)
            {
                var outputNode = edges[i].output.node as BaseNode;
                var inputNode = edges[i].input.node as BaseNode;
                easyGraphAsset.edgeData.Add(new EdgeData
                {
                    sourcePort = edges[i].output.portName,
                    targetPort = edges[i].input.portName,
                    sourceNodeGUID = outputNode.GUID,
                    targetNodeGUID = inputNode.GUID,
                });
            }

            foreach (var node in ExtractNodes(easyGraphView))
            {
                easyGraphAsset.commonNodeData.Add(new CommonNodeData
                {
                    nodeGUID = node.GUID,
                    nodeTypeId = node.GetType().AssemblyQualifiedName,
                    serializedData = node.Serialize(),
                    position = node.GetPosition().position
                });
            }
        }

        private static void SaveCommentGroups(EasyGraphView easyGraphView, EasyGraphAsset easyGraphAsset)
        {
            var groups = ExtractGroups(easyGraphView);
            foreach (var group in groups)
            {
                var nodeGuidList = group.containedElements.Where(x => x is BaseNode).Cast<BaseNode>().Select(x => x.GUID).ToList();
                easyGraphAsset.commentGroupData.Add(new CommentGroupData
                {
                    managedNodes = nodeGuidList,
                    title = group.title,
                    position = group.GetPosition().position
                });
            }
        }

        public static void LoadGraphViewAsset(EasyGraphAsset easyGraphAsset, EasyGraphView easyGraphView)
        {
            if (easyGraphAsset == null)
            {
                Debug.LogError("failed to load");
                return;
            }

            ClearGraph(easyGraphView);
            LoadNodes(easyGraphAsset, easyGraphView);
            LoadEdges(easyGraphAsset, easyGraphView);
            LoadCommentGroups(easyGraphAsset, easyGraphView);
        }

        public static void LoadGraphViewAsset(EasyGraphView easyGraphView)
        {
            string filePath = EditorUtility.OpenFilePanel("where to load?", ".", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                Debug.LogError("not exist");
                return;
            }

            string assetPath = FileUtil.GetProjectRelativePath(filePath);
            var easyGraphAsset = AssetDatabase.LoadAssetAtPath<EasyGraphAsset>(assetPath);

            LoadGraphViewAsset(easyGraphAsset, easyGraphView);
        }

        private static void ClearGraph(EasyGraphView easyGraphView)
        {
            var nodes = ExtractNodes(easyGraphView);
            foreach (var node in nodes)
            {
                var edges = ExtractEdges(easyGraphView);
                edges.Where(x => x.input.node == node || x.output.node == node).ToList()
                    .ForEach(edge => easyGraphView.RemoveElement(edge));
                easyGraphView.RemoveElement(node);
            }
        }

        private static void LoadNodes(EasyGraphAsset easyGraphAsset, EasyGraphView easyGraphView)
        {
            foreach (var nodeData in easyGraphAsset.commonNodeData)
            {
                easyGraphView.LoadAndAddNode(nodeData);
            }
        }

        private static Dictionary<string, BaseNode> cache = new Dictionary<string, BaseNode>();
        private static void LoadEdges(EasyGraphAsset easyGraphAsset, EasyGraphView easyGraphView)
        {
            cache.Clear();

            foreach (var node in ExtractNodes(easyGraphView))
            {
                cache.Add(node.GUID, node);
            }

            foreach (var e in easyGraphAsset.edgeData)
            {
                var sourceNode = cache[e.sourceNodeGUID];
                var targetNode = cache[e.targetNodeGUID];

                var sourcePort = sourceNode.outputContainer.Q<Port>(e.sourcePort);
                var targetPort = targetNode.inputContainer.Q<Port>(e.targetPort);

                var edge = MakeEdge(sourcePort, targetPort);
                easyGraphView.Add(edge);
            }

            cache.Clear();

        }

        private static Edge MakeEdge(Port output, Port input)
        {
            var edge = new Edge();
            edge.output = output;
            edge.input = input;
            edge.input.Connect(edge);
            edge.output.Connect(edge);

            return edge;
        }

        private static void LoadCommentGroups(EasyGraphAsset easyGraphAsset, EasyGraphView easyGraphView)
        {
            foreach (var groupData in easyGraphAsset.commentGroupData)
            {
                var group = easyGraphView.CreateCommentGroup(
                    new Rect(groupData.position,  EasyGraphView.CommentGroupSize),
                    groupData);

                group.AddElements(ExtractNodes(easyGraphView).Where(x => groupData.managedNodes.Contains(x.GUID)));
            }
        }
    }
}
