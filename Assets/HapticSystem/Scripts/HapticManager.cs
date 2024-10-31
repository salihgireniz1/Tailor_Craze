namespace TailorCraze.Haptic
{
    public static class HapticManager
    {
        /* HAPTIK SIDDETLERI;
         * 
         * -> Pop =  Tek seferlik titresim
         * -> Peek = Pop gibi daha uzun sure titresim
         * -> Nope = İki sefer hizli titresim
         * 
         */
        public static bool IsOnVibration
        {
            get => ES3.Load("Vibration", true);
            set => ES3.Save("Vibration", value);
        }
        static HapticManager()
        {
            Vibration.Init();
        }

        public static void HapticPlay(HapticType hapticType)
        {
            if (!IsOnVibration) return;
            if (hapticType == HapticType.NONE) return;

            switch (hapticType)
            {
                case HapticType.Vibrate:
                    Vibrate(); break;
                case HapticType.VibrateNope:
                    HapticNope(); break;
                case HapticType.VibratePeek:
                    HapticPeek(); break;
                case HapticType.VibratePop:
                    HapticPop(); break;

                default: break;
            }
        }

        static void Vibrate()
        {
            if (!IsOnVibration) return;

            Vibration.Vibrate();
        }

        static void HapticPop()
        {
            if (!IsOnVibration) return;

            Vibration.VibratePop();
        }

        static void HapticPeek()
        {
            if (!IsOnVibration) return;

            Vibration.VibratePeek();
        }

        static void HapticNope()
        {
            if (!IsOnVibration) return;

            Vibration.VibrateNope();
        }
    }

    public enum HapticType
    {
        NONE, VibratePop, VibratePeek, VibrateNope, Vibrate
    }
}
