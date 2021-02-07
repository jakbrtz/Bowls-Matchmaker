using Matchmaker.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matchmaker.UserInterface.Controls
{
    public partial class WeightView : UserControl
    {
        public WeightView()
        {
            InitializeComponent();
        }

        Weight weight;

        const double rescaleMultiplier = 100d;

        public void BindToWeight(Weight weight)
        {
            this.weight = weight;
            TBRvalue.Value = (int)weight.Score;
            TBRmultiplier.Value = (int)(weight.Multiplier * rescaleMultiplier);
        }

        public void UpdateFromWeight() => BindToWeight(weight);

        public string Description
        {
            get => LBLname.Text;
            set => LBLname.Text = value;
        }

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when either a mouse or keyboard action moves the scrollbar")]
        public event EventHandler WeightChanged;

        private void TBRvalue_Scroll(object sender, EventArgs e)
        {
            if (!(weight.Score >= TBRvalue.Maximum && TBRvalue.Value >= TBRvalue.Maximum) && 
                !(weight.Score <= TBRvalue.Minimum && TBRvalue.Value <= TBRvalue.Minimum))
            {
                weight.Score = TBRvalue.Value;
                WeightChanged?.Invoke(this, e);
            }
        }

        private void TBRmultiplier_Scroll(object sender, EventArgs e)
        {
            if (!(weight.Multiplier >= TBRmultiplier.Maximum / rescaleMultiplier && TBRmultiplier.Value >= TBRmultiplier.Maximum) &&
                !(weight.Multiplier <= TBRmultiplier.Minimum / rescaleMultiplier && TBRmultiplier.Value <= TBRmultiplier.Minimum))
            {
                weight.Multiplier = TBRmultiplier.Value / rescaleMultiplier;
                WeightChanged?.Invoke(this, e);
            }
        }

        private void TBR_MouseWheel(object sender, EventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }
}
