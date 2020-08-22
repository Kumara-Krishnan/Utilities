using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Utilities.Util
{
    public sealed class NetworkConnectivity
    {
        public static NetworkConnectivity Instance { get { return NetworkConnectivitySingleton.Instance; } }

        /// <summary>
        /// Uses Win32 API https://docs.microsoft.com/en-us/windows/win32/api/wininet/nf-wininet-internetgetconnectedstate to check for Network connectivity.
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        private static readonly object padlock = new object();

        private NetworkConnectivity()
        {
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
            CheckAndSetNetworkStatus();
        }

        ~NetworkConnectivity()
        {
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
        }

        public bool IsInternetAvailable => InternetGetConnectedState(out _, 0);

        private bool IsInitialCheck = true;
        private bool PreviouslyOnline = false;

        public event Action<bool> NetworkStatusChanged;

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            CheckAndSetNetworkStatus();
        }

        private void CheckAndSetNetworkStatus()
        {
            lock (padlock)
            {
                bool? reconnected = null;
                if (IsInternetAvailable && !PreviouslyOnline)
                {
                    reconnected = true;
                }
                if (PreviouslyOnline && !IsInternetAvailable)
                {
                    reconnected = false;
                }
                if (IsInitialCheck)
                {
                    IsInitialCheck = false;
                    return;
                }
                if (reconnected != null)
                {
                    NetworkStatusChanged?.Invoke((bool)reconnected);
                }
                PreviouslyOnline = IsInternetAvailable;
            }
        }

        private class NetworkConnectivitySingleton
        {
            static NetworkConnectivitySingleton() { }

            internal static readonly NetworkConnectivity Instance = new NetworkConnectivity();
        }
    }
}