using System;
using UnityEngine;
using xb.input;

namespace xb.pipe {

    public class Player : SingletonMonobehaviour<Player> {

        public static Action OnDie;
        public static Action<float> OnDistanceTraveledChange;

        public PipeSystem pipeSystem;

        public float startVelocity;
        public float[] accelerations;

        public float rotationVelocity;


        private float acceleration, velocify;


        private Pipe currentPipe;
        private float distanceTraveled;

        private float deltaToRotation;
        private float systemRotation; // 度

        private Transform world, rotater;
        private float worldRotation, avatarRotation;



        private void Start() {
            world = pipeSystem.transform.parent;
            rotater = transform.GetChild(0);

            gameObject.SetActive(false);
        }


        private void Update() {

            velocify += acceleration * Time.deltaTime;

            float delta = velocify * Time.deltaTime;
            distanceTraveled += delta;
            NotifyDistanceTraveled();

            systemRotation += delta * deltaToRotation;

            if (systemRotation >= currentPipe.CurveAngle) {

                // 剩余旋转，转换成弧度
                delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
                currentPipe = pipeSystem.SetupNextPipe();
                // 新的 旋转角度/单位弧度
                SetupCurrentPipe();
                systemRotation = delta * deltaToRotation;
            }

            pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);

            UpdateAvatarRotation();
        }

        private void UpdateAvatarRotation() {
            avatarRotation += rotationVelocity * Time.deltaTime * PipeUserInput.Instance.frameInput.horizontal;
            if (avatarRotation < 0f) {
                avatarRotation += 360f;
            } else if (avatarRotation >= 360f) {
                avatarRotation -= 360f;
            }
            rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
        }

        private void SetupCurrentPipe() {
            deltaToRotation = 360 / (2f * Mathf.PI * currentPipe.CurveRadius);

            // 补偿当前管道的相对旋转
            worldRotation += currentPipe.RelativeRotation;
            if (worldRotation < 0f) {
                worldRotation += 360f;
            } else if (worldRotation >= 360f) {
                worldRotation -= 360f;
            }
            world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
        }

        public void Die() {

            int best = (int)distanceTraveled;
            if (best > UserRepository.Instance.BestScore) {
                UserRepository.Instance.BestScore = best;
            }


            gameObject.SetActive(false);
            OnDie?.Invoke();
        }


        public void StartGame(int mode) {
            distanceTraveled = 0f;
            NotifyDistanceTraveled();

            avatarRotation = 0f;
            systemRotation = 0f;
            worldRotation = 0f;
            velocify = startVelocity;
            acceleration = accelerations[mode];
            currentPipe = pipeSystem.SetupFirstPipe();
            SetupCurrentPipe();
            gameObject.SetActive(true);
        }

        private void NotifyDistanceTraveled() {
            OnDistanceTraveledChange?.Invoke(distanceTraveled);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
#endif
    }

}