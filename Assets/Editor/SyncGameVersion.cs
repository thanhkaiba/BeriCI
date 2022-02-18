#if UNITY_EDITOR
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEngine.Networking;

namespace Piratera.Build
{
    public class SyncGameVersion
    {
        public static IEnumerator RuncSyncVersionCode(string url, string version, string downloadUrl)
        {
            var myData = new
            {
                min_version = version,
                start_maintain_time = -1,
                end_maintain_time = -1,
                download_url = downloadUrl
            };
            var request = new UnityWebRequest(url, "PATCH");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(myData));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                UnityEngine.Debug.Log("Error: " + request.error);
            }
            else
            {
                UnityEngine.Debug.Log("All OK");
                UnityEngine.Debug.Log("Status Code: " + request.responseCode);
            }

        }


        public static void Sync(BuildType type, string version)
        {
            string path = EditorUtility.OpenFilePanel(GetGameFolder(type), "", "");
            UploadDrive(
                path,
                type,
                version
            );

        }

        public static string GetGameFolder(BuildType type)
        {
            switch (type)
            {
                case BuildType.WINDOW_DEV:
                case BuildType.WINDOW_LIVE:
                    return $"{MyBuildProcessor.BuildFolder}/l2cpp/"; ;
                case BuildType.ANDROID_DEV:
                case BuildType.ANDROID_LIVE:
                    return $"{MyBuildProcessor.BuildFolder}/Android/";
                default:
                    return MyBuildProcessor.BuildFolder;
            }
        }

        public static void UploadDrive(string filePath, BuildType buildType, string version)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"cmd.exe";
            string args = $"/k node {MyBuildProcessor.BuildFolder}/UploadScript/index.js -p \"{filePath}\" -b {(int)buildType} -v {version}";

            UnityEngine.Debug.Log("Run Node.js: " + args);
            psi.Arguments = @args;

            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }
    }
}

#endif