//https://discussions.unity.com/t/ui-toolkit-text-best-fit/248895/
//This code is from the Unity discussions board and is used to autosize the font to fit the label
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelAutoFit : Label
{
  [UnityEngine.Scripting.Preserve]
  public new class UxmlFactory : UxmlFactory<LabelAutoFit, UxmlTraits> { }

  [UnityEngine.Scripting.Preserve]
  public new class UxmlTraits : Label.UxmlTraits
  {
    public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }

    public override void Init(VisualElement visualElement, IUxmlAttributes attributes, CreationContext creationContext)
    {
      base.Init(visualElement, attributes, creationContext);
    }
  }

  public LabelAutoFit()
  {
    RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
  }

  private void OnAttachToPanel(AttachToPanelEvent e)
  {
    UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
    RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
  }

  private void OnGeometryChanged(GeometryChangedEvent e)
  {
    UpdateFontSize();
  }

  private void UpdateFontSize()
  {
    UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    var previousWidthStyle = style.width;

    try
    {
      var width = resolvedStyle.width;

      // Set width to auto temporarily to get the actual width of the label
      style.width = StyleKeyword.Auto;
      var currentFontSize = MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

      var multiplier = resolvedStyle.width / Mathf.Max(currentFontSize.x, 1);
      var newFontSize = Mathf.RoundToInt(Mathf.Clamp(multiplier * currentFontSize.y, 1, resolvedStyle.height));

      if (Mathf.RoundToInt(currentFontSize.y) != newFontSize)
        style.fontSize = new StyleLength(new Length(newFontSize));
    }
    finally
    {
      style.width = previousWidthStyle;
      RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
  }
}