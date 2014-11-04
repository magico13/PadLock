using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PadLock
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class PadLock : MonoBehaviour
    {
        private static bool previous_Setting = false, do_Reset = false, counter_started = false;

        public PadLock()
        {
            Debug.Log("PL: Adding event");
            GameEvents.OnVesselRollout.Add(OnRollout);
        }

        public void OnDestroy()
        {
            if (do_Reset)
            {
                HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = previous_Setting;
                do_Reset = false;
                counter_started = false;
            }
            Debug.Log("PL: OnDestroy");
            GameEvents.OnVesselRollout.Remove(OnRollout);
        }

        private static DateTime resetCounter;
        public void FixedUpdate()
        {
            if (do_Reset)
            {
                if (!counter_started && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.loaded && !FlightGlobals.ActiveVessel.packed)
                {
                    Debug.Log("PL: Starting counter");
                    resetCounter = DateTime.Now;
                    counter_started = true;
                }
                if (counter_started && DateTime.Now.CompareTo(resetCounter.AddSeconds(1)) > 0)
                {
                    Debug.Log("PL: Reseting");
                    HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = previous_Setting;
                    do_Reset = false;
                    counter_started = false;
                }
            }
        }

        public void OnRollout(ShipConstruct rolledOut)
        {
            Debug.Log("PL: OnRollout");
            previous_Setting = HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities;

            if (previous_Setting) return;

            HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = true;

            do_Reset = true;
        }

    }
}
