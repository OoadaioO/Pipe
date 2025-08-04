using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace xb.pipe {
    public class DistanceUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI distanceText;



        private void Awake() {
            Player.OnDistanceTraveledChange += UpdateDistanceText;
            UpdateDistanceText(0);
        }
        private void OnDestroy() {
            Player.OnDistanceTraveledChange -= UpdateDistanceText;
        }



        private void UpdateDistanceText(float distance) {
            distanceText.text = $"{(int)distance}m";
        }

    }
}
