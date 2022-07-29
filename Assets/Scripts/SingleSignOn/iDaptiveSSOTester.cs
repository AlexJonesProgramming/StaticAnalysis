using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using UnityEngine;

namespace i5.Toolkit.Core.OpenIDConnectClient
{
    public class iDaptiveSSOTester : MonoBehaviour
    {
        private bool isSubscribedToOidc = false;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ServiceManager.GetService<OpenIDConnectService>().RedirectURI = "fselifestyles://";

                // only subscribe to the event if it was not yet done before, e.g. in a failed login attempt
                if (!isSubscribedToOidc)
                {
                    ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += OpenIDConnectTester_LoginCompleted;
                    isSubscribedToOidc = true;
                }
                ServiceManager.GetService<OpenIDConnectService>().OpenLoginPage();


            }
        }

        public void ButtonPressed()
        {

            ServiceManager.GetService<OpenIDConnectService>().RedirectURI = "fselifestyles://";

            // only subscribe to the event if it was not yet done before, e.g. in a failed login attempt
            if (!isSubscribedToOidc)
            {
                ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += OpenIDConnectTester_LoginCompleted;
                isSubscribedToOidc = true;
            }
            ServiceManager.GetService<OpenIDConnectService>().OpenLoginPage();
        }

        public void DebugLogRedirect()
        {
            Debug.Log(ServiceManager.GetService<OpenIDConnectService>().RedirectURI);
        }

        private async void OpenIDConnectTester_LoginCompleted(object sender, System.EventArgs e)
        {
            Debug.Log("Login completed", this);
            Debug.Log(ServiceManager.GetService<OpenIDConnectService>().AccessToken, this);
            ServiceManager.GetService<OpenIDConnectService>().LoginCompleted -= OpenIDConnectTester_LoginCompleted;
            isSubscribedToOidc = false;

            IUserInfo userInfo = await ServiceManager.GetService<OpenIDConnectService>().GetUserDataAsync();
            Debug.Log("Currently logged in user: " + userInfo.FullName
                + " (username: " + userInfo.Username + ") with the mail address " + userInfo.Email, this);
        }
    }
}
