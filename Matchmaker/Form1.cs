using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matchmaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly List<Player> players = new List<Player>();
        readonly List<Day> history = new List<Day>();
        readonly Weights weights = new Weights();

        Day generatedDay;
        Day fixedMatchesDay;

        #region save and load

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.FileMain))
                Properties.Settings.Default.FileMain = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bowls matchmaker\\file.bmm";
            ReadWriteMainFile.Input(Properties.Settings.Default.FileMain, players, history, weights);

            SCBfixedmatchplayers.SortBy = (obj, str) => Tools.RelevanceToSearch(obj as Player, str);
            SetUpHTMLscripting();
            RefreshPageMatchesPlayers();
            DoBindingStuff();
            LoadSettings();

            if (players.Count == 0) BTNmainPlayers.PerformClick();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save();
        }

        void Save()
        {
            ReadWriteMainFile.Output(Properties.Settings.Default.FileMain, players, history, weights);
        }

        void SetUpHTMLscripting()
        {
            HTMLscripter scripterNewGames = new HTMLscripter();
            scripterNewGames.PlayerClickedOn += ScripterNewGames_PlayerClickedOn;
            scripterNewGames.RinkChanged += ScripterNewGames_RinkChanged;
            scripterNewGames.DateChanged += Scripter_DateChanged;
            WEBnewgames.ObjectForScripting = scripterNewGames;

            HTMLscripter scripterFixGames = new HTMLscripter();
            scripterFixGames.PlayerClickedOn += ScripterFixGames_PlayerClickedOn;
            scripterFixGames.RinkChanged += ScripterFixGames_RinkChanged;
            scripterFixGames.DateChanged += Scripter_DateChanged;
            scripterFixGames.SizeChanged += ScripterFixGames_SizeChanged;
            scripterFixGames.MatchDeleted += ScripterFixGames_MatchDeleted;
            WEBfixmatches.ObjectForScripting = scripterFixGames;

            DisplayFixedMatches();
            TBXhtml.Text = HTMLdocument.format;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region data binding

        void DoBindingStuff()
        {
            RefreshFullListOfPlayers();
            RefreshHistoryList();

            PositionPrimary.DataSource = Enums.Positions;
            PositionSecondary.DataSource = Enums.PositionsIncludingNone;
            GradePrimary.DataSource = Enums.Grades;
            GradeSecondary.DataSource = Enums.Grades;
            preferredTeamSizesDataGridViewTextBoxColumn.DataSource = Enums.TeamSizes;

            BindWeights();
        }

        void RefreshHistoryList()
        {
            LBXhistory.Items.Clear();
            foreach (Day day in history.Reverse<Day>())
                LBXhistory.Items.Add(day);
        }

        private void PlayerBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            Player player = new Player { ID = Tools.UniqueRandomInt(this.players) };
            if (!RBNvisitor.Checked) player.TagNumber = Tools.NextTagNumber(this.players);
            e.NewObject = player;
            this.players.Add(player);
        }

        public void RefreshPageMatchesPlayers(bool isTyping = false)
        {
            var relevantPlayers = players.Where(player => Tools.Filter(player, TBXfilterPagePlayers.Text));
            if (isTyping) relevantPlayers = relevantPlayers.OrderBy(player => Tools.RelevanceToSearch(player, TBXfilterPagePlayers.Text));

            CLBpagePlayers.Items.Clear();
            foreach (Player player in relevantPlayers)
                CLBpagePlayers.Items.Add(player, playersSelectedForDay.Contains(player));
        }

        private void AddAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLBpagePlayers.Items.Count; i++)
                CLBpagePlayers.SetItemChecked(i, true);
        }

        void RefreshFullListOfPlayers()
        {
            RefreshPlayerFilters();

            SCBfixedmatchplayers.SetAllItems(players);
        }

        void BindWeights()
        {
            WVWpairsplaytogether.BindToWeight(weights.PairPlayedTogetherInTeam);
            WVWenemies.BindToWeight(weights.PairPlayedTogetherAgainstEachOther);
            WVWmainposition.BindToWeight(weights.IncorrectPosition);
            WVWsecondaryposition.BindToWeight(weights.SecondaryPosition);
            WVWbadpositionbadgrade.BindToWeight(weights.BadPositionForBadGrade);
            WVWunbalancedplayers.BindToWeight(weights.UnbalancedPlayers);
            WVWunbalancedteams.BindToWeight(weights.UnbalancedTeams);
            WVWteamsize.BindToWeight(weights.IncorrectTeamSize);
        }

        #endregion

        #region navigation

        private void BTNmainMatch_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainMatch;

            if (tabControlMatches.SelectedTab == pageViewMatch)
                tabControlMatches.SelectedTab = pageConfirmBeforeMatches;
            if (tabControlMatches.SelectedTab == pagePlayers)
                RefreshPageMatchesPlayers();
        }

        private void BTNsettings_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainSettings;
        }

        private void BTNmainPlayers_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainPlayers;
        }

        private void BTNmainhistory_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainHistory;
            if (LBXhistory.SelectedItems.Count == 0 && LBXhistory.Items.Count > 0)
                LBXhistory.SelectedIndex = 0;
        }

        private void BTNplayers_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainMatch;
            tabControlMatches.SelectedTab = pagePlayers;
            RefreshPageMatchesPlayers();
        }

        private void BTNfixedMatches_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainMatch;
            tabControlMatches.SelectedTab = pageFixedMatches;
        }

        private void BTNconfirmbeforecreating_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainMatch;
            tabControlMatches.SelectedTab = pageConfirmBeforeMatches;
            PickNumMatchesIfNotAlreadySelected();
            ReloadConfirmationConsole();
        }

        private void BTNnewgame_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = pageMainMatch;
            tabControlMatches.SelectedTab = pageViewMatch;
            BTNnewgamesgo.PerformClick();
        }

        #endregion

        #region select players for day

        readonly HashSet<Player> playersSelectedForDay = new HashSet<Player>();

        private void TBXfilterPagePlayers_TextChanged(object sender, EventArgs e)
        {
            RefreshPageMatchesPlayers(isTyping: true);
        }

        private void BTNsortnamematches_Click(object sender, EventArgs e)
        {
            players.Sort(Tools.PlayerCompare);
            CLBpagePlayers.FormatString = "%name - %position";
            RefreshPageMatchesPlayers();
        }

        private void BTNsorttagmatches_Click(object sender, EventArgs e)
        {
            players.Sort(Tools.TagNumberCompare);
            CLBpagePlayers.FormatString = "%tag - %name - %position";
            RefreshPageMatchesPlayers();
        }

        private void CLBpagePlayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Player player = CLBpagePlayers.Items[e.Index] as Player;
            if (e.NewValue == CheckState.Checked)
                playersSelectedForDay.Add(player);
            else
            {
                playersSelectedForDay.Remove(player);
                if (fixedMatchesDay != null)
                    foreach (Match match in fixedMatchesDay.matches)
                        foreach (Team team in match.teams)
                            for (int position = 0; position < Team.MaxSize; position++)
                                if (team.players[position] == player)
                                    team.players[position] = null;
            }
            ListOfPlayersChanges();
        }

        private void BTNimportplayersforday_Click(object sender, EventArgs e)
        {
            Day recentDayInHistory = history.LastOrDefault();
            if (recentDayInHistory != null && !string.IsNullOrEmpty(recentDayInHistory.date))
                OFDplayersinday.FileName = Tools.GuessFilename(recentDayInHistory) + ".txt";
            if (OFDplayersinday.ShowDialog() == DialogResult.Cancel) return;
            ReadWriteTable.ImportPlayerForDay(OFDplayersinday.FileName, players, playersSelectedForDay);
            RefreshPageMatchesPlayers();
        }

        #endregion

        #region fixed matches

        private void BTNaddfixedmatch_Click(object sender, EventArgs e)
        {
            if (fixedMatchesDay == null)
            {
                fixedMatchesDay = new Day
                {
                    date = DateTime.Now.ToString("d", CultureInfo.CurrentCulture)
                };
            }
            fixedMatchesDay.matches.Add(new Match(3, true));
            DisplayFixedMatches();
        }

        void DisplayFixedMatches()
        {
            if (fixedMatchesDay == null)
                WEBfixmatches.DocumentText = "Press \"Add Match\" to get started";
            else
                WEBfixmatches.DocumentText = HTMLdocument.GenerateDay(fixedMatchesDay, HTMLmode.FixMatches);

            SCBfixedmatchplayers.SetAllItems(players.Except(Tools.PlayersInDay(fixedMatchesDay)));
        }

        int swapPlayerIndexForFixing = -1;
        private void ScripterFixGames_PlayerClickedOn(int matchIndex, int teamIndex, int position)
        {
            int playerIndex = Swap.CreatePlayerIndex(matchIndex, teamIndex, position);
            Player player = fixedMatchesDay.matches[matchIndex].teams[teamIndex].players[position];

            if (swapPlayerIndexForFixing != -1)
            {
                Swap swap = new Swap(swapPlayerIndexForFixing, playerIndex, fixedMatchesDay);
                swap.DoSwap();

                swapPlayerIndexForFixing = -1;
                toolStripStatusLabel1.Text = $"Swapped {swap.Player1?.Name ?? "[empty slot]"} with {swap.Player2?.Name ?? "[empty slot]"}";

                DisplayFixedMatches();
            }
            else if (SCBfixedmatchplayers.SelectedItem != null)
            {
                Player addedPlayer = SCBfixedmatchplayers.SelectedItem as Player;
                if (addedPlayer != null)
                    playersSelectedForDay.Add(addedPlayer);
                fixedMatchesDay.matches[matchIndex].teams[teamIndex].players[position] = addedPlayer;
                SCBfixedmatchplayers.SelectedItem = player;

                DisplayFixedMatches();
                ListOfPlayersChanges();
            }
            else
            {
                swapPlayerIndexForFixing = playerIndex;
                toolStripStatusLabel1.Text = $"Swapping {player?.Name ?? "[empty slot]"}...";
            }
        }

        private void Scripter_DateChanged(string date)
        {
            if (fixedMatchesDay != null)
                fixedMatchesDay.date = date;
            if (generatedDay != null)
                generatedDay.date = date;
        }

        private void ScripterFixGames_RinkChanged(int matchIndex, string value)
        {
            fixedMatchesDay.matches[matchIndex].rink = value;
        }

        private void ScripterFixGames_SizeChanged(int matchIndex, int value)
        {
            Match match = fixedMatchesDay.matches[matchIndex];
            match.SetTeamSize(value);
            foreach (Team team in match.teams)
                for (int position = 0; position < Team.MaxSize; position++)
                    if (!team.PositionShouldBeFilled((Position)position))
                        team.players[position] = null;
            swapPlayerIndexForFixing = -1;
            toolStripStatusLabel1.Text = $"Changed a match to {EnumParser.NameOfTeamSize(value)}";
            DisplayFixedMatches();
        }

        private void ScripterFixGames_MatchDeleted(int matchIndex)
        {
            fixedMatchesDay.matches.RemoveAt(matchIndex);
            DisplayFixedMatches();
        }

        private void SCBfixedmatchplayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A player was selected
            if (!(SCBfixedmatchplayers.SelectedItem is Player addedPlayer)) return;
            // One of the positions was already selected
            if (swapPlayerIndexForFixing == -1) return;
            // The position that was already selected did not have a player there
            Swap.GetIndiciesForPlayerIndex(swapPlayerIndexForFixing, out int matchIndex, out int teamIndex, out int position);
            if (fixedMatchesDay.matches[matchIndex].teams[teamIndex].players[position] != null) return;

            // Move the selected player to the position
            playersSelectedForDay.Add(addedPlayer);
            fixedMatchesDay.matches[matchIndex].teams[teamIndex].players[position] = addedPlayer;

            // Deselect the player and refresh the screen
            swapPlayerIndexForFixing = -1;
            SCBfixedmatchplayers.SelectedItem = null;
            DisplayFixedMatches();
            ListOfPlayersChanges();
        }

        #endregion

        #region confirmation before creating day

        void ListOfPlayersChanges()
        {
            string str = $"{playersSelectedForDay.Count} players have been selected. ";
            if (fixedMatchesDay != null)
            {
                int fixedPlayerCount = Tools.PlayersInDay(fixedMatchesDay).Count();
                int freePlayerCount = playersSelectedForDay.Count - fixedPlayerCount;
                int expectedFixedPlayerCount = fixedMatchesDay.matches.Sum(match => match.Size * 2);
                if (fixedPlayerCount > 0 && expectedFixedPlayerCount < playersSelectedForDay.Count)
                    str += $"{freePlayerCount} selected for regular matches and {fixedPlayerCount} selected for fixed matches. ";
            }
            toolStripStatusLabel1.Text = str;

            NUDnumMatches_ValueChanged(null, null);
        }

        void ReloadConfirmationConsole()
        {
            GetNumPlayers(out HashSet<Player> playersFree, out _, out _, out _);
            List<Player>[] playersPerPosition = new List<Player>[Team.MaxSize];
            for (int position = 0; position < Team.MaxSize; position++)
                playersPerPosition[position] = new List<Player>();
            foreach (Player player in playersFree)
                playersPerPosition[(int)player.PositionPrimary].Add(player);
            for (int position = 0; position < Team.MaxSize; position++)
                playersPerPosition[position].Sort(Tools.PlayerCompare);

            SuspendLayout();
            TBXconfirmConsole.Clear();
            TBXconfirmConsole.Text += $"{Tools.PlayersInDay(fixedMatchesDay).Count()} players in fixed matches {Environment.NewLine}";
            foreach (Player player in Tools.PlayersInDay(fixedMatchesDay))
            {
                TBXconfirmConsole.Text += $"  {player} {Environment.NewLine}";
            }
            TBXconfirmConsole.Text += $"{playersFree.Count} players that need to be placed {Environment.NewLine}";
            for (int position = 0; position < Team.MaxSize; position++)
            {
                if ((Position)position == Position.Third && playersPerPosition[position].Count == 0) continue;
                TBXconfirmConsole.Text += $"  {playersPerPosition[position].Count} {((Position)position).ToUserFriendlyStringPlural()} {Environment.NewLine}";
                foreach (Player player in playersPerPosition[position])
                {
                    TBXconfirmConsole.Text += $"    {player} {Environment.NewLine}";
                }
            }
            ResumeLayout();
        }

        private void NUDnumMatches_ValueChanged(object sender, EventArgs e)
        {
            GetNumPlayers(out HashSet<Player> playersFree, out int numPairs, out int numTrips, out int numFours);
            int numPlayersByMatch = (numPairs * 2 + numTrips * 3 + numFours * 4) * 2;
            if (numPlayersByMatch == playersFree.Count)
            {
                LBLnummatcheswarning.Text = "";
            }
            else
            {
                LBLnummatcheswarning.Text = $"You have selected {playersFree.Count} players, but you have enough matches for {numPlayersByMatch} people";
            }
        }

        private void BTNmaxpairs_Click(object sender, EventArgs e)
        {
            MaxNumMatches(2);
        }

        private void BTNmaxtrips_Click(object sender, EventArgs e)
        {
            MaxNumMatches(3);
        }

        private void BTNmaxfours_Click(object sender, EventArgs e)
        {
            MaxNumMatches(4);
        }

        void MaxNumMatches(int preferredSize)
        {
            Tools.PickNumGamesForPlayers(playersSelectedForDay.Count - Tools.PlayersInDay(fixedMatchesDay).Count(), preferredSize, out int numPairs, out int numTrips, out int numFours);
            NUDpairs.Value = numPairs;
            NUDtriples.Value = numTrips;
            NUDfours.Value = numFours;
        }

        void PickNumMatchesIfNotAlreadySelected()
        {
            GetNumPlayers(out HashSet<Player> playersFree, out int numPairs, out int numTrips, out int numFours);
            if (numPairs == 0 && numTrips == 0 && numFours == 0)
            {
                MaxNumMatches(3);
            }
            else if (fixedMatchesDay != null && numPairs * 4 + numTrips * 6 + numFours * 8 == playersFree.Count + fixedMatchesDay.matches.Sum(match => match.Size * 2))
            {
                foreach (Match match in fixedMatchesDay.matches)
                {
                    switch (match.Size)
                    {
                        case 2: numPairs--; break;
                        case 3: numTrips--; break;
                        case 4: numFours--; break;
                    }
                }
                if (numPairs >= 0 && numTrips >= 0 && numFours >= 0)
                {
                    NUDpairs.Value = numPairs;
                    NUDtriples.Value = numTrips;
                    NUDfours.Value = numFours;
                }
            }
        }

        #endregion

        #region create matches

        bool GetParameters(out DayGeneratorParameters parameters, bool validate)
        {
            parameters = null;

            GetNumPlayers(out HashSet<Player> playersFree, out int numPairs, out int numTrips, out int numFours);
            if (validate)
            {
                for (int matchIndex = 0; matchIndex < fixedMatchesDay?.matches.Count; matchIndex++)
                {
                    Match match = fixedMatchesDay.matches[matchIndex];
                    int nullsWhereThereShouldBePlayers = 0;
                    foreach (Team team in match.teams)
                        for (int position = 0; position < Team.MaxSize; position++)
                            if (team.players[position] == null && team.PositionShouldBeFilled((Position)position))
                                nullsWhereThereShouldBePlayers++;
                    if (nullsWhereThereShouldBePlayers == 2 * match.Size)
                        fixedMatchesDay.matches.RemoveAt(matchIndex--);
                    else if (nullsWhereThereShouldBePlayers > 0)
                    {
                        BTNfixedMatches.PerformClick();
                        MessageBox.Show($"Please make sure that all the positions in the fixed matches have been filled");
                        return false;
                    }
                }

                if (playersSelectedForDay.Count == 0)
                {
                    BTNplayers.PerformClick();
                    MessageBox.Show("Please select players", "Not enough players");
                    return false;
                }

                if (playersFree.Count < 4)
                {
                    BTNplayers.PerformClick();
                    MessageBox.Show("Please select more players", "Not enough players");
                    return false;
                }

                if (playersFree.Count % 2 == 1)
                {
                    BTNplayers.PerformClick();
                    MessageBox.Show("Please select an even number of players for this day", "Uneven players");
                    return false;
                }

                if ((numPairs * 2 + numTrips * 3 + numFours * 4) * 2 != playersFree.Count)
                {
                    BTNconfirmbeforecreating.PerformClick();
                    MessageBox.Show("Please make sure that the number of matches is correct for the number of players", "Wrong number of players");
                    return false;
                }
            }

            parameters = new DayGeneratorParameters
            {
                players = new List<Player>(playersFree),
                history = history,
                weights = weights,
                numTeamSizes = new int[] { numPairs, numTrips, numFours },
            };
            return true;
        }

        DayGeneratorParameters GetParameters()
        {
            GetParameters(out var result, false);
            return result;
        }

        void GetNumPlayers(out HashSet<Player> playersFree, out int numPairs, out int numTrips, out int numFours)
        {
            playersFree = new HashSet<Player>(playersSelectedForDay.Except(Tools.PlayersInDay(fixedMatchesDay)));
            numPairs = (int)NUDpairs.Value;
            numTrips = (int)NUDtriples.Value;
            numFours = (int)NUDfours.Value;
        }

        private void BTNnewgamesgo_Click(object sender, EventArgs e)
        {
            if (!GetParameters(out var parameters, true))
            {
                return;
            }

            panel5.Enabled = false;
            TBXnewdayConsole.Clear();
            WEBnewgames.DocumentText = "loading...";
            backgroundWorker1.RunWorkerAsync(parameters);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DayGenerator generator = new DayGenerator(e.Argument as DayGeneratorParameters);
            using ProgressUpdater progressUpdater = new ProgressUpdater(generator, this, toolStripProgressBar1, toolStripStatusLabel1, timer1);
            generatedDay = generator.Generate();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (fixedMatchesDay == null)
            {
                generatedDay.date = DateTime.Now.ToString("d", CultureInfo.CurrentCulture);
            }
            else
            {
                generatedDay.date = fixedMatchesDay.date;
                foreach (Match match in fixedMatchesDay.matches)
                    generatedDay.matches.Add(match);
            }
            DisplayGeneratedDay();
            RewritePenalties();
            panel5.Enabled = true;
        }

        private void BTNnewgameProblems_Click(object sender, EventArgs e)
        {
            SCTnewgames.Panel2Collapsed = !SCTnewgames.Panel2Collapsed;
            BTNnewgameProblems.Text = SCTnewgames.Panel2Collapsed ? "Show Problems" : "Hide Problems";
        }

        private void BTNimprove_Click(object sender, EventArgs e)
        {
            var swap = new DayImprover(generatedDay, GetParameters()).DoOneImprovement();
            if (swap == null)
            {
                toolStripStatusLabel1.Text = "Could not find an improvement";
            }
            else
            {
                toolStripStatusLabel1.Text = $"Swapped {swap.Player1} with {swap.Player2}"; 
                DisplayGeneratedDay();
                RewritePenalties();
            }
        }

        #endregion

        #region confirming matches

        void RewritePenalties()
        {
            if (generatedDay == null) return;

            SuspendLayout();
            TBXnewdayConsole.Clear();
            TBXnewdayConsole.Text += $"Total score: {Math.Round(generatedDay.matches.Sum(match => match.penalties.Sum(penalty => penalty.Score())), 3)}{Environment.NewLine}";
            foreach (var Penalty in generatedDay.matches.SelectMany(match => match.penalties).OrderByDescending(penalty => penalty.Score()))
                TBXnewdayConsole.Text += Math.Round(Penalty.Score(), 3) + "\t" + Penalty.Reason(history) + Environment.NewLine;
            if (generatedDay.matches.All(match => match.penalties.Count == 0))
                TBXnewdayConsole.Text += "Perfect score!" + Environment.NewLine + "I could not find anything wrong with these matches" + Environment.NewLine;
            TBXnewdayConsole.SelectionStart = 0;
            TBXnewdayConsole.ScrollToCaret();
            ResumeLayout();
        }

        void DisplayGeneratedDay()
        {
            WEBnewgames.DocumentText = HTMLdocument.GenerateDay(generatedDay, HTMLmode.ConfirmMatches);
        }

        int swapPlayerIndex = -1;

        private void ScripterNewGames_PlayerClickedOn(int matchIndex, int teamIndex, int position)
        {
            int playerIndex = Swap.CreatePlayerIndex(matchIndex, teamIndex, position);

            if (swapPlayerIndex == -1)
            {
                swapPlayerIndex = playerIndex;
                toolStripStatusLabel1.Text = $"Swapping {generatedDay.matches[matchIndex].teams[teamIndex].players[position].Name}...";
            }
            else
            {
                Swap swap = new Swap(swapPlayerIndex, playerIndex, generatedDay);
                swap.DoSwap();

                CachedPenalties penalties = new CachedPenalties(GetParameters());
                penalties.RecalculateScore(generatedDay);

                swapPlayerIndex = -1;
                toolStripStatusLabel1.Text = $"Swapped {swap.Player1.Name} with {swap.Player2.Name}";

                RewritePenalties(); 
                DisplayGeneratedDay();
                if (swap.match1.isFixed || swap.match2.isFixed)
                    DisplayFixedMatches();
            }
        }

        private void ScripterNewGames_RinkChanged(int matchIndex, string value)
        {
            generatedDay.matches[matchIndex].rink = value;
        }

        int scrollTop = -1;
        private void WEBnewgames_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            scrollTop = WEBnewgames.Document?.GetElementsByTagName("HTML")[0].ScrollTop ?? -1;
        }

        private void WEBnewgames_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (scrollTop != -1 && WEBnewgames.ReadyState == WebBrowserReadyState.Complete)
            {
                WEBnewgames.Document.GetElementsByTagName("HTML")[0].ScrollTop = scrollTop;
                scrollTop = -1;
            }
        }

        private void BTNnewgamesave_Click(object sender, EventArgs e)
        {
            if (generatedDay == null) return;

            generatedDay.matches.Sort(Tools.MatchRinkCompare);

            history.Add(generatedDay);
            Save();
            
            generatedDay = null;
            fixedMatchesDay = null;
            playersSelectedForDay.Clear();
            NUDpairs.Value = 0;
            NUDtriples.Value = 0;
            NUDfours.Value = 0;

            RefreshFullListOfPlayers();
            DisplayFixedMatches();
            RefreshHistoryList();
            tabControlMatches.SelectedTab = pagePlayers;
            BTNmainhistory.PerformClick();
        }

        #endregion

        #region settings screen

        void LoadSettings()
        {
            BFFmain.FileName = Properties.Settings.Default.FileMain;
            BFFhtml.FileName = Properties.Settings.Default.FileHTML;

            HTMLdocument.ReloadFormat(Properties.Settings.Default.FileHTML);
        }

        private void WVW_WeightChanged(object sender, EventArgs e)
        {
            RewritePenalties();
        }

        private void BTNresetWeights_Click(object sender, EventArgs e)
        {
            weights.ResetToDefaults();
            foreach (Control control in FLPweights.Controls)
                if (control is WeightView view)
                    view.UpdateFromWeight();
        }

        private void BFFmain_FileNameChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FileMain = BFFmain.FileName;
            Properties.Settings.Default.Save();
        }

        private void BFFhtml_FileNameChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FileHTML = BFFhtml.FileName;
            Properties.Settings.Default.Save();
            HTMLdocument.ReloadFormat(Properties.Settings.Default.FileHTML);
            // todo: update even when the file name doesn't change
        }

        private void BTNseedefaulthtml_Click(object sender, EventArgs e)
        {
            if (SFDhtml.ShowDialog() == DialogResult.Cancel) return;
            using StreamWriter streamWriter = new StreamWriter(SFDhtml.FileName);
            streamWriter.Write(Properties.Resources.table);
            Process.Start("notepad.exe", SFDhtml.FileName);
        }

        #endregion

        #region players list screen

        private void RBNmemberorvisitor_CheckedChanged(object sender, EventArgs e)
        {
            PlayerListFilter = m => true;
        }

        private void RBNmember_CheckedChanged(object sender, EventArgs e)
        {
            PlayerListFilter = m => !m.Visitor;
        }

        private void RBNvisitor_CheckedChanged(object sender, EventArgs e)
        {
            PlayerListFilter = m => m.Visitor;
        }

        Func<Player, bool> _filter = p => true;
        Func<Player, bool> PlayerListFilter
        {
            get => _filter;
            set
            {
                _filter = value;
                RefreshPlayerFilters();
            }
        }

        void RefreshPlayerFilters()
        {
            playerBindingSource.DataSource = players.Where(PlayerListFilter).ToList();
        }

        private void BTNsortnameplayers_Click(object sender, EventArgs e)
        {
            players.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
            RefreshPlayerFilters();
        }

        private void BTNsorttagplayers_Click(object sender, EventArgs e)
        {
            players.Sort(Tools.TagNumberCompare);
            RefreshPlayerFilters();
        }

        private void BTNaddplayer_Click(object sender, EventArgs e)
        {
            Player player = new Player()
            {
                ID = Tools.UniqueRandomInt(players),
                TagNumber = RBNvisitor.Checked ? "" : Tools.NextTagNumber(players),
            };
            players.Add(player);
            RefreshPlayerFilters();
            int rowIndex = (playerBindingSource.DataSource as List<Player>).IndexOf(player);
            dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1];
            dataGridView1.BeginEdit(true);
        }

        private void DeletePlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var indicies = new List<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                if (!indicies.Contains(cell.RowIndex))
                    indicies.Add(cell.RowIndex);
            List<Player> playersInGrid = playerBindingSource.DataSource as List<Player>;
            List<Player> selectedPlayers = new List<Player>();
            foreach (int rowIndex in indicies)
                selectedPlayers.Add(playersInGrid[rowIndex]);

            // Confirmation
            string listPlayersForMessage = Tools.ShortListDescription(selectedPlayers);
            if (MessageBox.Show("Are you sure you want to delete " + listPlayersForMessage + "?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No) return;

            DeleteData.DeletePlayers(selectedPlayers, players, history);

            RefreshPlayerFilters();
        }

        private void BTNexcelimport_Click(object sender, EventArgs e)
        {
            if (OFDexcel.ShowDialog() == DialogResult.Cancel) return;
            SFDexcel.FileName = OFDexcel.FileName;

            if (ReadWriteTable.ImportPlayerDetails(OFDexcel.FileName, players))
            {
                RefreshFullListOfPlayers();
                toolStripStatusLabel1.Text = "Imported from " + SFDexcel.FileName;
            }
            else
            {
                MessageBox.Show("Something went wrong while importing", "Error");
            }
        }

        private void BTNexcelexport_Click(object sender, EventArgs e)
        {
            if (SFDexcel.ShowDialog() == DialogResult.Cancel) return;
            OFDexcel.FileName = SFDexcel.FileName;

            if (ReadWriteTable.ExportPlayerDetails(SFDexcel.FileName, players))
            {
                toolStripStatusLabel1.Text = "Saved to " + SFDexcel.FileName;
                if (MessageBox.Show("Would you like to open the file now?", "Saved file", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start(OFDexcel.FileName);
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Error when exporting to " + SFDexcel.FileName;
                MessageBox.Show("Something went wrong while saving", "Error");
            }
        }

        #endregion

        #region view history screen

        private void LBXhistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Day> days = new List<Day>();
            foreach (Day day in LBXhistory.SelectedItems)
                days.Add(day);
            WEBhistory.DocumentText = HTMLdocument.GenerateDays(days);
            deleteToolStripMenuItem.Text = days.Count == 1 ? "Delete day" : $"Delete {days.Count} days";
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBXhistory.SelectedItems.Count == 0) return;

            // Confirmation
            string listDaysForMessage = Tools.ShortListDescription(LBXhistory.SelectedItems);
            if (MessageBox.Show($"Are you sure you want to delete {listDaysForMessage}?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No) return;

            List<int> indices = new List<int>();
            foreach(int selectedIndex in LBXhistory.SelectedIndices)
                indices.Add(history.Count - 1 - selectedIndex);

            DeleteData.DeleteFromHistory(history, indices);

            toolStripStatusLabel1.Text = $"Deleted " + listDaysForMessage;
            RefreshHistoryList();
            LBXhistory.SelectedIndices.Clear();
            if (LBXhistory.Items.Count > 0)
                LBXhistory.SelectedIndices.Add(0);
            LBXhistory_SelectedIndexChanged(null, null);
        }

        private void BTNprintHistory_Click(object sender, EventArgs e)
        {
            WEBhistory.ShowPrintDialog();
        }

        private void BTNexportplayersinday_Click(object sender, EventArgs e)
        {
            foreach (Day day in LBXhistory.SelectedItems)
            {
                SFDplayersinday.FileName = Tools.GuessFilename(day) + ".txt";
                if (SFDplayersinday.ShowDialog() == DialogResult.Cancel) continue;
                ReadWriteTable.ExportPlayersFromDay(SFDplayersinday.FileName, day);
            }
        }

        private void EditHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SCTNhistory.Panel2Collapsed)
            {
                SCTNhistory.Panel2Collapsed = false;
                WEBhistory.IsWebBrowserContextMenuEnabled = true;
            }
            else
            {
                SCTNhistory.Panel2Collapsed = true;
                WEBhistory.IsWebBrowserContextMenuEnabled = false;
            }
        }

        private void TBXhtml_Leave(object sender, EventArgs e)
        {
            HTMLdocument.format = TBXhtml.Text;
            LBXhistory_SelectedIndexChanged(null, null);
        }

        #endregion
    }
}
