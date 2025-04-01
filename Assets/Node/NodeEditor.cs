using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeEditor : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Add Node"))
        {
            AddNode();
        }
        
        if (GUI.Button(new Rect(120, 10, 100, 30), "Connect Nodes"))
        {
            StartCoroutine(ConnectNodes());
        }
    }
    
    void AddNode()
    {
        GameObject nodeObj = new GameObject("Node " + nodes.Count);
        Node node = nodeObj.AddComponent<Node>();
        node.nodeName = "Node " + nodes.Count;
        node.nodeRect.position = new Vector2(100 + nodes.Count * 120, 100);
        node.transform.position = node.nodeRect.position;
        nodes.Add(node);
    }
    
    System.Collections.IEnumerator ConnectNodes()
    {
        if (nodes.Count < 2) yield break;
        
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            if (!nodes[i].connectedNodes.Contains(nodes[i+1]))
            {
                nodes[i].connectedNodes.Add(nodes[i+1]);
            }
            yield return null;
        }
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (nodes == null) return;
        
        foreach (var node in nodes)
        {
            if (node != null && node.connectedNodes != null)
            {
                foreach (var connectedNode in node.connectedNodes)
                {
                    if (connectedNode != null)
                    {
                        Node.DrawNodeCurve(node.nodeRect, connectedNode.nodeRect);
                    }
                }
            }
        }
    }
#endif
}


