using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace CustomControls
{
    class WaterMarkTextBox : TextBox
    {
        private Font oldFont = null;
        private Boolean waterMarkTextEnabled = false;

        #region Attributes
        private Color _waterMarkColor = Color.Gray;
        public Color WaterMarkColor
        {
            get { return _waterMarkColor; }
            set { _waterMarkColor = value; Invalidate();/*thanks to Bernhard Elbl for Invalidate()*/ }
        }

        private string _waterMarkText = "Water Mark";
        public string WaterMarkText
        {
            get { return _waterMarkText; }
            set { _waterMarkText = value; Invalidate(); }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                this.SetStyle(ControlStyles.UserPaint, false);
                base.Font = value;
                this.SetStyle(ControlStyles.UserPaint, this.waterMarkTextEnabled);
            }
        }
        #endregion

        //Default constructor
        public WaterMarkTextBox()
        {
            JoinEvents(true);
        }

        //Override OnCreateControl ... thanks to  "lpgray .. codeproject guy"
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            WaterMark_Toggel(null, null);
        }

        //Override OnPaint
        protected override void OnPaint(PaintEventArgs args)
        {
            // Use the same font that was defined in base class
            System.Drawing.Font drawFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
            //Create new brush with gray color or 
            SolidBrush drawBrush = new SolidBrush(WaterMarkColor);//use Water mark color
            //Draw Text or WaterMark
            args.Graphics.DrawString((waterMarkTextEnabled ? WaterMarkText : Text), drawFont, drawBrush, new PointF(0.0F, 0.0F));
            base.OnPaint(args);
        }

        private void JoinEvents(Boolean join)
        {
            if (join)
            {
                this.TextChanged += new System.EventHandler(this.WaterMark_Toggel);
                this.LostFocus += new System.EventHandler(this.WaterMark_Toggel);
                this.FontChanged += new System.EventHandler(this.WaterMark_FontChanged);
                //No one of the above events will start immeddiatlly 
                //TextBox control still in constructing, so,
                //Font object (for example) couldn't be catched from within WaterMark_Toggle
                //So, call WaterMark_Toggel through OnCreateControl after TextBox is totally created
                //No doupt, it will be only one time call

                //Old solution uses Timer.Tick event to check Create property
            }
        }

        private void WaterMark_Toggel(object sender, EventArgs args)
        {
            if (this.Text.Length <= 0)
                EnableWaterMark();
            else
                DisbaleWaterMark();
        }

        private void EnableWaterMark()
        {
            //Save current font until returning the UserPaint style to false (NOTE: It is a try and error advice)
            oldFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
            //Enable OnPaint event handler
            this.SetStyle(ControlStyles.UserPaint, true);
            this.waterMarkTextEnabled = true;
            //Triger OnPaint immediatly
            Refresh();
        }

        private void DisbaleWaterMark()
        {
            //Disbale OnPaint event handler
            this.waterMarkTextEnabled = false;
            this.SetStyle(ControlStyles.UserPaint, false);
            //Return back oldFont if existed
            if (oldFont != null)
                this.Font = new System.Drawing.Font(oldFont.FontFamily, oldFont.Size, oldFont.Style, oldFont.Unit);
        }

        private void WaterMark_FontChanged(object sender, EventArgs args)
        {
            if (waterMarkTextEnabled)
            {
                oldFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
                Refresh();
            }
        }
    }

    class ClearTrackBar : TrackBar
    {
        protected override void OnCreateControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            if (Parent != null)
                BackColor = Parent.BackColor;

            base.OnCreateControl();
        }
    }

    class TabControlWithoutHeader : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode)
                m.Result = (IntPtr)1;
            else
                base.WndProc(ref m);
        }
    }

    class AnchorFlowLayoutPanel : FlowLayoutPanel
    {
        public AnchorFlowLayoutPanel()
        {
            this.AutoScroll = true;
            this.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.WrapContents = false;

            this.Resize += AnchorFlowLayoutPanel_Resize;
            this.PaddingChanged += AnchorFlowLayoutPanel_Resize;
            SetChildrenWidth();
        }

        private void AnchorFlowLayoutPanel_Resize(object sender, EventArgs e)
        {
            SetChildrenWidth();
        }

        private void SetChildrenWidth()
        {
            foreach (Control control in Controls)
                if (!control.Anchor.HasFlag(AnchorStyles.Left) && !control.Anchor.HasFlag(AnchorStyles.Right))
                    control.Width = this.Width - control.Margin.Left - this.Padding.Left - control.Margin.Right - this.Padding.Right - (VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
        }
    }

    public class SearchableComboBox : ComboBox
    {
        private ObjectCollection _allItems;
        public ObjectCollection AllItems => _allItems ??= new ObjectCollection(this);

        public Func<object, string, int> SortBy;

        protected override void OnTextUpdate(EventArgs e)
        {
            RedoSearch(true);
            base.OnTextUpdate(e);
        }

        private void RedoSearch(bool openDropDown)
        {
            int cursorPosition = this.SelectionStart;

            var itemsToSearch = AllItems.Cast<object>();
            if (!string.IsNullOrEmpty(this.Text))
                itemsToSearch = itemsToSearch.Where(x => x.ToString().IndexOf(this.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            if (SortBy != null)
                itemsToSearch = itemsToSearch.OrderBy(x => SortBy(x, this.Text));

            this.Items.Clear();
            foreach (object item in itemsToSearch)
                this.Items.Add(item);

            if (openDropDown)
            {
                this.DroppedDown = true;
                this.SelectionStart = cursorPosition;
                Cursor.Current = Cursors.Default;
            }
        }

        public void SetAllItems(IEnumerable<object> items)
        {
            var selected = this.SelectedItem;
            AllItems.Clear();
            foreach (var item in items)
                AllItems.Add(item);
            RedoSearch(false);
            this.SelectedItem = selected;
        }
    }

    public class BrowseForFile : UserControl
    {
        private readonly Label label1 = new Label()
        {
            AutoSize = true,
            Location = new Point(3, 8),
            Size = new Size(79, 13),
            TabIndex = 0,
            Text = "Browse for File:",
        };
        private readonly TextBox textBox1 = new TextBox()
        {
            Location = new Point(103, 5),
            Size = new Size(359, 20),
            TabIndex = 1,
        };
        private readonly Button button1 = new Button()
        {
            Location = new Point(483, 3),
            Size = new Size(75, 23),
            TabIndex = 2,
            Text = "Browse",
            UseVisualStyleBackColor = true,
        };
        private readonly OpenFileDialog openFileDialog1 = new OpenFileDialog();

        public BrowseForFile()
        {
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Size = new Size(561, 29);

            this.button1.Click += new EventHandler(this.Button_Click);
            this.textBox1.Leave += new EventHandler(this.TextBox_Leave);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            FileName = openFileDialog1.FileName;
            this.FileNameChanged?.Invoke(this, e);
        }

        [Browsable(true)]
        public override string Text
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        public string FileName
        {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user enters a File Name")]
        public event EventHandler FileNameChanged;

        private void TextBox_Leave(object sender, EventArgs e)
        {
            this.FileNameChanged?.Invoke(this, e);
        }
    }
}
