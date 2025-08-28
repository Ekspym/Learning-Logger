using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestCasesModule
{
    public class DummyWordpadLogic : BaseModule
    {
        private readonly int totalWords;
        private readonly int? failAtWord;

        public DummyWordpadLogic(IAgentTaskProgressQueue progressQueue, int totalWords, int? failAtWord = null)
            : base(progressQueue)
        {
            this.totalWords = totalWords;
            this.failAtWord = failAtWord;
        }

        public ModuleReportDto TryExecute(AgentJobDto job)
        {
            LogProgress(job, "Starting dummy typing...", 0);

            for (int i = 1; i <= totalWords; i++)
            {
                SimulateTypingWord(i);

                if (failAtWord.HasValue && i == failAtWord.Value)
                {
                    throw new InvalidOperationException($"Simulated failure at word {i}");
                }

                if (i % 100 == 0 || i == totalWords)
                {
                    int progress = (int)((i / (double)totalWords) * 100);
                    LogProgress(job, $"Typed {i} words...", progress);
                }
            }

            LogProgress(job, "Finished typing.", 100);

            return new ModuleReportDto { Success = true };
        }

        private void SimulateTypingWord(int wordIndex)
        {
            var dummyWord = $"Word{wordIndex}";
            Thread.SpinWait(10000);
        }
    }
}
