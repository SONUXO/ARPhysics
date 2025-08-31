using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{

    public TMP_Text emailVerificationText,PasswordRestLinkSent;
    public static AuthManager Instance;

    public GameObject LoginPanel, SignUpPanel, emailVerificationPanel, ForgetPasswordPanel, ForgetPasswordConfrimationPanel;

    public  TMP_InputField LoginEmail, LoginPassword ,UserName, SignUpEmail, SignUpPassword,SignUpConfrimPassword;

    private void Awake(){
        CreatInstance();
    }

    private void CreatInstance(){
        if(Instance==null){
            Instance=this;
        }
    }
    public void OpenLoginPanel(){
        LoginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        ForgetPasswordPanel.SetActive(false);
        ForgetPasswordConfrimationPanel.SetActive(false);
    }
    public void OpenSignUpPanel(){
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(true);
        emailVerificationPanel.SetActive(false);
        ForgetPasswordPanel.SetActive(false);
        ForgetPasswordConfrimationPanel.SetActive(false);
    }

    public void OpenForgetPasswordPanel(){
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        ForgetPasswordPanel.SetActive(true);
        ForgetPasswordConfrimationPanel.SetActive(false);
    }


    public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage){
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        emailVerificationPanel.SetActive(true);
        ForgetPasswordPanel.SetActive(false);
        ForgetPasswordConfrimationPanel.SetActive(false);
        if(isEmailSent){
            emailVerificationText.text = $"Please verify your email address \n verfification email has been sent to {emailId}";
        }
        else{
            emailVerificationText.text= $"Couldn't sent email: {errorMessage}";
        }
    }

    public void ShowResetLinkSent(bool isEmailSent, string emailId, string errorMessage){
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        ForgetPasswordPanel.SetActive(false);
        ForgetPasswordConfrimationPanel.SetActive(true);
        if(isEmailSent){
            PasswordRestLinkSent.text = $"Password Reset link has been\n sent to {emailId}";
        }
        else{
            PasswordRestLinkSent.text= $"Couldn't sent email: {errorMessage}";
        }
    }

}
