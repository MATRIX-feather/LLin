namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Data.Consts
{
    public class OsuStates
    {
        /// <summary>
        /// 默认状态（在主界面中）
        /// 如果当前屏幕没有已知的对应的值，则也会切换到此状态。
        /// </summary>
        public const int DEFAULT_IDLE = 0;

        /// <summary>
        /// 在编辑器中
        /// </summary>
        public const int EDITOR = 1;

        /// <summary>
        /// 正在游玩
        /// </summary>
        public const int PLAYING = 2;

        /// <summary>
        /// osu!stable只会在退出时进入这个状态,
        /// 我们目前对此状态无从下手，因为暂时没找到如何在osu!退出时执行动作的方法
        /// </summary>
        public const int HOST_EXITING = 3;

        /// <summary>
        /// osu!stable中切换到编辑器的歌曲选择时会进入此状态。
        /// 但此功能在lazer中并不存在
        /// </summary>
        public const int EDITOR_SONG_SELECT = 4;

        /// <summary>
        /// 单人游戏歌曲选择
        /// </summary>
        public const int SOLO_SONG_SELECT = 5;

        /// <summary>
        /// 结算界面
        /// </summary>
        public const int RESULTS = 7;

        /// <summary>
        /// 在多人游戏大厅中
        /// </summary>
        public const int MULTIPLAYER_LOUNGE = 11;

        /// <summary>
        /// 在多人游戏房间中或正在创建多人游戏房间
        /// PS: 有一个正在创建歌单是因为目前的精度只能做到跟踪屏幕，但 MultiplayerMatchSubScreen 可以有 正在创建 和 已经创建 两种状态
        /// </summary>
        public const int MULTIPLAYER_ROOM = 12;

        // 以下为Hikariii独有

        /// <summary>
        /// 在歌单模式大厅中
        /// </summary>
        public const int PLAYLISTS_LOUNGE = 10001;

        /// <summary>
        /// 在歌单模式房间中或正在创建歌单
        /// PS: 和上面的 <see cref="MULTIPLAYER_ROOM"/> 情况一样
        /// </summary>
        public const int PLAYLISTS_ROOM = 10002;

        /// <summary>
        /// 在Hikariii(LLin)播放器中
        /// </summary>
        public const int HIKARIII_PLAYER = 10003;

        /// <summary>
        /// 在Hikariii播放器的歌曲界面中
        /// </summary>
        public const int HIKARIII_SONG_SELECT = 10004;
    }
}
