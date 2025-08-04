using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace xb.pipe {
    public class BestScoreUI : MonoBehaviour {
        [SerializeField] private GameObject bestRoot;
        [SerializeField] private TextMeshProUGUI bestText;

        private void Awake() {

            SetBestText(UserRepository.Instance.BestScore);
            UserRepository.OnBestScoreChanged += SetBestText;
        }
        private void OnDestroy() {
            UserRepository.OnBestScoreChanged -= SetBestText;
        }

        public void SetBestText(int bestScore){
            bestRoot.SetActive(bestScore > 0);
            bestText.text = $"{bestScore}M";
        }

    }
}
