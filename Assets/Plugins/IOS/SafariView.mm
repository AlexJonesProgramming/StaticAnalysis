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

   void OpenDeepLink(const char *url)
   {
      NSURL *myURL = [NSURL URLWithString:[[NSString alloc] initWithUTF8String:url]];
      [[UIApplication sharedApplication] openURL:myURL];
   }

}