using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpUI : MonoBehaviour
{
    public static HelpUI Instance;
    public UIDocument document;
    private VisualElement background;
    private Button closeButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        background = document.rootVisualElement.Q<VisualElement>("Background");
        closeButton = document.rootVisualElement.Q<Button>("CloseButton");
        closeButton.RegisterCallback<ClickEvent>(CloseButton);
        background.visible = false;
    }

    public void OpenHelp(ClickEvent evt)
    {
        background.visible = true;
    }

    public void CloseButton(ClickEvent evt)
    {
        background.visible = false;
    }
}
