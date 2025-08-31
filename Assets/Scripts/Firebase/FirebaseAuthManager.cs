using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;


    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;


    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;


    [Header("ResetPassword")]
    public TMP_InputField UserEmailField;
    public TMP_Text notificationText;

    void Awake()
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {

                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
  
        auth = FirebaseAuth.DefaultInstance;
    }

    // public void Logout(){
    //     if(auth !=null && User != null){
    //         auth.SignOut();
    //         AuthManager.Instance.OpenLoginPanel();
    //         emailLoginField.text= "";
    //         passwordLoginField.text="";
    //     }
    // }

    public void LoginButton()
    {

        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void ResetButton(){
        if(UserEmailField.text==""){
            notificationText.text="empty field! please enter your email address";
        }
        else{
            StartCoroutine(PasswordReset(UserEmailField.text));
            notificationText.text="email has been sent!";
        }
    }

    private IEnumerator Login(string _email, string _password)
    {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {

            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            if(User.IsEmailVerified){
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");          
            }
            else{
                SendEmailForVarification();
            }

        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
        }
        else 
        {
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {

                User = RegisterTask.Result.User;

                if (User != null)
                {
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        if(User.IsEmailVerified){
                        AuthManager.Instance.OpenLoginPanel();
                        warningRegisterText.text = "";                            
                        }
                        else{
                            SendEmailForVarification();
                        }

                    }
                }
            }
        }
    }

    public void SendEmailForVarification(){
        StartCoroutine(SendEmailForVarificationAsync());
    }

    private IEnumerator SendEmailForVarificationAsync(){
        if(User !=null){
            var sendEmailTask  = User.SendEmailVerificationAsync();

            yield return new WaitUntil(() => sendEmailTask.IsCompleted);
            if(sendEmailTask.Exception!=null){
                FirebaseException firebaseException= sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error =(AuthError)firebaseException.ErrorCode;

                string errorMessage ="Unknown Error: please try again later";

                switch(error){
                    case AuthError.Cancelled:
                        errorMessage="Email Varification was cancelled";
                        break;
                    case AuthError.TooManyRequests: 
                        errorMessage="Too many Requests";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        errorMessage="The email you entered is invalid";
                        break;
                }

                AuthManager.Instance.ShowVerificationResponse(false,User.Email,errorMessage);

            }
            else{
                Debug.Log("Emai has successfully sent");
                AuthManager.Instance.ShowVerificationResponse(true,User.Email,null);
            }
        }
    }

    private IEnumerator PasswordReset(string UserEmail){
        auth.SendPasswordResetEmailAsync(UserEmail).ContinueWith(task => {
        if (task.IsCanceled) {
            Debug.LogError("SendPasswordResetEmailAsync was canceled.");
            return;
        }
        if (task.IsFaulted) {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
            return;
        }
        else{
        Debug.Log("Password reset email sent successfully.");           
        AuthManager.Instance.ShowResetLinkSent(true,UserEmail,null);        
        }
    }
    );
    yield return null;
    }
}