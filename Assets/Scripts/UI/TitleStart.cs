using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;
using UnityEngine.SceneManagement;
using TMPro;
using TagGame.Photon;

using UnityEditor;

public class TitleStart : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Text ConnectionStateText;

    [SerializeField]
    private Image FadeImage;

    [SerializeField]
    private float fadeRuntime;

    [SerializeField]
    private Gradient FadeGradient;

    [SerializeField]
    private string nextScene;

    private CoroutineWrapper wrapper;

    private void Awake()
    {
        wrapper = new CoroutineWrapper(this);

        LocalPlayerData.Instance.Initialize();
        RemotePlayerData.Instance.Initialize();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (wrapper.IsPlaying)
            return;

        wrapper.StartSingleton(StartFadeAndMoveToGameScene());
    }

    IEnumerator StartFadeAndMoveToGameScene()
    {
        PhotonManager.Instance.StartConnect();

        ConnectionStateText.text = "만만한 상대를 기다리는중";

        yield return new WaitUntil(() => PhotonManager.Instance.ConnectedPlayerCount.CurrentData == 2);

        SetupCharacterIndex();

        yield return YieldInstructionCache.WaitForSeconds(1f);

        ConnectionStateText.text = "시작";

        float t = 0;
        while (t < fadeRuntime)
        {
            FadeImage.color = FadeGradient.Evaluate(t / fadeRuntime);
            t += Time.deltaTime;
            yield return null;
        }

        FadeImage.color = FadeGradient.Evaluate(1);

        SceneManager.LoadScene(nextScene);
    }

    private void SetupCharacterIndex()
    {
        if (!PhotonManager.Instance.isMasterClient)
            return;

        LocalPlayerData.Instance.characterIndex.CurrentData = Random.Range(0, 2);
        RemotePlayerData.Instance.characterIndex.CurrentData = LocalPlayerData.Instance.characterIndex.CurrentData == 1 ? 0 : 1;

        Debug.Log($"remote : {RemotePlayerData.Instance.characterIndex.CurrentData}, local : {LocalPlayerData.Instance.characterIndex.CurrentData}");

        PhotonManager.SendChacterInitializePacketData(new ChacterInitializePacket()
        {
            index = LocalPlayerData.Instance.characterIndex.CurrentData
        });
    }


    private void OnDestroy()
    {
    }
}