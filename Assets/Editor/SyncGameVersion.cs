#if UNITY_EDITOR
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Piratera.Constance;
using System.Diagnostics;

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

            UploadDrive(
                GetGameFile(type, version),
                type,
                version
            );

        }

        public static string GetGameFile(BuildType type, string version)
        {
            switch (type)
            {
                case BuildType.WINDOW_DEV:
                    return $"{MyBuildProcessor.BuildFolder}/l2cpp/Piratera Installer v{version}.exe"; ;
                case BuildType.ANDROID_DEV:
                    return $"{MyBuildProcessor.BuildFolder}/Android/Piratera v{version}.apk";
                default:
                    return "";
            }
        }

        public static string GetVersionUrl(BuildType type)
        {
            switch (type)
            {
                case BuildType.WINDOW_DEV:
                    return GameConst.WINDOW_DEV_VERSION_URL;
                case BuildType.ANDROID_DEV:
                    return GameConst.ANDROID_DEV_VERSION_URL;
                default:
                    return "";
            }
        }

        public static void UploadDrive(string filePath, BuildType buildType, string version)
        {
            Process process = new Process();
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