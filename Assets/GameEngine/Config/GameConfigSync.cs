using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Piratera.Config
{
    public class GameConfigSync : MonoBehaviour
    {
        class ConfigFileMeta
        {
            public string FileName;
            public string MD5Hash;
            public string Content;

            public ConfigFileMeta(ISFSObject packet)
            {
                NewFromSFSObject(packet);
            }

            private void NewFromSFSObject(ISFSObject packet)
            {
                FileName = packet.GetUtfString("file_name");
                MD5Hash = packet.GetUtfString("md5_name");
                Content = packet.GetUtfString("content");
            }

            private void SaveFile()
            {
                File.WriteAllText(Application.persistentDataPath + FileName, Content);
            }

        
        }


        private List<ConfigFileMeta> manifest;


        private void Start()
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("path", "");
            NetworkController.Send(SFSAction.GET_CONFIG, data);
            NetworkController.AddServerActionListener(onReceiveServerAction);
        }



        private void onReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
           
           if (action == SFSAction.GET_CONFIG)
           {

              if (errorCode == SFSErrorCode.SUCCESS)
                {
                    ISFSArray data = packet.GetSFSArray("manifest");

                    foreach (SFSObject obj in data)
                    {
                        manifest.Add(new ConfigFileMeta(obj));
                    }
                }
         
           }
        }

        private void OnDestroy()
        {
            NetworkController.RemoveServerActionListener(onReceiveServerAction);
        }

        private static string CalculateMD5(string filename)
        {
            string path = Application.persistentDataPath + filename;
            if (File.Exists(path)) {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            } else
            {
                return "";
            }
         
        }
    }
}
