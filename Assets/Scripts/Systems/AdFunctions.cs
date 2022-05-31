using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdFunctions : MonoBehaviour, IUnityAdsListener
{
    //id de development do jogo
    private string gameId = "4747893";
    private bool test_mode = true;

    //botão usado
    [SerializeField]
    private Button[] ad_btn;
    
    //id da função de placement do dashboard
    public string placementId = "teste";

    private void Start()
    {
        /*
        #if UNITY_EDITOR
        test_mode = true;
        
        //inicializa os ads
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, test_mode);

        if (ad_btn != null)
        {
            foreach(Button btn in ad_btn)
            {
                //para só dar para interagir com o botão se tiverem anúncios
                //btn.interactable = Advertisement.IsReady(placementId);

                btn.onClick.AddListener(RewardedAd);
            }
        }
        else print("ad_btn is null");
        #endif
        #if !UNITY_EDITOR
        if (ad_btn != null)
        {
            foreach(Button btn in ad_btn)
            {
                //para só dar para interagir com o botão se tiverem anúncios
                //btn.interactable = Advertisement.IsReady(placementId);

                btn.onClick.AddListener(RewardedAd);
            }
        }
        #endif
        */
        #if UNITY_EDITOR
        test_mode = true;
        #endif

        //inicializa os ads
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, test_mode);

        if (ad_btn != null)
        {
            foreach(Button btn in ad_btn)
            {
                //para só dar para interagir com o botão se tiverem anúncios
                //btn.interactable = Advertisement.IsReady(placementId);

                btn.onClick.AddListener(RewardedAd);
            }
        }
        else print("ad_btn is null");
    }

    //inicia o ad
    private void RewardedAd()
    {
        /*
        #if UNITY_EDITOR
        Advertisement.Show(placementId);
        #endif
        #if !UNITY_EDITOR
        Rewards();
        #endif
        */
        Advertisement.Show(placementId);
    }

    //recompensa do jogador por ver o anuncio
    private void Rewards()
    {
        PlayerEquipment.Instance.potions = PlayerEquipment.Instance.max_potions;
        PlayerHealth.Instance.pot_txt.text = "" + PlayerEquipment.Instance.potions;
    }

    public void OnUnityAdsReady(string p_id)
    {
        if(p_id == placementId)
        {
            foreach (Button btn in ad_btn)
            {
                btn.interactable = true;
            }
        }
    }

    public void OnUnityAdsDidFinish(string p_id, ShowResult showR)
    {
        if(showR == ShowResult.Finished)
        {
            //da a recompença
            Rewards();
        }
        else if(showR == ShowResult.Skipped)
        {

        }
        else if(showR == ShowResult.Failed)
        {
            print("Ad didn't finish due to an error");
        }
    }

    public void OnUnityAdsDidError(string msg)
    {
        //faz o log do erro
    }

    public void OnUnityAdsDidStart(string p_id)
    {
        //
    }
}
