using Matchmaker.Algorithms;
using System;
using System.Windows.Forms;

namespace Matchmaker.UserInterface
{
    public class ProgressUpdater : IDisposable
    {
        readonly FormMain form;
        readonly ToolStripProgressBar progressBar;
        readonly ToolStripLabel label;
        readonly Timer timer;
        readonly IAlgorithmWithProgress algorithm;

        public ProgressUpdater(IAlgorithmWithProgress algorithm, FormMain form, ToolStripProgressBar progressBar, ToolStripLabel label, Timer timer)
        {
            this.form = form;
            this.progressBar = progressBar;
            this.label = label;
            this.algorithm = algorithm;
            this.timer = timer;

            DoActionOnForm((MethodInvoker)delegate
            {
                timer.Tick += Timer_Tick;
                timer.Enabled = true;
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            algorithm.GetProgress(out double progress, out double score);

            DoActionOnForm((MethodInvoker)delegate
            {
                progressBar.Value = (int)(progress * 100);
                label.Text = $"Score is {score:0.###}";
            });
        }

        void DoActionOnForm(Delegate method)
        {
            form.BeginInvoke(method);
        }

        public void Dispose()
        {
            DoActionOnForm((MethodInvoker)delegate
            {
                timer.Enabled = false;
                timer.Tick -= Timer_Tick;
                progressBar.Value = 100;
                label.Text = "Done";
            });
        }
    }
}
