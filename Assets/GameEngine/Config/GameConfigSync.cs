using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Piratera.Config
{
    public class GameConfigSync : MonoBehaviour
    {
        public class ConfigFileMeta
        {
            public string FileName;
            public string MD5Hash;
            public string Content;

            public ConfigFileMeta(ISFSObject packet)
            {
                UnpackMeta(packet);
            }

            private void UnpackMeta(ISFSObject packet)
            {
                FileName = packet.GetUtfString("file_name");
                MD5Hash = packet.GetUtfString("md5_hash");
               
            }

            public void NewFromSFSObject(ISFSObject packet)
            {
                FileName = packet.GetUtfString("file_name");
                MD5Hash = packet.GetUtfString("md5_hash");
                Content = packet.GetUtfString("content");
                
            }

          

            public bool NeedUpdate()
            {
                string localMD5Hash = CalculateMD5();
                if (localMD5Hash != MD5Hash)
                {
                    return true;
                }
                return false;
            }

            public void RequestDowload()
            {
                SFSObject data = new SFSObject();
                data.PutUtfString("path", FileName);
                NetworkController.Send(SFSAction.GET_CONFIG, data);
            }

            private string CalculateMD5()
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    using var md5 = MD5.Create();
                    var hash = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(Content));
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
                else
                {
                    return "";
                }

            }


        }


        public static Dictionary<string, ConfigFileMeta> manifest = new Dictionary<string, ConfigFileMeta>();
        private Action<float> UpdateProgressBar;
        private Action OnSuccess;
        private Action OnError;
        private int TotalUnSync = 0;
        private int TotalSynced = 0;

        public static bool Synced = false;



        private void Start()
        {
            
            NetworkController.AddServerActionListener(onReceiveServerAction);

           
        }

        public void StartFlowSync(Action<float> progressAction, Action onSuccess, Action onError)
        {
           
            UpdateProgressBar = progressAction;
            OnSuccess = () => {
                Synced = true;
                onSuccess();
            };
            OnError = () => {
                Synced = false;
                onError();
            };
         
            SFSObject data = new SFSObject();
            data.PutUtfString("path", "");
            NetworkController.Send(SFSAction.GET_CONFIG_MANIFEST, data);
            manifest.Clear();
        }

       
        public static void ResetData()
        {
            Synced = false;
        }


        private void onReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            switch (action)
            {
                case SFSAction.GET_CONFIG_MANIFEST:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            ISFSArray data = packet.GetSFSArray("manifest");

                            foreach (SFSObject obj in data)
                            {
                                ConfigFileMeta meta = new ConfigFileMeta(obj);
                                if (!manifest.ContainsKey(meta.FileName))
                                {
                                    manifest.Add(meta.FileName, new ConfigFileMeta(obj));

                                    if (meta.NeedUpdate())
                                    {
                                        TotalUnSync++;
                                        meta.RequestDowload();
                                    }
                                }

                            }
                            CheckSuccess();

                        } else
                        {
                            OnError();
                        }
                        break;
                    }
                case SFSAction.GET_CONFIG:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            string fileName = packet.GetUtfString("file_name");
                            if (manifest.ContainsKey(fileName))
                            {
                                manifest[fileName].NewFromSFSObject(packet);
                                UpdateProgressBar(0.3f / TotalUnSync);
                                TotalSynced++;
                                CheckSuccess();


                            }
                        }
                        else
                        {
                            OnError();
                        }
                        break;
                    }



            }
        
        }

        private bool CheckSuccess()
        {
            if (TotalSynced >= TotalUnSync)
            {
                OnSuccess();
                return true;
            }

            return false;
        }

        private void OnDestroy()
        {
            NetworkController.RemoveServerActionListener(onReceiveServerAction);
        }

     
        public static string[] GetSailorFolder()
        {
            return manifest.Keys.Where(fileName => fileName.StartsWith("configs/Sailors")).ToArray();
        }

        public static string GetContent(string fileName)
        {
            if (manifest.ContainsKey(fileName))
            {
                return manifest[fileName].Content;
            }
            return "";
        }
    }
}
