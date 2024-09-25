using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textHightScore;
    [SerializeField] private Button backHome;


    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        backHome.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            Show();
            if (DeliveryManager.Instance.GetSuccessfulRecipesAmount() > DataManager.Instance.LoadHighScore())
            {
                textHightScore.gameObject.SetActive(true);
                textScore.gameObject.SetActive(false);
            }
            else
            {
                textHightScore.gameObject.SetActive(false);
                textScore.gameObject.SetActive(true);
            }
            DeliveryManager.Instance.SaveHightScore();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
            
           
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}