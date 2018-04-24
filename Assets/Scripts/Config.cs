using System;
using UnityEngine;
 
public class Config {
	 public static readonly string REMOTE_HOST = "localhost";
	//public static readonly string REMOTE_HOST = "worldofbalance.westus.cloudapp.azure.com";

    public static string GetHost()
    {
        string envHost = Environment.GetEnvironmentVariable("WOB_HOST");
        return String.IsNullOrEmpty(envHost) ? REMOTE_HOST : envHost;
    }
}
