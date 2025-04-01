using UnityEngine;
using UnityEngine.UI;

public class NodeEditorUIController : MonoBehaviour
{
    public NodeEditorUI nodeEditor;
    public Button addNodeButton;
    public Button deleteNodeButton;
    
    void Start()
    {
        if (addNodeButton != null)
        {
            addNodeButton.onClick.AddListener(nodeEditor.AddNode);
        }
        
        if (deleteNodeButton != null)
        {
            deleteNodeButton.onClick.AddListener(nodeEditor.DeleteSelectedNode);
        }
    }
}

