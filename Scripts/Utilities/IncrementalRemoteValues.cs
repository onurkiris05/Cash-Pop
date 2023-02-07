using ElephantSDK;

namespace _Main.Scripts.Utilities
{
    public static class IncrementalRemoteValues
    {
        public static int ValueBasePrice = 5;
        public static int ValueIncreaseAmount = 3;
        
        public static int LengthBasePrice = 1;
        public static int LengthIncreaseAmount = 2;
        
        public static int WallBasePrice = 1;
        public static int WallIncreasePrice = 1;
        
        public static int MergeBasePrice = 1;
        public static int MergeIncreaseAmount = 2;

        public static void LoadValues()
        {
            var remoteConfigs = RemoteConfig.GetInstance();
            ValueBasePrice = remoteConfigs.GetInt("value_base_price", 5);
            ValueIncreaseAmount = remoteConfigs.GetInt("value_increase_amount", 3);
            
            LengthBasePrice = remoteConfigs.GetInt("length_base_price", 1);
            LengthIncreaseAmount = remoteConfigs.GetInt("length_increase_amount", 2);
            
            WallBasePrice = remoteConfigs.GetInt("wall_base_price", 2);
            WallIncreasePrice = remoteConfigs.GetInt("wall_increase_amount", 2);
            
            MergeBasePrice = remoteConfigs.GetInt("merge_base_price", 1);
            MergeIncreaseAmount = remoteConfigs.GetInt("merge_increase_amount", 2);
        }
    }
}