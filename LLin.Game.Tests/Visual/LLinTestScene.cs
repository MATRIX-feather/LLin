using osu.Framework.Testing;

namespace LLin.Game.Tests.Visual
{
    public class LLinTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new LLinTestSceneTestRunner();

        private class LLinTestSceneTestRunner : LLinGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
