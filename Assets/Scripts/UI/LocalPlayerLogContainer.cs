using System.Collections;
using System.Collections.Generic;
using UI;
using UI.PlayerLog;
using UnityEngine;

public class LocalPlayerLogContainer : LabelContainer<LocalPlayerLogLabel, LocalPlayerLogContainer>
{
    public override LocalPlayerLogContainer GetInstance()
    {
        return this;
    }
}
