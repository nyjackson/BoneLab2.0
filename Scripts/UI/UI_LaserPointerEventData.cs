using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class UI_LaserPointerEventData : PointerEventData
    {
        public GameObject current;
		public UI_LaserPointer controller;
        public UI_LaserPointerEventData(EventSystem e) : base(e) { }

        public override void Reset()
        {
            current = null;
            controller = null;
            base.Reset();
        }
    }
}

