using System;
using System.Threading;
using System.Windows.Forms;

namespace Matchmaker
{
    public class ProgressUpdater : IDisposable
    {
        readonly Form1 form;
        readonly ToolStripProgressBar progressBar;
        readonly ToolStripLabel label;
        readonly System.Windows.Forms.Timer timer;
        readonly IAlgorithmWithProgress algorithm;

        public ProgressUpdater(IAlgorithmWithProgress algorithm, Form1 form, ToolStripProgressBar progressBar, ToolStripLabel label, System.Windows.Forms.Timer timer)
        {
            this.form = form;
            this.progressBar = progressBar;
            this.label = label;
            this.algorithm = algorithm;
            this.timer = timer;

            //this.timer = new System.Threading.Timer(UpdateProgress, null, 0, 100);
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
            try
            {
                if (form.Disposing) throw new ObjectDisposedException(nameof(form));
                form.BeginInvoke(method);
            }
            catch (ObjectDisposedException)
            {
                Dispose();
            }
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

    public interface IAlgorithmWithProgress
    {
        public void GetProgress(out double progress, out double score);
    }
}
