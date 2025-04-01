using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NodeConnection : MonoBehaviour
{
    public NodeUI fromNode;
    public NodeUI toNode;
    
    private Image lineImage;
    private RectTransform rectTransform;
    
    void Awake()
    {
        lineImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        if (fromNode && toNode)
        {
            UpdateLine();
        }
    }
    
    public void UpdateLine()
    {
        Vector2 startPos = fromNode.GetOutputPosition();
        Vector2 endPos = toNode.GetInputPosition();
        
        rectTransform.position = (startPos + endPos) / 2f;
        Vector2 direction = (endPos - startPos).normalized;
        rectTransform.sizeDelta = new Vector2(Vector2.Distance(startPos, endPos), 5f);
        rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }
    
    public static NodeConnection CreateConnection(Transform parent, NodeUI from, NodeUI to, Color color)
    {
        GameObject connectionObj = new GameObject("Connection");
        connectionObj.transform.SetParent(parent, false);
        
        NodeConnection connection = connectionObj.AddComponent<NodeConnection>();
        connection.fromNode = from;
        connection.toNode = to;
        
        Image img = connectionObj.GetComponent<Image>();
        img.color = color;
        
        return connection;
    }
}

