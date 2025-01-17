﻿using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;

namespace DotNetNuke.Web.Services
{
    internal class DigestAuthentication
    {
        private static readonly MD5 Md5 = new MD5CryptoServiceProvider();
        private DigestAuthenticationRequest _request;
        private string _password;
        private readonly int _portalId;

        public DigestAuthenticationRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public bool IsValid { get; private set; }

        public bool IsNonceStale { get; private set; }

        public string CalculateHashedDigest()
        {
            return CreateMd5HashBinHex(GenerateUnhashedDigest());
        }

        public IPrincipal User { get; private set; }

        public DigestAuthentication(DigestAuthenticationRequest request, int portalId)
        {
            _request = request;
            _portalId = portalId;
            AuthenticateRequest();
        }

        private void AuthenticateRequest()
        {
            _password = GetPassword(Request);
            if(_password != null)
            {
                IsNonceStale = ! (IsNonceValid(_request.RequestParams["nonce"]));
                //Services.Logging.LoggingController.SimpleLog(String.Format("Request hash: {0} - Response Hash: {1}", _request.RequestParams("response"), HashedDigest))
                if ((! IsNonceStale) && _request.RequestParams["response"] == CalculateHashedDigest())
                {
                    IsValid = true;
                    User = new GenericPrincipal(new GenericIdentity(_request.RawUsername, "digest"), null);
                }
            }
        }

        private string GetPassword(DigestAuthenticationRequest request)
        {
            UserInfo user = UserController.GetUserByName(_portalId, request.CleanUsername);
            if (user == null)
            {
                user = UserController.GetUserByName(_portalId, request.RawUsername);
            }
            if (user == null)
            {
                return null;
            }
            var password = UserController.GetPassword(ref user, "");
            
            string ipAddress = "";
            if (HttpContext.Current.Request.UserHostAddress != null)
            {
                ipAddress = HttpContext.Current.Request.UserHostAddress;
            }

            //Try to validate user
            var loginStatus = UserLoginStatus.LOGIN_FAILURE;
            user = UserController.ValidateUser(_portalId, user.Username, password, "DNN", "", ipAddress, ref loginStatus);

            return user != null ? password : null;
        }

        private string GenerateUnhashedDigest()
        {
            string a1 = String.Format("{0}:{1}:{2}", _request.RequestParams["username"].Replace("\\\\", "\\"),
                                      _request.RequestParams["realm"], _password);
            string ha1 = CreateMd5HashBinHex(a1);
            string a2 = String.Format("{0}:{1}", _request.HttpMethod, _request.RequestParams["uri"]);
            string ha2 = CreateMd5HashBinHex(a2);
            string unhashedDigest;
            if (_request.RequestParams["qop"] != null)
            {
                unhashedDigest = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", ha1, _request.RequestParams["nonce"],
                                               _request.RequestParams["nc"], _request.RequestParams["cnonce"],
                                               _request.RequestParams["qop"], ha2);
            }
            else
            {
                unhashedDigest = String.Format("{0}:{1}:{2}", ha1, _request.RequestParams["nonce"], ha2);
            }
            //Services.Logging.LoggingController.SimpleLog(A1, HA1, A2, HA2, unhashedDigest)
            return unhashedDigest;
        }

        private static string CreateMd5HashBinHex(string val)
        {
            //Services.Logging.LoggingController.SimpleLog(String.Format("Creating Hash for {0}", val))
            //Services.Logging.LoggingController.SimpleLog(String.Format("Back and forth: {0}", Encoding.Default.GetString(Encoding.Default.GetBytes(val))))
            byte[] bha1 = Md5.ComputeHash(Encoding.Default.GetBytes(val));
            string ha1 = "";
            for (int i = 0; i <= 15; i++)
            {
                ha1 += String.Format("{0:x02}", bha1[i]);
            }
            return ha1;
        }

        //the nonce is created in DotNetNuke.HttpModules.Services.ServicesModule.CreateNewNonce
        private static bool IsNonceValid(string nonce)
        {
            DateTime expireTime;

            int numPadChars = nonce.Length%4;
            if (numPadChars > 0)
            {
                numPadChars = 4 - numPadChars;
            }
            string newNonce = nonce.PadRight(nonce.Length + numPadChars, '=');

            try
            {
                byte[] decodedBytes = Convert.FromBase64String(newNonce);
                string expireStr = Encoding.Default.GetString(decodedBytes);
                expireTime = DateTime.Parse(expireStr);
            }
            catch (FormatException)
            {
                return false;
            }

            return (DateTime.Now <= expireTime);
        }
    }
}