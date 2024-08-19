using UnityEngine;

namespace xb.pipe.player {
    public class Avatar : MonoBehaviour {

        [SerializeField]
        private ParticleSystem shape, trial, burst;


        [SerializeField]
        private GameObject[] activeObjs;

        [SerializeField]
        private Player player;

        [SerializeField] 
        private float deathCountdown = -1f;

   

        private void OnTriggerEnter(Collider other) {

            if(deathCountdown < 0f){

                SetPlayerAvatarEnable(false);

                burst.Emit(burst.main.maxParticles);
                var main = burst.main;
                deathCountdown = main.startLifetime.constant;
            }
        }

        private void SetPlayerAvatarEnable(bool enable){
            foreach(GameObject obj in activeObjs){
                obj.SetActive(enable);
            }
        }


        private void Update() {
            if(deathCountdown >= 0f){
                deathCountdown -= Time.deltaTime;
                if(deathCountdown <=0f){
                    deathCountdown = -1f;
                    SetPlayerAvatarEnable(true);
                    
                    player.Die();
                }
            }
        }

    }



}
