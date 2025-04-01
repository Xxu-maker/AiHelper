using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class NodeUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public string nodeName = "Node";
    public List<NodeUI> connectedNodes = new List<NodeUI>();
    public RectTransform rectTransform;
    public Image background;
    public Text titleText;
    
    [Header("Connector Settings")]
    public RectTransform inputConnector;
    public RectTransform outputConnector;
    
    private Vector2 dragOffset;
    private bool isDragging = false;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (titleText != null) titleText.text = nodeName;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out dragOffset);
            dragOffset = rectTransform.anchoredPosition - dragOffset;
            transform.SetAsLastSibling(); // 拖动时置于最前
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition))
            {
                rectTransform.anchoredPosition = localPointerPosition + dragOffset;
            }
        }
    }
    
    void OnDisable()
    {
        isDragging = false;
    }
    
    public Vector2 GetInputPosition()
    {
        return inputConnector ? inputConnector.position : rectTransform.position;
    }
    
    public Vector2 GetOutputPosition()
    {
        return outputConnector ? outputConnector.position : rectTransform.position;
    }
}

