using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Text.RegularExpressions;

public static class PKCEHelper
{
    //=========== OAUTH PKCE CODE FLOW ==============
    //    The user clicks Login within the application.
    //    Auth0's SDK creates a cryptographically-random code_verifier and from this generates a code_challenge.
    //    Auth0's SDK redirects the user to the Auth0 Authorization Server (/authorize endpoint) along with the code_challenge.
    //    Your Auth0 Authorization Server redirects the user to the login and authorization prompt.
    //    The user authenticates using one of the configured login options and may see a consent page listing the permissions Auth0 will give to the application.
    //    Your Auth0 Authorization Server stores the code_challenge and redirects the user back to the application with an authorization code, which is good for one use.
    //    Auth0's SDK sends this code and the code_verifier (created in step 2) to the Auth0 Authorization Server (/oauth/token endpoint).
    //    Your Auth0 Authorization Server verifies the code_challenge and code_verifier.
    //    Your Auth0 Authorization Server responds with an ID Token and Access Token (and optionally, a Refresh Token).
    //    Your application can use the Access Token to call an API to access information about the user.
    //    The API responds with requested data.

    public static void ExampleUse()
    {
        string CodeVerifier = GenerateNonce();
        string CodeChallenge = GenerateCodeChallenge(CodeVerifier);
    }

    /// <summary>
    /// Generates and returns a nonce value that is in line with the PCKE standard.
    /// </summary>
    public static string GenerateNonce()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz123456789-_.~";
        var random = new System.Random();
        var nonce = new char[128];
        for (int i = 0; i < nonce.Length; i++)
        {
            nonce[i] = chars[random.Next(chars.Length)];
        }

        return new string(nonce);
    }

    /// <summary>
    /// Hashes a PKCE nonce value using SHA256 then encodes it using base64 URL encoding.
    /// </summary>
    /// <param name="codeVerifier">A PKCE nonce value</param>
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        var b64Hash = Convert.ToBase64String(hash);
        var code = Regex.Replace(b64Hash, "\\+", "-");
        code = Regex.Replace(code, "\\/", "_");
        code = Regex.Replace(code, "=+$", "");
        return code;
    }
}
