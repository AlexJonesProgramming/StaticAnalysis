using FakeItEasy;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.TestHelpers;
using i5.Toolkit.Core.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace i5.Toolkit.Core.Tests.OpenIDConnectClient
{
    public class GithubOIDCProviderTests
    {
        [Test]
        public void Constructor_Initialized_ContentLoaderNotNull()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();

            Assert.IsNotNull(ghoidc.RestConnector);
        }

        [Test]
        public void Constructor_Initialized_JsonWrapperNotNull()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();

            Assert.IsNotNull(ghoidc.JsonSerializer);
        }

        [UnityTest]
        public IEnumerator GetAccessCodeFromTokenAsync_NoClientData_ReturnsEmptyString()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            ghoidc.RestConnector = A.Fake<IRestConnector>();

            LogAssert.Expect(LogType.Error,
                new Regex(@"\w*No client data supplied for the OpenID Connect Client\w*"));

            Task<string> task = ghoidc.GetAccessTokenFromCodeAsync("", "");

            yield return AsyncTest.WaitForTask(task);

            string res = task.Result;

            Assert.IsEmpty(res);
        }

        [UnityTest]
        public IEnumerator GetAccessCodeFromTokenAsync_WebResponseSuccess_ReturnsToken()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.PostAsync(A<string>.Ignored, A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(Task.FromResult(new WebResponse<string>("json string", null, 200)));
            ghoidc.RestConnector = restConnector;
            ghoidc.ClientData = A.Fake<ClientData>();
            GitHubAuthorizationFlowAnswer answer = new GitHubAuthorizationFlowAnswer();
            answer.access_token = "myAccessToken";
            IJsonSerializer serializer = A.Fake<IJsonSerializer>();
            A.CallTo(() => serializer.FromJson<GitHubAuthorizationFlowAnswer>(A<string>.Ignored))
                .Returns(answer);
            ghoidc.JsonSerializer = serializer;

            Task<string> task = ghoidc.GetAccessTokenFromCodeAsync("", "");

            yield return AsyncTest.WaitForTask(task);

            string res = task.Result;

            Assert.AreEqual(answer.access_token, res);
        }

        [UnityTest]
        public IEnumerator GetAccessCodeFromTokenAsync_WebResponseFailed_ReturnsEmptyToken()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.PostAsync(A<string>.Ignored, A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(Task.FromResult(new WebResponse<string>("my error", 400)));
            ghoidc.RestConnector = restConnector;
            ghoidc.ClientData = A.Fake<ClientData>();
            ghoidc.JsonSerializer = A.Fake<IJsonSerializer>();

            LogAssert.Expect(LogType.Error,
                new Regex(@"\w*my error\w*"));

            Task<string> task = ghoidc.GetAccessTokenFromCodeAsync("", "");

            yield return AsyncTest.WaitForTask(task);

            string res = task.Result;

            Assert.IsEmpty(res);
        }

        [Test]
        public void GetAccessToken_TokenProvided_ExtractsToken()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();
            redirectParameters.Add("token", "myAccessToken");

            string res = ghoidc.GetAccessToken(redirectParameters);

            Assert.AreEqual("myAccessToken", res);
        }

        [Test]
        public void GetAccessToken_TokenNotProvided_ReturnsEmptyToken()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();

            LogAssert.Expect(LogType.Error, new Regex(@"\w*Redirect parameters did not contain access token\w*"));

            string res = ghoidc.GetAccessToken(redirectParameters);

            Assert.IsEmpty(res);
        }

        [UnityTest]
        public IEnumerator GetUserInfoAsync_WebResponseSuccessful_ReturnsUserInfo()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.GetAsync(A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(new WebResponse<string>("answer", null, 200));
            ghoidc.RestConnector = restConnector;
            GitHubUserInfo userInfo = new GitHubUserInfo("tester", "tester@test.com");
            IJsonSerializer serializer = A.Fake<IJsonSerializer>();
            A.CallTo(() => serializer.FromJson<GitHubUserInfo>(A<string>.Ignored))
                .Returns(userInfo);
            ghoidc.JsonSerializer = serializer;

            Task<IUserInfo> task = ghoidc.GetUserInfoAsync("");

            yield return AsyncTest.WaitForTask(task);

            IUserInfo res = task.Result;

            Assert.AreEqual(userInfo.Email, res.Email);
        }

        [UnityTest]
        public IEnumerator GetUserInfoAsync_WebResponseFailed_ReturnsNull()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.GetAsync(A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(new WebResponse<string>("This is a simulated error", 400));
            ghoidc.RestConnector = restConnector;

            LogAssert.Expect(LogType.Error, new Regex(@"\w*This is a simulated error\w*"));

            Task<IUserInfo> task = ghoidc.GetUserInfoAsync("");

            yield return AsyncTest.WaitForTask(task);

            IUserInfo res = task.Result;

            Assert.IsNull(res);
        }

        [UnityTest]
        public IEnumerator GetUserInfoAsync_JsonParseFailed_ReturnsNull()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.GetAsync(A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(new WebResponse<string>("answer", null, 200));
            ghoidc.RestConnector = restConnector;
            GitHubUserInfo userInfo = new GitHubUserInfo("tester", "tester@test.com");
            IJsonSerializer serializer = A.Fake<IJsonSerializer>();
            A.CallTo(() => serializer.FromJson<GitHubUserInfo>(A<string>.Ignored))
                .Returns(null);
            ghoidc.JsonSerializer = serializer;

            LogAssert.Expect(LogType.Error, new Regex(@"\w*Could not parse user info\w*"));

            Task<IUserInfo> task = ghoidc.GetUserInfoAsync("");

            yield return AsyncTest.WaitForTask(task);

            IUserInfo res = task.Result;

            Assert.IsNull(res);
        }

        [UnityTest]
        public IEnumerator CheckAccessTokenAsync_WebResponseSuccessful_ReturnsTrue()
        {
            GitHubOidcProvider lloidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.GetAsync(A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(new WebResponse<string>("answer", null, 200));
            lloidc.RestConnector = restConnector;
            GitHubUserInfo userInfo = new GitHubUserInfo("tester", "tester@test.com");
            IJsonSerializer serializer = A.Fake<IJsonSerializer>();
            A.CallTo(() => serializer.FromJson<GitHubUserInfo>(A<string>.Ignored))
                .Returns(userInfo);
            lloidc.JsonSerializer = serializer;

            Task<bool> task = lloidc.CheckAccessTokenAsync("");

            yield return AsyncTest.WaitForTask(task);

            bool res = task.Result;

            Assert.IsTrue(res);
        }

        [UnityTest]
        public IEnumerator CheckAccessTokenAsync_WebResponseFailed_ReturnsFalse()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IRestConnector restConnector = A.Fake<IRestConnector>();
            A.CallTo(() => restConnector.GetAsync(A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(new WebResponse<string>("This is a simulated error", 400));
            ghoidc.RestConnector = restConnector;

            LogAssert.Expect(LogType.Error, new Regex(@"\w*This is a simulated error\w*"));

            Task<bool> task = ghoidc.CheckAccessTokenAsync("");

            yield return AsyncTest.WaitForTask(task);

            bool res = task.Result;

            Assert.IsFalse(res);
        }

        [Test]
        public void OpenLoginPage_UriGiven_BrowserOpened()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            IBrowser browser = A.Fake<IBrowser>();
            ghoidc.Browser = browser;
            ghoidc.ClientData = A.Fake<ClientData>();
            string[] testScopes = new string[] { "testScope" };

            ghoidc.OpenLoginPage(testScopes, "http://www.test.com");

            A.CallTo(() => browser.OpenURL(A<string>.That.Contains("http://www.test.com"))).MustHaveHappenedOnceExactly();
            A.CallTo(() => browser.OpenURL(A<string>.That.Contains("testScope"))).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetAuthorizationCode_CodeProvided_ExtractsCode()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();
            redirectParameters.Add("code", "myCode");
            string res = ghoidc.GetAuthorizationCode(redirectParameters);

            Assert.AreEqual("myCode", res);
        }

        [Test]
        public void GetAuthorizationCode_CodeNotProvided_ReturnsEmptyString()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();

            LogAssert.Expect(LogType.Error, new Regex(@"\w*Redirect parameters did not contain authorization code\w*"));

            string res = ghoidc.GetAuthorizationCode(redirectParameters);

            Assert.IsEmpty(res);
        }

        [Test]
        public void ParametersContainError_NoError_ReturnsFalse()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();

            bool res = ghoidc.ParametersContainError(redirectParameters, out string message);

            Assert.IsFalse(res);
        }

        [Test]
        public void ParametersContainError_NoError_ErrorMessageEmpty()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();

            bool res = ghoidc.ParametersContainError(redirectParameters, out string message);

            Assert.IsEmpty(message);
        }

        [Test]
        public void ParametersContainError_ErrorGiven_ReturnsTrue()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();
            redirectParameters.Add("error", "This is a simulated error");

            bool res = ghoidc.ParametersContainError(redirectParameters, out string message);

            Assert.IsTrue(res);
        }

        [Test]
        public void ParametersContainError_ErrorGiven_ErrorMessageSet()
        {
            GitHubOidcProvider ghoidc = new GitHubOidcProvider();
            Dictionary<string, string> redirectParameters = new Dictionary<string, string>();
            string errorMsg = "This is a simulated error";
            redirectParameters.Add("error", errorMsg);

            bool res = ghoidc.ParametersContainError(redirectParameters, out string message);

            Assert.AreEqual(errorMsg, message);
        }
    }
}
