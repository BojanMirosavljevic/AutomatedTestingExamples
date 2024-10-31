using System;
using System.Collections;
using UnityEngine;

public static class Util 
{
    public static bool IsDevelopmentBuild()
    {
#if DEVELOPMENT_BUILD
        return true;
#endif
        return false;
    }

    public static bool NoInternetConnectivity()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }
}