using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SceneController1 : MonoBehaviour
{
    public TMP_InputField feedback;
    
    string URL ="https://docs.google.com/forms/u/0/d/e/1FAIpQLSeJ7TUHl7_AHqF4twH76VywrR0vr4V7uz7EPRTPr5IHEeJzkQ/formResponse";

    public GameObject UserGuidePanel, MainMenuPanel , FeedBackPanel;

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ARScene(){
        SceneManager.LoadScene("ARScene");
    }

    public void QuitApp(){
        Application.Quit();
    }

    public void UserGuidePanelOpen(){
        UserGuidePanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        FeedBackPanel.SetActive(false);
    }

    public void MainMenuPanelOpen(){
        FeedBackPanel.SetActive(false);
        UserGuidePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void FeedBackPanelOpen(){
        FeedBackPanel.SetActive(true);
        UserGuidePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }

    public void SendFeedBack(){
        StartCoroutine(Post(feedback.text));
    }

    IEnumerator Post(string s1){
        WWWForm form = new WWWForm();
        form.AddField("entry.81145259",s1);

        UnityWebRequest www= UnityWebRequest.Post(URL,form);
        yield return www.SendWebRequest();
    }
}
