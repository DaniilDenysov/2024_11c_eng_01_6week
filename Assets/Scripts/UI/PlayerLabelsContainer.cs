using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PlayerLabelsContainer : LabelContainer<PlayerLabel, PlayerLabelsContainer>
    {
        public override void Add(PlayerLabel item)
        {
            base.Add(item);
            //item.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        }

        public override PlayerLabelsContainer GetInstance()
        {
            return this;
        }
    }
}
