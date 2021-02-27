using CustomControls;
using Matchmaker.Data;
using Matchmaker.DataHandling;
using Matchmaker.UserInterface.StringConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matchmaker.FileOperations
{
    public abstract partial class FormTableImporter : Form
    {
        public FormTableImporter()
        {
            InitializeComponent();
        }

        public FormTableImporter(ITableReader reader) : this()
        {
            this.reader = reader;
            this.SelectedColumns = new int[reader.NumColumns];
            for (int i = 0; i < SelectedColumns.Length; i++)
                SelectedColumns[i] = -1;
        }

        /// <summary>
        /// The object that is used to read from the table
        /// </summary>
        readonly ITableReader reader;
        /// <summary>
        /// An array of possible columns that could be imported
        /// </summary>
        readonly List<string[]> Columns = new List<string[]>();
        /// <summary>
        /// Add a new possible column
        /// </summary>
        protected void AddColumnInfo(params string[] possibleMatches) => Columns.Add(possibleMatches);
        /// <summary>
        /// A map from column indexes to indexes in AllColumns
        /// </summary>
        readonly int[] SelectedColumns;
        /// <summary>
        /// Get the info of a column from its index
        /// </summary>
        protected string[] GetColumn(int index) => Columns[SelectedColumns[index]];

        /// <summary>
        /// Create all the data on the form
        /// </summary>
        public void DrawData()
        {
            // Compile a list of titles that will go at the top of the table
            string[] titles = new string[this.Columns.Count];
            for (int i = 0; i < this.Columns.Count; i++)
                titles[i] = this.Columns[i].First();

            // Create columns
            for (int i = 0; i < reader.NumColumns; i++)
            {
                DataGridViewDropDownHeaderColumn column = new DataGridViewDropDownHeaderColumn
                {
                    AllOptions = titles,
                    ReadOnly = true,
                };
                column.HeaderOptionClicked += FormTableImporter_HeaderOptionClicked;
                dataGridView1.Columns.Add(column);
            }

            // Import the data onto the table
            bool firstRow = true;
            while (reader.ReadRow(out string[] row))
            {
                // Add an extra element at the beginning for the checkboxes
                object[] arr = new object[row.Length + 1];
                arr[0] = !firstRow;
                firstRow = false;
                for (int i = 0; i < row.Length; i++)
                    arr[i + 1] = row[i];
                // Add the row to the table
                dataGridView1.Rows.Add(arr);

                // Guess the titles of the columns
                for (int i = 0; i < row.Length; i++)
                    if (SelectedColumns[i] == -1)
                        for (int j = 0; j < this.Columns.Count; j++)
                            foreach (string potentialmatch in this.Columns[j])
                                if (string.Equals(potentialmatch, row[i], StringComparison.OrdinalIgnoreCase))
                                    SelectedColumns[i] = j;
            }

            // Make sure the column titles aren't -1
            for (int i = 0; i < SelectedColumns.Length; i++)
                if (SelectedColumns[i] == -1)
                    SelectedColumns[i] = 0;

            // Display the titles of the columns
            for (int i = 0; i < SelectedColumns.Length; i++)
                dataGridView1.Columns[i + 1].HeaderText = GetColumn(i).First();
        }

        private void FormTableImporter_HeaderOptionClicked(object sender, int index)
        {
            SelectedColumns[(sender as DataGridViewDropDownHeaderColumn).DisplayIndex - 1] = index;
        }

        private void BTNcancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BTNimport_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value.ToString() == Boolean.TrueString)
                {
                    string[] data = new string[SelectedColumns.Length];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = row.Cells[i + 1].Value.ToString();
                    HandleRow(data);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Import a row
        /// </summary>
        /// <param name="data">The strings found on a single row in the table</param>
        public abstract void HandleRow(string[] data);
    }
    public class FormTableImporterPlayer : FormTableImporter
    {
        readonly IList<Player> players;

        public FormTableImporterPlayer(ITableReader reader, IList<Player> players) : base(reader)
        {
            this.players = players;
            AddColumnInfo("Tag Number", "ID", "#", "Number", "Tag");
            AddColumnInfo("Name", "Player");
            AddColumnInfo("Primary Position", "Position", "Primary");
            AddColumnInfo("Primary Grade", "Grade");
            AddColumnInfo("Secondary Position", "Secondary");
            AddColumnInfo("Secondary Grade");
            AddColumnInfo("Primary Position and Grade");
            AddColumnInfo("Secondary Position and Grade");
            AddColumnInfo("Team Size", "Preferred Team Size");
            this.Text = "Import Players";
            DrawData();
        }

        public override void HandleRow(string[] data)
        {
            // Identify:
            Player player = null;
            for (int i = 0; i < data.Length; i++)
            {
                switch (GetColumn(i).First())
                {
                    case "Tag Number":
                        if (player == null && !string.IsNullOrEmpty(data[i]))
                            player = players.FirstOrDefault(p => p.TagNumber.Equals(data[i]));
                        break;
                    case "Name":
                        player = players.FirstOrDefault(p => p.Name.Equals(data[i]));
                        break;
                }
            }
            if (player == null)
            {
                player = new Player { ID = DataCreation.UniqueRandomInt(players) };
                players.Add(player);
            }
            // Import
            for (int i = 0; i < data.Length; i++)
            {
                switch (GetColumn(i).First())
                {
                    case "Tag Number":
                        if (string.IsNullOrEmpty(data[i])) break;
                        player.TagNumber = data[i];
                        break;
                    case "Name":
                        if (string.IsNullOrEmpty(data[i])) break;
                        player.Name = data[i];
                        break;
                    case "Primary Position":
                        player.PositionPrimary = EnumStringConverter.ParsePosition(data[i]);
                        break;
                    case "Primary Grade":
                        player.GradePrimary = EnumStringConverter.ParseGrade(data[i]);
                        break;
                    case "Secondary Position":
                        player.PositionSecondary = EnumStringConverter.ParsePosition(data[i]);
                        break;
                    case "Secondary Grade":
                        player.GradeSecondary = EnumStringConverter.ParseGrade(data[i]);
                        break;
                    case "Primary Position and Grade":
                        player.PreferencePrimary = EnumStringConverter.ParsePositionAndGrade(data[i]);
                        break;
                    case "Secondary Position and Grade":
                        player.PreferenceSecondary = EnumStringConverter.ParsePositionAndGrade(data[i]);
                        break;
                    case "Team Size":
                        player.PreferredTeamSizes = EnumStringConverter.TryParseTeamSize(data[i], out TeamSize ts) ? ts : TeamSize.Any;
                        break;
                }
            }
        }
    }
}
