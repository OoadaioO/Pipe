using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xb.pipe;

public class MainUI : MonoBehaviour {
    [SerializeField] private GameObject gameOverDialog;
    [SerializeField] private Button newGameButton;
    [SerializeField] private AudioManager audioManager;

    private void Awake() {


#if UNITY_EDITOR
#else
    Application.targetFrameRate = 60;
#endif
    }

    private void OnEnable() {
        Player.OnDie += Player_OnDie;
        newGameButton.onClick.AddListener(NewGame);
    }

    private void OnDisable() {

        Player.OnDie -= Player_OnDie;
        newGameButton.onClick.RemoveListener(NewGame);
    }

    private void Player_OnDie() {
        Show();
        audioManager.FadeOut();
        audioManager.PlayGameOverOneShot();
    }

    private void Show() {
        gameOverDialog.SetActive(true);
    }

    private void Hide() {
        gameOverDialog.SetActive(false);
    }

    private void NewGame() {
        Player.Instance.StartGame(0);
        Hide();
        audioManager.FadeIn();
    }
}
