using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// Allows us to open an in app version of a safari browser.
/// </summary>
public static class SafariView
{

    #if UNITY_IOS
    [DllImport("__Internal")]

    extern static void launchUrl(string url);
    #endif

    public static void LaunchURL(string url)
    {
        #if UNITY_EDITOR
        Application.OpenURL(url);
        #elif UNITY_IOS
        launchUrl(url);
        #endif
    }

    /*
    
    In Assets/Plugins/IOS there needs to be a file named "SafariView.mm".
    While inspecting this plugin in the editor drop down the section titled "Rarely used Frameworks" and check "SafariSevices".
    
    The contents of this file are ===========================================================================

    #import <SafariServices/SafariServices.h>
 
    extern UIViewController* UnityGetGLViewController();

    extern "C"
    {

        void launchUrl(const char *url)

        {

            //Unity is obtain an instance of ViewController being displayed now 

            UIViewController *uvc = UnityGetGLViewController();


            //C # based on the C string passed from the side, it generates a NSURL object 

            NSURL *URL = [NSURL URLWithString:[[NSString alloc] initWithUTF8String:url]];


            //from the generated URL, generate a SFSafariViewController object 

            SFSafariViewController *sfvc = [[SFSafariViewController alloc] initWithURL:URL];


            //Start the generated SFSafariViewController object 

            [uvc presentViewController:sfvc animated:YES completion:nil];

        }
    }

    end ======================================================================================================
     */
}
