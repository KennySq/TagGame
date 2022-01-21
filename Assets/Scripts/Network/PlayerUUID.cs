using System.Collections;
using UnityEngine;
using System;

namespace Utils
{
    //dummy UUID Class
    class PlayerUUID
    {
        private static long uuid;
        public static bool isInitialized { get; private set; }

        public static long UUID
        {
            get
            {
                if (!isInitialized)
                {
                    uuid = DateTime.UtcNow.Ticks; // dummy
                    isInitialized = true;
                }

                return uuid;
            }
        }
    }
}