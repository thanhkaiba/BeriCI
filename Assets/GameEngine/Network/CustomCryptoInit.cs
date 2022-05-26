#if UNITY_EDITOR || !UNITY_WEBGL

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;


public class CustomCryptoInitializerV2 : ICryptoInitializer
{
    private const string KEY_SESSION_TOKEN = "SessToken";
    private const string TARGET_SERVLET = "/BlueBox/CryptoManager";
    private SmartFox sfs;
    private bool useHttps = true;

    public CustomCryptoInitializerV2(SmartFox sfs)
    {
        if (!sfs.IsConnected)
        {
            throw new InvalidOperationException("Cryptography cannot be initialized before connecting to SmartFoxServer!");
        }
        if (sfs.GetSocketEngine().CryptoKey != null)
        {
            throw new InvalidOperationException("Cryptography is already initialized!");
        }
        this.sfs = sfs;
    }

    [AsyncStateMachine(typeof(InitCrytoState)), DebuggerStepThrough]
    private void Init()
    {
        InitCrytoState stateMachine = new InitCrytoState
        {
            voidMethodBuilder = AsyncVoidMethodBuilder.Create(),
            cryptoInitInstance = this,
            curState = -1
        };
        stateMachine.voidMethodBuilder.Start<InitCrytoState>(ref stateMachine);
    }

    private void OnHttpError(string errorMsg)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["success"] = false;
        data["errorMessage"] = errorMsg;
        sfs.Dispatcher.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, data));
    }

    private void OnHttpResponse(string rawData)
    {
        byte[] data = Convert.FromBase64String(rawData);
        ByteArray key = new ByteArray();
        ByteArray iv = new ByteArray();
        key.WriteBytes(data, 0, 0x10);
        iv.WriteBytes(data, 0x10, 0x10);
        this.sfs.GetSocketEngine().CryptoKey = new CryptoKey(iv, key);
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["success"] = true;
        sfs.Dispatcher.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, dictionary));
    }

    public void Run()
    {
        Init();
    }

    [CompilerGenerated]
    private sealed class InitCrytoState : IAsyncStateMachine
    {
        public int curState;
        public AsyncVoidMethodBuilder voidMethodBuilder;
        public CustomCryptoInitializerV2 cryptoInitInstance;
        private string targetUrl;
        private Dictionary<string, string> formContent;
        private UnityWebRequest wwwRequest;
        private string responseData;
        private Exception exceptionData;
        private TaskAwaiter taskAwaiterU;

        void IAsyncStateMachine.MoveNext()
        {
            Exception exception;
            int num = curState;
            try
            {
                if (num != 0)
                {
                    string[] textArray1 = new string[5];
                    string[] textArray2 = new string[5];
                    textArray2[0] = cryptoInitInstance.useHttps ? "https://" : "http://";
                    string[] local1 = textArray2;
                    local1[1] = cryptoInitInstance.sfs.Config.Host;
                    local1[2] = ":";
                    local1[3] = (cryptoInitInstance.useHttps ? cryptoInitInstance.sfs.Config.HttpsPort : cryptoInitInstance.sfs.Config.HttpPort).ToString();
                    string[] local2 = local1;
                    local2[4] = "/BlueBox/CryptoManager";
                    targetUrl = string.Concat(local2);
                    formContent = new Dictionary<string, string>();
                    formContent.Add("SessToken", cryptoInitInstance.sfs.SessionToken);
                    wwwRequest = UnityWebRequest.Post(targetUrl, formContent);
                    wwwRequest.certificateHandler = new CustomCertificateHandler();

                }
                try
                {
                    if (num != 0)
                    {
                    }
                    try
                    {
                        TaskAwaiter awaiter;
                        if (num == 0)
                        {
                            awaiter = this.taskAwaiterU;
                            taskAwaiterU = new TaskAwaiter();
                            curState = num = -1;
                            goto TR_000B;
                        }
                        else
                        {
                            awaiter = ((AsyncOperation)this.wwwRequest.SendWebRequest()).GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto TR_000B;
                            }
                            else
                            {
                                curState = num = 0;
                                taskAwaiterU = awaiter;
                                InitCrytoState stateMachine = this;
                                voidMethodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                            }
                        }
                        return;
                    TR_000B:
                        awaiter.GetResult();
                        if (wwwRequest.responseCode == 200L)
                        {
                            responseData = wwwRequest.downloadHandler.text;
                            cryptoInitInstance.OnHttpResponse(responseData);
                            responseData = null;
                        }
                        else
                        {
                            cryptoInitInstance.OnHttpError("Error " + wwwRequest.responseCode.ToString() + ": " + wwwRequest.error);
                            goto TR_0005;
                        }
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        exceptionData = exception;
                        cryptoInitInstance.OnHttpError(this.exceptionData.Message);
                    }
                }
                finally
                {
                    if ((num < 0) && (wwwRequest != null))
                    {
                        wwwRequest.Dispose();
                    }
                }
                this.wwwRequest = null;
                goto TR_0005;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                curState = -2;
                targetUrl = null;
                formContent = null;
                voidMethodBuilder.SetException(exception);
            }
            return;
        TR_0005:
            curState = -2;
            targetUrl = null;
            formContent = null;
            voidMethodBuilder.SetResult();
        }

        [DebuggerHidden]
        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }


    }
}
#endif
