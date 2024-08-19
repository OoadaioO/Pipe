using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xb.input {
    public class PipeUserInput : SingletonMonobehaviour<PipeUserInput> {

        public FrameInput frameInput;

        private float halfScreenWidth;

        protected override void Awake() {
            base.Awake();
            halfScreenWidth = Screen.width * 0.5f;
        }

        // Update is called once per frame
        private void Update() {
            frameInput.horizontal = 0;

            if (Input.GetMouseButton(0)) {

                if (Input.mousePosition.x < halfScreenWidth) {
                    frameInput.horizontal = -1;
                } else {
                    frameInput.horizontal = 1;
                }

            } else {
                frameInput.horizontal = Input.GetAxis("Horizontal");
            }

        }



    }

    public struct FrameInput {
        public float horizontal;
    }

}
