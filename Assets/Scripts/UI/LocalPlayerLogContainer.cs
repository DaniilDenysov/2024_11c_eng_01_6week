using System.Collections;
using System.Collections.Generic;
using UI;
using UI.PlayerLog;
using UnityEngine;

public class LocalPlayerLogContainer : LabelContainer<LocalPlayerLogLabel, LocalPlayerLogContainer>
{
    [SerializeField] private LocalPlayerLogLabel message;

    public void AddLogMessage(string messageText)
    {
        var label = Instantiate(message).GetComponent<LocalPlayerLogLabel>();
        label.Construct(messageText);
        Add(label);
    }

    public override LocalPlayerLogContainer GetInstance()
    {
        return this;
    }
}
