using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class ImageDisplay : ObjectHoverDisplay {

        public Image output;
        public Sprite emptyImage;
        public Color emptyColor;
        public Color itemColor = Color.white;

        protected void Awake() {
            Validate();
        }

        protected override void Start() {
            base.Start();
        }

        public override void Validate() {
            Spriteable source = (Spriteable)ToDisplay;
            if(source == null) {
                Empty(); 
            } else {
                string spriteName = source.SpriteName();
                if(spriteName == null) {
                    Empty();
                } else {
                    output.color = itemColor;
                    output.sprite = Sprites.Get(spriteName);
                }
            }
        }

        public void Empty() {
            output.sprite = emptyImage;
            output.color = emptyColor;
        }
    }
}