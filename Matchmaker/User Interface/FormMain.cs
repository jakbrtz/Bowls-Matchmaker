﻿using Matchmaker.Algorithms;
using Matchmaker.Algorithms.Structures;
using Matchmaker.Collections;
using Matchmaker.Data;
using Matchmaker.DataHandling;
using Matchmaker.FileOperations;
using Matchmaker.UserInterface.Controls;
using Matchmaker.UserInterface.StringConverters;
using Matchmaker.UserInterface.Intermediate;
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
using Day = Matchmaker.Data.Day;

namespace Matchmaker.UserInterface
{
    public partial class FormMain : Form
    {
        public FormMain()
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
                Properties.Settings.Default.FileMain = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bowls matchmaker\\data.json";

            SetUpHTMLscripting();
            PrepareComboBoxes();
            LoadSettings();
            
            FileLoad();
        }

        void FileLoad()
        {
            ReadWriteMainFile.Input(Properties.Settings.Default.FileMain, players, history, weights);

            SCBfixedmatchplayers.SortBy = (obj, str) => Search.RelevanceToSearch(obj as Player, str);
            
            RefreshPageMatchesPlayers();
            RefreshFullListOfPlayers();
            RefreshHistoryList();
            BindWeights();

            if (players.Count == 0)
                BTNmainPlayers.PerformClick();
            else
                BTNplayers.PerformClick();
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
            WEBhistory.DocumentText = "";
        }

        #endregion

        #region data binding

        void PrepareComboBoxes()
        {
            PositionPrimary.DataSource = Enums.Positions;
            PositionSecondary.DataSource = Enums.PositionsIncludingNone;
            GradePrimary.DataSource = Enums.Grades;
            GradeSecondary.DataSource = Enums.GradesIncludingNone;
            preferredTeamSizesDataGridViewTextBoxColumn.DataSource = Enums.TeamSizes;

            EnumStringConverter.AddAttributesToEnumsAndStructs();
        }

        void RefreshHistoryList()
        {
            LBXhistory.Items.Clear();
            foreach (Day day in history.Reverse<Day>())
                LBXhistory.Items.Add(day);
        }

        private void PlayerBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            Player player = new Player { ID = DataCreation.UniqueRandomInt(this.players) };
            if (!RBNvisitor.Checked) player.TagNumber = DataCreation.NextTagNumber(this.players);
            e.NewObject = player;
            this.players.Add(player);
        }

        public void RefreshPageMatchesPlayers(bool highlightWhenDone = false)
        {
            Player selected = CurrentlySelectedPlayer();

            CLBpagePlayers.Items.Clear();
            foreach (Player player in players)
                CLBpagePlayers.Items.Add(new PlayerIntermediate(player), playersSelectedForDay.Contains(player));

            if (highlightWhenDone) HighlightPlayer(selected);
        }

        Player CurrentlySelectedPlayer() => (CLBpagePlayers.SelectedItem as PlayerIntermediate)?.player;

        public void HighlightPlayer(Player originalSelection)
        {
            Player selection = originalSelection;

            bool searching = !string.IsNullOrEmpty(TBXfilterPagePlayers.Text);
            bool originalIsGood = originalSelection != null && 
                new PlayerIntermediate(originalSelection).ToString(CLBpagePlayers.FormatString, null).StartsWith(TBXfilterPagePlayers.Text, StringComparison.OrdinalIgnoreCase);

            if (searching && !originalIsGood)
            {
                var bestMatch = Search.GetBestMatch(players, TBXfilterPagePlayers.Text);
                if (bestMatch != null)
                {
                    selection = bestMatch;
                }
            }

            CLBpagePlayers.SelectedIndex = players.IndexOf(selection);
        }

        private void AddAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLBpagePlayers.Items.Count; i++)
                CLBpagePlayers.SetItemChecked(i, true);
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLBpagePlayers.Items.Count; i++)
                CLBpagePlayers.SetItemChecked(i, false);
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
            WVWgoodskipsgetskip.BindToWeight(weights.GoodSkipsGetSkip);
            WVWgoodleadsmoveup.BindToWeight(weights.GoodLeadsMoveUp);
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
            NewGames(false);
        }

        #endregion

        #region select players for day

        readonly HashSet<Player> playersSelectedForDay = new HashSet<Player>();

        private void TBXfilterPagePlayers_TextChanged(object sender, EventArgs e)
        {
            HighlightPlayer(CurrentlySelectedPlayer());
        }
        private void TBXfilterPagePlayers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && CLBpagePlayers.Items.Count > 0 && CLBpagePlayers.SelectedIndex >= 0 && TBXfilterPagePlayers.Text?.Length > 0)
            {
                CLBpagePlayers.SetItemChecked(CLBpagePlayers.SelectedIndex, true);
                TBXfilterPagePlayers.Text = "";
                TBXfilterPagePlayers.SelectionStart = 0; // todo: the control should handle this automatically
            }
        }

        private void BTNsortnamematches_Click(object sender, EventArgs e)
        {
            players.Sort(Sorts.PlayerCompare);
            RefreshPageMatchesPlayers(highlightWhenDone: true); 
        }

        private void BTNsorttagmatches_Click(object sender, EventArgs e)
        {
            players.Sort(Sorts.TagNumberCompare);
            RefreshPageMatchesPlayers(highlightWhenDone: true);
        }

        private void CLBpagePlayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Player player = (CLBpagePlayers.Items[e.Index] as PlayerIntermediate).player;
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
            ReadWriteTable.ImportPlayerForDay(OFDplayersinday.FileName, players, playersSelectedForDay, out List<string> namesNotImported);
            if (namesNotImported.Count == 1)
            {
                MessageBox.Show($"I could not find anyone with the name {namesNotImported[0]}", "Incomplete import");
            }
            else if (namesNotImported.Count > 1)
            {
                string msg = "I could not find anyone with these names:";
                foreach (string name in namesNotImported)
                    msg += "\n" + name;
                MessageBox.Show(msg, "Incomplete import");
            }
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
            fixedMatchesDay.matches.Add(new Match(MatchSize.Triples, true, false));
            DisplayFixedMatches();
        }

        void DisplayFixedMatches()
        {
            if (fixedMatchesDay == null)
                WEBfixmatches.DocumentText = "Press \"Add Match\" to get started";
            else
                WEBfixmatches.DocumentText = HTMLdocument.GenerateDay(fixedMatchesDay, HTMLmode.FixMatches);

            SCBfixedmatchplayers.SetAllItems(players.Except(fixedMatchesDay.Players()));
        }

        private void WEBfixmatches_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            SetWebBrowserZoom(WEBfixmatches, currentZoom);
        }

        int swapPlayerIndexForFixing = -1;
        private void ScripterFixGames_PlayerClickedOn(int matchIndex, int teamIndex, int position)
        {
            int playerIndex = RegularSwap.CreatePlayerIndex(matchIndex, teamIndex, position);
            Player player = fixedMatchesDay.matches[matchIndex].teams[teamIndex].players[position];

            if (swapPlayerIndexForFixing != -1)
            {
                RegularSwap swap = new RegularSwap(swapPlayerIndexForFixing, playerIndex, fixedMatchesDay);
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
            match.SetTeamSize(new MatchSize(value));
            foreach (Team team in match.teams)
                for (int position = 0; position < Team.MaxSize; position++)
                    if (!team.PositionShouldBeFilled((Position)position))
                        team.players[position] = null;
            swapPlayerIndexForFixing = -1;
            toolStripStatusLabel1.Text = $"Changed a match to {EnumStringConverter.NameOfTeamSize(value)}";
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
            RegularSwap.GetIndiciesForPlayerIndex(swapPlayerIndexForFixing, out int matchIndex, out int teamIndex, out int position);
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
                int fixedPlayerCount = fixedMatchesDay.Players().Count();
                int freePlayerCount = playersSelectedForDay.Count - fixedPlayerCount;
                int expectedFixedPlayerCount = fixedMatchesDay.matches.Sum(match => match.Team1.size + match.Team2.size);
                if (fixedPlayerCount > 0 && expectedFixedPlayerCount < playersSelectedForDay.Count)
                    str += $"{freePlayerCount} selected for regular matches and {fixedPlayerCount} selected for fixed matches. ";
            }
            toolStripStatusLabel1.Text = str;

            NUDnumMatches_ValueChanged(null, null);
        }

        void ReloadConfirmationConsole()
        {
            GetNumPlayers(out HashSet<Player> playersFree, out _);
            List<Player>[] playersPerPosition = new List<Player>[Team.MaxSize];
            for (int position = 0; position < Team.MaxSize; position++)
                playersPerPosition[position] = new List<Player>();
            foreach (Player player in playersFree)
                playersPerPosition[(int)player.PositionPrimary].Add(player);
            for (int position = 0; position < Team.MaxSize; position++)
                playersPerPosition[position].Sort(Sorts.PlayerCompare);

            SuspendLayout();
            TBXconfirmConsole.Clear();
            TBXconfirmConsole.Text += $"{fixedMatchesDay.Players().Count()} players in fixed matches {Environment.NewLine}";
            foreach (Player player in fixedMatchesDay.Players())
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
            GetNumPlayers(out HashSet<Player> playersFree, out Counter<MatchSize> numMatchSizes);
            int numPlayersByMatch = Tools.SumOfPlayersInMatchSizes(numMatchSizes);
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
            MaxNumMatches(MatchSize.Pairs);
        }

        private void BTNmaxtrips_Click(object sender, EventArgs e)
        {
            MaxNumMatches(MatchSize.Triples);
        }

        private void BTNmaxfours_Click(object sender, EventArgs e)
        {
            MaxNumMatches(MatchSize.Fours);
        }

        private void BTNmax3v2_Click(object sender, EventArgs e)
        {
            MaxNumMatches(MatchSize.TripVsPair);
        }

        private void BTNmax4v3_Click(object sender, EventArgs e)
        {
            MaxNumMatches(MatchSize.FourVsTrip);
        }

        void MaxNumMatches(MatchSize preferredSize)
        {
            Tools.PickNumGamesForPlayers(playersSelectedForDay.Count - fixedMatchesDay.Players().Count(), preferredSize, out Counter<MatchSize> numMatchSizes);
            SetMatchSizes(numMatchSizes);
        }

        void PickNumMatchesIfNotAlreadySelected()
        {
            GetNumPlayers(out HashSet<Player> playersFree, out var numMatchSizes);
            if (Tools.SumOfPlayersInMatchSizes(numMatchSizes) == 0)
            {
                MaxNumMatches(MatchSize.Triples);
            }
            else if (fixedMatchesDay != null && Tools.SumOfPlayersInMatchSizes(numMatchSizes) == playersFree.Count + fixedMatchesDay.matches.Sum(match => match.Team1.size + match.Team2.size))
            {
                foreach (Match match in fixedMatchesDay.matches)
                {
                    numMatchSizes[match.GetMatchSize()]--;
                }
                if (numMatchSizes.All(kvp => kvp.Value >= 0))
                {
                    SetMatchSizes(numMatchSizes);
                }
            }
        }

        void SetMatchSizes(Counter<MatchSize> numMatchSizes)
        {
            NUDpairs.Value = numMatchSizes[MatchSize.Pairs];
            NUDtriples.Value = numMatchSizes[MatchSize.Triples];
            NUDfours.Value = numMatchSizes[MatchSize.Fours];
            NUD3v2.Value = numMatchSizes[MatchSize.TripVsPair];
            NUD4v3.Value = numMatchSizes[MatchSize.FourVsTrip];
        }

        #endregion

        #region create matches

        bool GetParameters(out DayGeneratorParameters parameters, bool validate, bool fromScratch)
        {
            parameters = null;

            GetNumPlayers(out HashSet<Player> playersFree, out Counter<MatchSize> numMatchSizes);

            if (validate)
            {
                // Make sure all the players are correct

                if (playersSelectedForDay.Count == 0)
                {
                    BTNplayers.PerformClick();
                    MessageBox.Show("Please select players", "Not enough players");
                    return false;
                }

                if (Tools.SumOfPlayersInMatchSizes(numMatchSizes) != playersFree.Count)
                {
                    BTNconfirmbeforecreating.PerformClick();
                    MessageBox.Show("Please make sure that the number of matches is correct for the number of players", "Wrong number of players");
                    return false;
                }

                for (int matchIndex = 0; matchIndex < fixedMatchesDay?.matches.Count; matchIndex++)
                {
                    Match match = fixedMatchesDay.matches[matchIndex];
                    
                    foreach (Team team in match.teams)
                    {
                        bool errorFound = false;
                        int size = 0;
                        foreach (Player player in team.players)
                            if (player != null)
                                size++;
                        for (int position = 0; position < Team.MaxSize; position++)
                            if (Match.PositionShouldBeFilled((Position)position, size) != (team.players[position] != null))
                                errorFound = true;
                        if (errorFound)
                        {
                            BTNfixedMatches.PerformClick();
                            MessageBox.Show($"Please make sure that all the positions in the fixed matches have been filled");
                            return false;
                        }
                    }
                }

                // Adjust anything that needs to be fixed

                for (int matchIndex = 0; matchIndex < fixedMatchesDay?.matches.Count; matchIndex++)
                {
                    Match match = fixedMatchesDay.matches[matchIndex];
                    foreach (Team team in match.teams)
                    {
                        int size = 0;
                        foreach (Player player in team.players)
                            if (player != null)
                                size++;
                        team.size = size;
                    }
                }
            }

            parameters = new DayGeneratorParameters
            {
                players = new List<Player>(playersFree),
                history = history,
                weights = weights,
                numMatchSizes = numMatchSizes,
                existingDay = fromScratch ? null : generatedDay,
            };
            return true;
        }

        DayGeneratorParameters GetParameters(bool fromScratch)
        {
            GetParameters(out var result, false, fromScratch);
            return result;
        }

        void GetNumPlayers(out HashSet<Player> playersFree, out Counter<MatchSize> numMatchSizes)
        {
            playersFree = new HashSet<Player>(playersSelectedForDay.Except(fixedMatchesDay.Players()));
            numMatchSizes = new Counter<MatchSize>
            {
                { MatchSize.Pairs, (int)NUDpairs.Value },
                { MatchSize.Triples, (int)NUDtriples.Value },
                { MatchSize.Fours, (int)NUDfours.Value },
                { MatchSize.TripVsPair, (int)NUD3v2.Value },
                { MatchSize.FourVsTrip, (int)NUD4v3.Value },
            };
        }

        private void BTNnewgamesgo_Click(object sender, EventArgs e)
        {
            NewGames(true);
        }

        private void NewGames(bool fromScratch)
        {
            if (!GetParameters(out var parameters, true, fromScratch))
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
            var swap = new DayImprover(generatedDay, GetParameters(false)).DoOneImprovement();
            if (swap == null)
            {
                toolStripStatusLabel1.Text = "Could not find an improvement";
            }
            else
            {
                toolStripStatusLabel1.Text = swap switch
                {
                    RegularSwap s => $"Swapped {s.Player1} with {s.Player2}",
                    SimpleDoubleSwap s => $"Swapped {s.Player1a} with {s.Player2a} and {s.Player1b} with {s.Player2b}",
                    _ => throw new NotImplementedException(),
                };
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
                TBXnewdayConsole.Text += Math.Round(Penalty.Score(), 3) + "\t" + PenaltyConverter.Convert(Penalty) + " " + PenaltyConverter.CiteOccurence(Penalty, history) + Environment.NewLine;
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
            // Look at which player was clicked on
            Team team = generatedDay.matches[matchIndex].teams[teamIndex];
            Player player = team.Player(position);
            // Make sure a player was clicked on
            if (player == null) return;
            // Adjust the position so it points to the player
            position = Array.IndexOf(team.players, player);
            // Find the playerIndex
            int playerIndex = RegularSwap.CreatePlayerIndex(matchIndex, teamIndex, position);

            if (swapPlayerIndex == -1)
            {
                swapPlayerIndex = playerIndex;
                toolStripStatusLabel1.Text = $"Swapping {player.Name}...";
            }
            else
            {
                RegularSwap swap = new RegularSwap(swapPlayerIndex, playerIndex, generatedDay);
                swap.DoSwap();

                CachedPenalties penalties = new CachedPenalties(GetParameters(false));
                penalties.RecalculateScore(generatedDay);

                swapPlayerIndex = -1;
                toolStripStatusLabel1.Text = $"Swapped {swap.Player1.Name} with {swap.Player2.Name}";

                RewritePenalties(); 
                DisplayGeneratedDay();
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
            if (WEBnewgames.ReadyState != WebBrowserReadyState.Complete) return;

            SetWebBrowserZoom(WEBnewgames, currentZoom);

            if (scrollTop != -1)
            {
                WEBnewgames.Document.GetElementsByTagName("HTML")[0].ScrollTop = scrollTop;
                scrollTop = -1;
            }
        }

        private void BTNnewgamesave_Click(object sender, EventArgs e)
        {
            if (generatedDay == null) return;

            generatedDay.matches.Sort(Sorts.MatchRinkCompare);

            history.Add(generatedDay);
            Save();
            
            generatedDay = null;
            fixedMatchesDay = null;
            playersSelectedForDay.Clear();
            SetMatchSizes(new Counter<MatchSize>());

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

            Zoom(Properties.Settings.Default.Zoom);

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
            HTMLformatedChanged();
        }

        void HTMLformatedChanged()
        {
            DisplayFixedMatches();
            ReloadSelectedDaysInHistory();
        }

        private void BTNseedefaulthtml_Click(object sender, EventArgs e)
        {
            if (SFDhtml.ShowDialog() == DialogResult.Cancel) return;
            using StreamWriter streamWriter = new StreamWriter(SFDhtml.FileName);
            streamWriter.Write(Properties.Resources.table);
            Process.Start("notepad.exe", SFDhtml.FileName);
        }

        private void BTNprintersettings_Click(object sender, EventArgs e)
        {
            WEBhistory.ShowPageSetupDialog();
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

        Func<PlayerIntermediate, bool> _filter = p => true;
        Func<PlayerIntermediate, bool> PlayerListFilter
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
            playerBindingSource.DataSource = players.Select(p => new PlayerIntermediate(p)).Where(PlayerListFilter).ToList();
        }

        private void BTNsortnameplayers_Click(object sender, EventArgs e)
        {
            players.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
            RefreshPlayerFilters();
        }

        private void BTNsorttagplayers_Click(object sender, EventArgs e)
        {
            players.Sort(Sorts.TagNumberCompare);
            RefreshPlayerFilters();
        }

        private void BTNaddplayer_Click(object sender, EventArgs e)
        {
            Player player = new Player()
            {
                ID = DataCreation.UniqueRandomInt(players),
                TagNumber = RBNmember.Checked ? DataCreation.NextTagNumber(players) : "",
            };
            players.Insert(0, player);
            RefreshPlayerFilters();
            int rowIndex = (playerBindingSource.DataSource as List<PlayerIntermediate>).FindIndex(p => p.player == player);
            dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1];
            dataGridView1.BeginEdit(true);
        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (e.ColumnIndex == -1) return;
                if (dataGrid.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
                {
                    dataGrid.CurrentCell = dataGrid[e.ColumnIndex, e.RowIndex];
                    dataGrid.BeginEdit(true);
                    ((ComboBox)dataGrid.EditingControl).DroppedDown = true;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
                CMSdeleteplayer.Show(Cursor.Position);
            }
        }

        private void DeletePlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var indicies = new List<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                if (!indicies.Contains(cell.RowIndex))
                    indicies.Add(cell.RowIndex);
            List<PlayerIntermediate> playersInGrid = playerBindingSource.DataSource as List<PlayerIntermediate>;
            List<Player> selectedPlayers = new List<Player>();
            foreach (int rowIndex in indicies)
                selectedPlayers.Add(playersInGrid[rowIndex].player);

            // Confirmation
            string listPlayersForMessage = Tools.ShortListDescription(selectedPlayers);
            if (MessageBox.Show("Are you sure you want to delete " + listPlayersForMessage + "?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No) return;

            DataDeletion.DeletePlayers(selectedPlayers, players, history, playersSelectedForDay);
            toolStripStatusLabel1.Text = "Deleted " + listPlayersForMessage;

            RefreshPlayerFilters();
        }

        private void BTNexcelimport_Click(object sender, EventArgs e)
        {
            if (OFDexcel.ShowDialog() == DialogResult.Cancel) return;
            SFDexcel.FileName = OFDexcel.FileName;

            FormTableImporter importerForm = new FormTableImporterPlayer(new ExcelReader(OFDexcel.FileName), players);
            if (importerForm.ShowDialog() == DialogResult.OK)
            {
                RefreshFullListOfPlayers();
                toolStripStatusLabel1.Text = "Imported from " + SFDexcel.FileName;
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
            ReloadSelectedDaysInHistory();
        }

        void ReloadSelectedDaysInHistory()
        {
            List<Day> days = new List<Day>();
            foreach (Day day in LBXhistory.SelectedItems)
                days.Add(day);
            WEBhistory.DocumentText = HTMLdocument.GenerateDays(days);
            deleteToolStripMenuItem.Text = days.Count == 1 ? "Delete day" : $"Delete {days.Count} days";
        }

        private void WEBhistory_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            SetWebBrowserZoom(WEBhistory, currentZoom);
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

            DataDeletion.DeleteFromHistory(history, indices);

            toolStripStatusLabel1.Text = $"Deleted " + listDaysForMessage;
            RefreshHistoryList();
            LBXhistory.SelectedIndices.Clear();
            if (LBXhistory.Items.Count > 0)
                LBXhistory.SelectedIndices.Add(0);
            ReloadSelectedDaysInHistory();
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

        #endregion

        #region screen zooming

        private void Zoom100ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Zoom(1f);
        }

        private void Zoom125ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Zoom(1.25f);
        }

        private void Zoom150ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Zoom(1.5f);
        }

        private void Zoom200ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Zoom(2f);
        }

        private void Zoom250ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Zoom(2.5f);
        }

        private float currentZoom = 1;

        private void Zoom(float size)
        {
            ZoomRecursive(this, size, size / currentZoom);
            currentZoom = size;

            toolStripDropDownButton1.Text = $"Zoom: {currentZoom * 100}%";

            Properties.Settings.Default.Zoom = currentZoom;
            Properties.Settings.Default.Save();
        }

        private void ZoomRecursive(Control control, float size, float scale)
        {
            if (control.Font != control.Parent?.Font)
            {
                Debug.Assert(this.AutoScaleMode == AutoScaleMode.Font, "The scale should be driven by the font.");
                control.Font = new Font(
                    control.Font.FontFamily,
                    control.Font.Size * scale,
                    control.Font.Style,
                    control.Font.Unit,
                    control.Font.GdiCharSet,
                    control.Font.GdiVerticalFont);
            }

            if (control is WebBrowser wb && wb.ReadyState == WebBrowserReadyState.Complete)
            {
                SetWebBrowserZoom(wb, size);
            }
            else if (control is DataGridView dgv)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.Width = (int)(column.Width * scale);
                }
            }

            if (control.Controls != null)
            {
                foreach (Control child in control.Controls)
                {
                    ZoomRecursive(child, size, scale);
                }
            }
        }

        private void SetWebBrowserZoom(WebBrowser wb, float size)
        {
            int OLECMDID_OPTICAL_ZOOM = 63;
            int OLECMDEXECOPT_DONTPROMPTUSER = 2;
            dynamic iwb2 = wb.ActiveXInstance;
            int zoom = Math.Max(Math.Min(1000, (int)(size * 100)), 10);
            iwb2.ExecWB(OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT_DONTPROMPTUSER, zoom, zoom);
        }

        #endregion
    }
}
