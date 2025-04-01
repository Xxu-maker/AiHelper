using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeEditorUI : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public RectTransform nodesContainer;
    public RectTransform connectionsContainer;
    public GameObject nodePrefab;
    
    [Header("Settings")]
    public Color connectionColor = Color.white;
    public Vector2 newNodeOffset = new Vector2(50, 50);
    
    private List<NodeUI> nodes = new List<NodeUI>();
    private List<NodeConnection> connections = new List<NodeConnection>();
    private NodeUI selectedNode;
    
    void Start()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Node prefab is not assigned!");
            enabled = false;
        }
    }
    
    public void AddNode()
    {
        GameObject newNodeObj = Instantiate(nodePrefab, nodesContainer);
        NodeUI newNode = newNodeObj.GetComponent<NodeUI>();
        
        if (nodes.Count > 0)
        {
            RectTransform lastNode = nodes[nodes.Count - 1].rectTransform;
            newNode.rectTransform.anchoredPosition = lastNode.anchoredPosition + newNodeOffset;
        }
        else
        {
            newNode.rectTransform.anchoredPosition = Vector2.zero;
        }
        
        newNode.nodeName = "Node " + nodes.Count;
        if (newNode.titleText != null) newNode.titleText.text = newNode.nodeName;
        
        nodes.Add(newNode);
    }
    
    public void SelectNode(NodeUI node)
    {
        selectedNode = node;
    }
    
    public void ConnectToSelected(NodeUI targetNode)
    {
        if (selectedNode != null && selectedNode != targetNode)
        {
            if (!selectedNode.connectedNodes.Contains(targetNode))
            {
                selectedNode.connectedNodes.Add(targetNode);
                
                // 创建可视化连线
                NodeConnection connection = NodeConnection.CreateConnection(
                    connectionsContainer,
                    selectedNode,
                    targetNode,
                    connectionColor);
                
                connections.Add(connection);
            }
        }
    }
    
    public void DeleteSelectedNode()
    {
        if (selectedNode != null)
        {
            // 删除所有相关连线
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i].fromNode == selectedNode || 
                    connections[i].toNode == selectedNode)
                {
                    Destroy(connections[i].gameObject);
                    connections.RemoveAt(i);
                }
            }
            
            // 从其他节点的连接列表中移除
            foreach (var node in nodes)
            {
                node.connectedNodes.Remove(selectedNode);
            }
            
            nodes.Remove(selectedNode);
            Destroy(selectedNode.gameObject);
            selectedNode = null;
        }
    }
}

