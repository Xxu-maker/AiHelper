using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class Node : MonoBehaviour
{
    public string nodeName = "Node";
    public List<Node> connectedNodes = new List<Node>();
    public Rect nodeRect = new Rect(0, 0, 100, 50);
    
    private bool isDragging = false;
    private Vector2 dragOffset;
    
    void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            // 绘制连线
            foreach (var connectedNode in connectedNodes)
            {
                if (connectedNode != null)
                {
                    DrawNodeCurve(nodeRect, connectedNode.nodeRect);
                }
            }
        }
    }
    
    void OnMouseDown()
    {
        isDragging = true;
        dragOffset = (Vector2)transform.position - GetMouseWorldPos();
    }
    
    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + dragOffset;
            nodeRect.position = transform.position;
        }
    }
    
    void OnMouseUp()
    {
        isDragging = false;
    }
    
    private Vector2 GetMouseWorldPos()
    {
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = Screen.height - mousePos.y; // 转换为世界坐标
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    
    public static void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height/2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height/2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        
        //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 3);
    }
}
