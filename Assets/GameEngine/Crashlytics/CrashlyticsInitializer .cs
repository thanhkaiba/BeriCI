namespace Piratera.Crashlytics
{
    using UnityEngine;
    using Firebase;
    using System;

    public class CrashlyticsInitializer : Singleton<CrashlyticsInitializer>
    {
        // Use this for initialization
        public void CrashlyticsInit()
        {
            // Initialize Firebase
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    FirebaseApp app = FirebaseApp.DefaultInstance;

                    // Set a flag here for indicating that your project is ready to use Firebase.
                    Debug.Log("Project is ready to use Firebase Crashlytics");
                }
                else
                {
                    Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

    }
}
