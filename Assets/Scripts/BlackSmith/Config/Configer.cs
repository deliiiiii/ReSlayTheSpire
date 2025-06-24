using Sirenix.OdinInspector;

namespace BlackSmith
{
    public class Configer : Singleton<Configer>
    {
        [ShowInInspector]
        public MainConfig MainConfigIns;
        
        public static MainConfig MainConfig => Instance.MainConfigIns;
    }
}