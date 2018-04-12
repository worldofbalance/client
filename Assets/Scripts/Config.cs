using System;
using UnityEngine;
 
public class Config {
	 //public static readonly string REMOTE_HOST = "localhost";
	//public static readonly string REMOTE_HOST = "52.25.138.18";
  	// public static readonly string REMOTE_HOST = "thecity.sfsu.edu";
	//public static readonly string REMOTE_HOST = "smurf.sfsu.edu";
	public static readonly string REMOTE_HOST = "worldofbalance.westus.cloudapp.azure.com";
	// public static readonly string REMOTE_HOST = "54.153.66.118";   // AWS from Ben, 2-2017

	// public static readonly string REMOTE_HOST = "smurf.sfsu.edu";
	//public static readonly string REMOTE_HOST = "130.212.93.116";
	//public static readonly string REMOTE_HOST = "52.32.228.220";

    public static string GetHost()
    {
        string envHost = Environment.GetEnvironmentVariable("WOB_HOST");
        return String.IsNullOrEmpty(envHost) ? REMOTE_HOST : envHost;
    }
}
