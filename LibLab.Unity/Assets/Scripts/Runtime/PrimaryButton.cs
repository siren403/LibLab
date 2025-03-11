using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DisabledControl : Control<Button, bool>
{
    protected override ChangeResult OnValueChanged(Button source, bool value)
    {
        bool toValue = !value;
        if (source.interactable == toValue)
        {
            return ChangeResult.NotDirty;
        }
        source.interactable = toValue;
        return ChangeResult.Success;
    }
}

[Serializable]
public class LabelControl : Control<TextMeshProUGUI, string>
{

    protected override ChangeResult OnValueChanged(TextMeshProUGUI source, string value)
    {
        if (source.text == value)
        {
            return ChangeResult.NotDirty;
        }
        source.text = value;
        return ChangeResult.Success;
    }
}

public class PrimaryButton : MonoBehaviour
{
    [SerializeField]
    private DisabledControl disabled;

    [SerializeField]
    private LabelControl label;

    [SerializeField]
    private SourceSelector<TextMeshProUGUI> text;

    private void Awake()
    {
    }
}
