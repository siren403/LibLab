using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DisabledControl : Control<Button, bool>
{
    protected override void OnValueChanged(Button source, bool value)
    {
        Debug.Log($"DisabledControl.OnValueChanged({source}, {value})");
        source.interactable = !value;
    }
}

public class PrimaryButton : MonoBehaviour
{
    [SerializeField]
    private DisabledControl disabled;

    [SerializeField]
    private SourceSelector<Button> button;

    [SerializeField]
    private SourceSelector<TextMeshProUGUI> label;

    private void Awake()
    {
    }
}
