//---------------------------------------------------------------------
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Reflection;

namespace CustomControls
{
    /// <summary>
    /// Provides a drop-down filter list in a DataGridViewColumnHeaderCell.
    /// </summary>
    public class DataGridViewDropDownHeaderCell : DataGridViewColumnHeaderCell
    {
        /// <summary>
        /// The ListBox used for all drop-down lists. 
        /// </summary>
        private readonly DropDownListBox dropDownListBox = new DropDownListBox();

        /// <summary>
        /// A list of options that appear in the dropdown list
        /// </summary>
        public string[] AllOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumnHeaderCell 
        /// class and sets its property values to the property values of the 
        /// specified DataGridViewColumnHeaderCell.
        /// </summary>
        /// <param name="oldHeaderCell">The DataGridViewColumnHeaderCell to copy property values from.</param>
        public DataGridViewDropDownHeaderCell(DataGridViewColumnHeaderCell oldHeaderCell)
        {
            this.ContextMenuStrip = oldHeaderCell.ContextMenuStrip;
            this.ErrorText = oldHeaderCell.ErrorText;
            this.Tag = oldHeaderCell.Tag;
            this.ToolTipText = oldHeaderCell.ToolTipText;
            this.Value = oldHeaderCell.Value;
            this.ValueType = oldHeaderCell.ValueType;

            // Use HasStyle to avoid creating a new style object
            // when the Style property has not previously been set. 
            if (oldHeaderCell.HasStyle)
            {
                this.Style = oldHeaderCell.Style;
            }

            // Copy this type's properties if the old cell is an auto-filter cell. 
            // This enables the Clone method to reuse this constructor. 
            if (oldHeaderCell is DataGridViewDropDownHeaderCell filterCell)
            {
                this.DropDownListBoxMaxLines = filterCell.DropDownListBoxMaxLines;
                this.currentDropDownButtonPaddingOffset = filterCell.currentDropDownButtonPaddingOffset;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumnHeaderCell 
        /// class. 
        /// </summary>
        public DataGridViewDropDownHeaderCell()
        {
        }

        /// <summary>
        /// Creates an exact copy of this cell.
        /// </summary>
        /// <returns>An object that represents the cloned DataGridViewAutoFilterColumnHeaderCell.</returns>
        public override object Clone()
        {
            return new DataGridViewDropDownHeaderCell(this);
        }

        /// <summary>
        /// Called when the value of the DataGridView property changes
        /// in order to perform initialization that requires access to the 
        /// owning control and column. 
        /// </summary>
        protected override void OnDataGridViewChanged()
        {
            // Continue only if there is a DataGridView. 
            if (this.DataGridView == null)
            {
                return;
            }

            // Disable sorting and filtering for columns that can't make
            // effective use of them. 
            // Ensure that the column SortMode property value is not Automatic.
            // This prevents sorting when the user clicks the drop-down button.
            if (OwningColumn != null && OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
            {
                OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            // Add handlers to DataGridView events. 
            HandleDataGridViewEvents();

            // Initialize the drop-down button bounds so that any initial
            // column autosizing will accommodate the button width. 
            SetDropDownButtonBounds();

            // Call the OnDataGridViewChanged method on the base class to 
            // raise the DataGridViewChanged event.
            base.OnDataGridViewChanged();
        }

        #region DataGridView events: HandleDataGridViewEvents, DataGridView event handlers, ResetDropDown, ResetFilter

        /// <summary>
        /// Add handlers to various DataGridView events, primarily to invalidate 
        /// the drop-down button bounds, hide the drop-down list, and reset 
        /// cached filter values when changes in the DataGridView require it.
        /// </summary>
        private void HandleDataGridViewEvents()
        {
            this.DataGridView.Scroll += new ScrollEventHandler(DataGridView_Scroll);
            this.DataGridView.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnDisplayIndexChanged);
            this.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
            this.DataGridView.ColumnHeadersHeightChanged += new EventHandler(DataGridView_ColumnHeadersHeightChanged);
            this.DataGridView.SizeChanged += new EventHandler(DataGridView_SizeChanged);
            this.DataGridView.DataSourceChanged += new EventHandler(DataGridView_DataSourceChanged);
            this.DataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGridView_DataBindingComplete);
        }

        /// <summary>
        /// Invalidates the drop-down button bounds when the user scrolls horizontally.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A ScrollEventArgs that contains the event data.</param>
        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ResetDropDown();
            }
        }

        /// <summary>
        /// Invalidates the drop-down button bounds when the column display index changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetDropDown();
        }

        /// <summary>
        /// Invalidates the drop-down button bounds when a column width changes
        /// in the DataGridView control. A width change in any column of the 
        /// control has the potential to affect the drop-down button location, 
        /// depending on the current horizontal scrolling position and whether
        /// the changed column is to the left or right of the current column. 
        /// It is easier to invalidate the button in all cases. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A DataGridViewColumnEventArgs that contains the event data.</param>
        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetDropDown();
        }

        /// <summary>
        /// Invalidates the drop-down button bounds when the height of the column headers changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void DataGridView_ColumnHeadersHeightChanged(object sender, EventArgs e)
        {
            ResetDropDown();
        }

        /// <summary>
        /// Invalidates the drop-down button bounds when the size of the DataGridView changes.
        /// This prevents a painting issue that occurs when the right edge of the control moves 
        /// to the right and the control contents have previously been scrolled to the right.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void DataGridView_SizeChanged(object sender, EventArgs e)
        {
            ResetDropDown();
        }

        /// <summary>
        /// Invalidates the drop-down button bounds, hides the drop-down 
        /// filter list, if it is showing, and resets the cached filter values
        /// if the filter has been removed. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A DataGridViewBindingCompleteEventArgs that contains the event data.</param>
        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                ResetDropDown();
            }
        }

        /// <summary>
        /// Verifies that the data source meets requirements, invalidates the 
        /// drop-down button bounds, hides the drop-down filter list if it is 
        /// showing, and resets the cached filter values if the filter has been removed. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void DataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            ResetDropDown();
        }

        /// <summary>
        /// Invalidates the drop-down button bounds and hides the filter
        /// list if it is showing.
        /// </summary>
        private void ResetDropDown()
        {
            InvalidateDropDownButtonBounds();
            if (dropDownListBoxShowing)
            {
                HideDropDownList();
            }
        }

        #endregion DataGridView events

        /// <summary>
        /// Paints the column header cell, including the drop-down button. 
        /// </summary>
        /// <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
        /// <param name="clipBounds">A Rectangle that represents the area of the DataGridView that needs to be repainted.</param>
        /// <param name="cellBounds">A Rectangle that contains the bounds of the DataGridViewCell that is being painted.</param>
        /// <param name="rowIndex">The row index of the cell that is being painted.</param>
        /// <param name="cellState">A bitwise combination of DataGridViewElementStates values that specifies the state of the cell.</param>
        /// <param name="value">The data of the DataGridViewCell that is being painted.</param>
        /// <param name="formattedValue">The formatted data of the DataGridViewCell that is being painted.</param>
        /// <param name="errorText">An error message that is associated with the cell.</param>
        /// <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
        /// <param name="advancedBorderStyle">A DataGridViewAdvancedBorderStyle that contains border styles for the cell that is being painted.</param>
        /// <param name="paintParts">A bitwise combination of the DataGridViewPaintParts values that specifies which parts of the cell need to be painted.</param>
        protected override void Paint(
            Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // Use the base method to paint the default appearance. 
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                cellState, value, formattedValue,
                errorText, cellStyle, advancedBorderStyle, paintParts);

            // Continue only if filtering is enabled and ContentBackground is 
            // part of the paint request. 
            if ((paintParts & DataGridViewPaintParts.ContentBackground) == 0)
            {
                return;
            }

            // Retrieve the current button bounds. 
            Rectangle buttonBounds = DropDownButtonBounds;

            // Continue only if the buttonBounds is big enough to draw.
            if (buttonBounds.Width < 1 || buttonBounds.Height < 1) return;

            // Paint the button manually or using visual styles if visual styles 
            // are enabled, using the correct state depending on whether the 
            // filter list is showing and whether there is a filter in effect 
            // for the current column. 
            if (Application.RenderWithVisualStyles)
            {
                ComboBoxState state = ComboBoxState.Normal;

                if (dropDownListBoxShowing)
                {
                    state = ComboBoxState.Pressed;
                }
                ComboBoxRenderer.DrawDropDownButton(
                    graphics, buttonBounds, state);
            }
            else
            {
                // Determine the pressed state in order to paint the button 
                // correctly and to offset the down arrow. 
                Int32 pressedOffset = 0;
                PushButtonState state = PushButtonState.Normal;
                if (dropDownListBoxShowing)
                {
                    state = PushButtonState.Pressed;
                    pressedOffset = 1;
                }
                ButtonRenderer.DrawButton(graphics, buttonBounds, state);

                graphics.FillPolygon(SystemBrushes.ControlText, new Point[] {
                        new Point(
                            buttonBounds.Width / 2 +
                                buttonBounds.Left - 1 + pressedOffset,
                            buttonBounds.Height * 3 / 4 +
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width / 4 +
                                buttonBounds.Left + pressedOffset,
                            buttonBounds.Height / 2 +
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width * 3 / 4 +
                                buttonBounds.Left - 1 + pressedOffset,
                            buttonBounds.Height / 2 +
                                buttonBounds.Top - 1 + pressedOffset)
                    });
            }
        }

        /// <summary>
        /// Handles mouse clicks to the header cell, displaying the 
        /// drop-down list or sorting the owning column as appropriate. 
        /// </summary>
        /// <param name="e">A DataGridViewCellMouseEventArgs that contains the event data.</param>
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            Debug.Assert(this.DataGridView != null, "DataGridView is null");

            // Continue only if the user did not click the drop-down button 
            // while the drop-down list was displayed. This prevents the 
            // drop-down list from being redisplayed after being hidden in 
            // the LostFocus event handler. 
            if (lostFocusOnDropDownButtonClick)
            {
                lostFocusOnDropDownButtonClick = false;
                return;
            }

            // Retrieve the current size and location of the header cell,
            // excluding any portion that is scrolled off screen. 
            Rectangle cellBounds = this.DataGridView
                .GetCellDisplayRectangle(e.ColumnIndex, -1, false);

            // Continue only if the column is not manually resizable or the
            // mouse coordinates are not within the column resize zone. 
            if (this.OwningColumn.Resizable == DataGridViewTriState.True &&
                ((this.DataGridView.RightToLeft == RightToLeft.No &&
                cellBounds.Width - e.X < 6) || e.X < 6))
            {
                return;
            }

            // Unless RightToLeft is enabled, store the width of the portion
            // that is scrolled off screen. 
            Int32 scrollingOffset = 0;
            if (this.DataGridView.RightToLeft == RightToLeft.No &&
                this.DataGridView.FirstDisplayedScrollingColumnIndex == this.ColumnIndex)
            {
                scrollingOffset = this.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
            }

            // Show the drop-down list if filtering is enabled and the mouse click occurred
            // within the drop-down button bounds. Otherwise, if sorting is enabled and the
            // click occurred outside the drop-down button bounds, sort by the owning column. 
            // The mouse coordinates are relative to the cell bounds, so the cell location
            // and the scrolling offset are needed to determine the client coordinates.
            if (DropDownButtonBounds.Contains(e.X + cellBounds.Left - scrollingOffset, e.Y + cellBounds.Top))
            {
                // If the current cell is in edit mode, commit the edit. 
                if (this.DataGridView.IsCurrentCellInEditMode)
                {
                    // Commit and end the cell edit.  
                    this.DataGridView.EndEdit();

                    // Commit any change to the underlying data source. 
                    if (this.DataGridView.DataSource is BindingSource source)
                    {
                        source.EndEdit();
                    }
                }
                ShowDropDownList();
            }

            base.OnMouseDown(e);
        }

        #region drop-down list: Show/HideDropDownListBox, SetDropDownListBoxBounds, DropDownListBoxMaxHeightInternal

        /// <summary>
        /// Indicates whether dropDownListBox is currently displayed 
        /// for this header cell. 
        /// </summary>
        private bool dropDownListBoxShowing;

        /// <summary>
        /// Displays the drop-down filter list. 
        /// </summary>
        public void ShowDropDownList()
        {
            Debug.Assert(this.DataGridView != null, "DataGridView is null");

            // Ensure that the current row is not the row for new records.
            // This prevents the new row from affecting the filter list and also
            // prevents the new row from being added when the filter changes.
            if (this.DataGridView.CurrentRow != null &&
                this.DataGridView.CurrentRow.IsNewRow)
            {
                this.DataGridView.CurrentCell = null;
            }

            dropDownListBox.Items.Clear();
            dropDownListBox.Items.AddRange(AllOptions);

            // Add handlers to dropDownListBox events. 
            HandleDropDownListBoxEvents();

            // Set the size and location of dropDownListBox, then display it. 
            SetDropDownListBoxBounds();
            dropDownListBox.Visible = true;
            dropDownListBoxShowing = true;

            Debug.Assert(dropDownListBox.Parent == null,
                "ShowDropDownListBox has been called multiple times before HideDropDownListBox");

            // Add dropDownListBox to the DataGridView. 
            this.DataGridView.Controls.Add(dropDownListBox);

            // Set the input focus to dropDownListBox. 
            dropDownListBox.Focus();

            // Invalidate the cell so that the drop-down button will repaint
            // in the pressed state. 
            this.DataGridView.InvalidateCell(this);
        }

        /// <summary>
        /// Hides the drop-down filter list. 
        /// </summary>
        public void HideDropDownList()
        {
            Debug.Assert(this.DataGridView != null, "DataGridView is null");

            // Hide dropDownListBox, remove handlers from its events, and remove 
            // it from the DataGridView control. 
            dropDownListBoxShowing = false;
            dropDownListBox.Visible = false;
            UnhandleDropDownListBoxEvents();
            this.DataGridView.Controls.Remove(dropDownListBox);

            // Invalidate the cell so that the drop-down button will repaint
            // in the unpressed state. 
            this.DataGridView.InvalidateCell(this);
        }

        /// <summary>
        /// Sets the dropDownListBox size and position based on the formatted 
        /// values in the filters dictionary and the position of the drop-down 
        /// button. Called only by ShowDropDownListBox.  
        /// </summary>
        private void SetDropDownListBoxBounds()
        {
            Debug.Assert(AllOptions.Length > 0, "AllOptions.Length <= 0");

            // Declare variables that will be used in the calculation, 
            // initializing dropDownListBoxHeight to account for the 
            // ListBox borders.
            Int32 dropDownListBoxHeight = 2;
            Int32 currentWidth;
            Int32 dropDownListBoxWidth = 0;
            Int32 dropDownListBoxLeft;

            // For each formatted value in the filters dictionary Keys collection,
            // add its height to dropDownListBoxHeight and, if it is wider than 
            // all previous values, set dropDownListBoxWidth to its width.
            using (Graphics graphics = dropDownListBox.CreateGraphics())
            {
                foreach (string option in AllOptions)
                {
                    SizeF stringSizeF = graphics.MeasureString(option, dropDownListBox.Font);
                    dropDownListBoxHeight += (Int32)stringSizeF.Height;
                    currentWidth = (Int32)stringSizeF.Width;
                    if (dropDownListBoxWidth < currentWidth)
                    {
                        dropDownListBoxWidth = currentWidth;
                    }
                }
            }

            // Increase the width to allow for horizontal margins and borders. 
            dropDownListBoxWidth += 6;

            // Constrain the dropDownListBox height to the 
            // DropDownListBoxMaxHeightInternal value, which is based on 
            // the DropDownListBoxMaxLines property value but constrained by
            // the maximum height available in the DataGridView control.
            if (dropDownListBoxHeight > DropDownListBoxMaxHeightInternal)
            {
                dropDownListBoxHeight = DropDownListBoxMaxHeightInternal;

                // If the preferred height is greater than the available height,
                // adjust the width to accommodate the vertical scroll bar. 
                dropDownListBoxWidth += SystemInformation.VerticalScrollBarWidth;
            }

            // Calculate the ideal location of the left edge of dropDownListBox 
            // based on the location of the drop-down button and taking the 
            // RightToLeft property value into consideration. 
            if (this.DataGridView.RightToLeft == RightToLeft.No)
            {
                dropDownListBoxLeft = DropDownButtonBounds.Right - dropDownListBoxWidth + 1;
            }
            else
            {
                dropDownListBoxLeft = DropDownButtonBounds.Left - 1;
            }

            // Determine the left and right edges of the available horizontal
            // width of the DataGridView control. 
            Int32 clientLeft = 1;
            Int32 clientRight = this.DataGridView.ClientRectangle.Right;
            if (this.DataGridView.DisplayedRowCount(false) < this.DataGridView.RowCount)
            {
                if (this.DataGridView.RightToLeft == RightToLeft.Yes)
                {
                    clientLeft += SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    clientRight -= SystemInformation.VerticalScrollBarWidth;
                }
            }

            // Adjust the dropDownListBox location and/or width if it would
            // otherwise overlap the left or right edge of the DataGridView.
            if (dropDownListBoxLeft < clientLeft)
            {
                dropDownListBoxLeft = clientLeft;
            }
            Int32 dropDownListBoxRight =
                dropDownListBoxLeft + dropDownListBoxWidth + 1;
            if (dropDownListBoxRight > clientRight)
            {
                if (dropDownListBoxLeft == clientLeft)
                {
                    dropDownListBoxWidth -=
                        dropDownListBoxRight - clientRight;
                }
                else
                {
                    dropDownListBoxLeft -=
                        dropDownListBoxRight - clientRight;
                    if (dropDownListBoxLeft < clientLeft)
                    {
                        dropDownListBoxWidth -= clientLeft - dropDownListBoxLeft;
                        dropDownListBoxLeft = clientLeft;
                    }
                }
            }

            // Set the ListBox.Bounds property using the calculated values. 
            dropDownListBox.Bounds = new Rectangle(dropDownListBoxLeft,
                DropDownButtonBounds.Bottom, // top of drop-down list box
                dropDownListBoxWidth, dropDownListBoxHeight);
        }

        /// <summary>
        /// Gets the actual maximum height of the drop-down list, in pixels.
        /// The maximum height is calculated from the DropDownListBoxMaxLines 
        /// property value, but is limited to the available height of the 
        /// DataGridView control. 
        /// </summary>
        protected Int32 DropDownListBoxMaxHeightInternal
        {
            get
            {
                // Calculate the height of the available client area
                // in the DataGridView control, taking the horizontal
                // scroll bar into consideration and leaving room
                // for the ListBox bottom border. 
                Int32 dataGridViewMaxHeight = this.DataGridView.Height -
                    this.DataGridView.ColumnHeadersHeight - 1;
                if (this.DataGridView.DisplayedColumnCount(false) <
                    this.DataGridView.ColumnCount)
                {
                    dataGridViewMaxHeight -=
                        SystemInformation.HorizontalScrollBarHeight;
                }

                // Calculate the height of the list box, using the combined 
                // height of all items plus 2 for the top and bottom border. 
                Int32 listMaxHeight = dropDownListBoxMaxLinesValue * dropDownListBox.ItemHeight + 2;

                // Return the smaller of the two values. 
                if (listMaxHeight < dataGridViewMaxHeight)
                {
                    return listMaxHeight;
                }
                else
                {
                    return dataGridViewMaxHeight;
                }
            }
        }

        #endregion drop-down list

        #region ListBox events: HandleDropDownListBoxEvents, UnhandleDropDownListBoxEvents, ListBox event handlers

        /// <summary>
        /// Adds handlers to ListBox events for handling mouse
        /// and keyboard input.
        /// </summary>
        private void HandleDropDownListBoxEvents()
        {
            dropDownListBox.MouseClick += new MouseEventHandler(DropDownListBox_MouseClick);
            dropDownListBox.LostFocus += new EventHandler(DropDownListBox_LostFocus);
            dropDownListBox.KeyDown += new KeyEventHandler(DropDownListBox_KeyDown);
        }

        /// <summary>
        /// Removes the ListBox event handlers. 
        /// </summary>
        private void UnhandleDropDownListBoxEvents()
        {
            dropDownListBox.MouseClick -= new MouseEventHandler(DropDownListBox_MouseClick);
            dropDownListBox.LostFocus -= new EventHandler(DropDownListBox_LostFocus);
            dropDownListBox.KeyDown -= new KeyEventHandler(DropDownListBox_KeyDown);
        }

        /// <summary>
        /// Adjusts the filter in response to a user selection from the drop-down list. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        private void DropDownListBox_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.Assert(this.DataGridView != null, "DataGridView is null");

            // Continue only if the mouse click was in the content area
            // and not on the scroll bar. 
            if (!dropDownListBox.DisplayRectangle.Contains(e.X, e.Y))
            {
                return;
            }

            ItemSelected();
            HideDropDownList();
        }

        /// <summary>
        /// Indicates whether the drop-down list lost focus because the
        /// user clicked the drop-down button. 
        /// </summary>
        private Boolean lostFocusOnDropDownButtonClick;

        /// <summary>
        /// Hides the drop-down list when it loses focus. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void DropDownListBox_LostFocus(object sender, EventArgs e)
        {
            // If the focus was lost because the user clicked the drop-down
            // button, store a value that prevents the subsequent OnMouseDown
            // call from displaying the drop-down list again. 
            if (DropDownButtonBounds.Contains(
                this.DataGridView.PointToClient(new Point(
                Control.MousePosition.X, Control.MousePosition.Y))))
            {
                lostFocusOnDropDownButtonClick = true;
            }
            HideDropDownList();
        }

        /// <summary>
        /// Handles the ENTER and ESC keys.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        void DropDownListBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ItemSelected();
                    HideDropDownList();
                    break;
                case Keys.Escape:
                    HideDropDownList();
                    break;
            }
        }

        /// <summary>
        /// Updates the BindingSource.Filter value based on a user selection
        /// from the drop-down filter list. 
        /// </summary>
        private void ItemSelected()
        {
            HeaderOptionClicked(dropDownListBox.SelectedIndex);
            Value = dropDownListBox.SelectedItem.ToString();
        }

        public event Action<int> HeaderOptionClicked;

        #endregion ListBox events

        #region button bounds: DropDownButtonBounds, InvalidateDropDownButtonBounds, SetDropDownButtonBounds, AdjustPadding

        /// <summary>
        /// The bounds of the drop-down button, or Rectangle.Empty if filtering 
        /// is disabled or the button bounds need to be recalculated. 
        /// </summary>
        private Rectangle dropDownButtonBoundsValue = Rectangle.Empty;

        /// <summary>
        /// The bounds of the drop-down button, or Rectangle.Empty if filtering
        /// is disabled. Recalculates the button bounds if filtering is enabled
        /// and the bounds are empty.
        /// </summary>
        protected Rectangle DropDownButtonBounds
        {
            get
            {
                if (dropDownButtonBoundsValue == Rectangle.Empty)
                {
                    SetDropDownButtonBounds();
                }
                return dropDownButtonBoundsValue;
            }
        }

        /// <summary>
        /// Sets dropDownButtonBoundsValue to Rectangle.Empty if it isn't already empty. 
        /// This indicates that the button bounds should be recalculated. 
        /// </summary>
        private void InvalidateDropDownButtonBounds()
        {
            if (!dropDownButtonBoundsValue.IsEmpty)
            {
                dropDownButtonBoundsValue = Rectangle.Empty;
            }
        }

        /// <summary>
        /// Sets the position and size of dropDownButtonBoundsValue based on the current 
        /// cell bounds and the preferred cell height for a single line of header text. 
        /// </summary>
        private void SetDropDownButtonBounds()
        {
            // Retrieve the cell display rectangle, which is used to 
            // set the position of the drop-down button. 
            Rectangle cellBounds =
                this.DataGridView.GetCellDisplayRectangle(
                this.ColumnIndex, -1, false);

            // Initialize a variable to store the button edge length,
            // setting its initial value based on the font height. 
            Int32 buttonEdgeLength = this.InheritedStyle.Font.Height + 5;

            // Calculate the height of the cell borders and padding.
            Rectangle borderRect = BorderWidths(
                this.DataGridView.AdjustColumnHeaderBorderStyle(
                this.DataGridView.AdvancedColumnHeadersBorderStyle,
                new DataGridViewAdvancedBorderStyle(), false, false));
            Int32 borderAndPaddingHeight = 2 +
                borderRect.Top + borderRect.Height +
                this.InheritedStyle.Padding.Vertical;
            Boolean visualStylesEnabled =
                Application.RenderWithVisualStyles &&
                this.DataGridView.EnableHeadersVisualStyles;
            if (visualStylesEnabled)
            {
                borderAndPaddingHeight += 3;
            }

            // Constrain the button edge length to the height of the 
            // column headers minus the border and padding height. 
            if (buttonEdgeLength >
                this.DataGridView.ColumnHeadersHeight -
                borderAndPaddingHeight)
            {
                buttonEdgeLength =
                    this.DataGridView.ColumnHeadersHeight -
                    borderAndPaddingHeight;
            }

            // Constrain the button edge length to the
            // width of the cell minus three.
            if (buttonEdgeLength > cellBounds.Width - 3)
            {
                buttonEdgeLength = cellBounds.Width - 3;
            }

            // Calculate the location of the drop-down button, with adjustments
            // based on whether visual styles are enabled. 
            Int32 topOffset = visualStylesEnabled ? 4 : 1;
            Int32 top = cellBounds.Bottom - buttonEdgeLength - topOffset;
            Int32 leftOffset = visualStylesEnabled ? 3 : 1;
            Int32 left;
            if (this.DataGridView.RightToLeft == RightToLeft.No)
            {
                left = cellBounds.Right - buttonEdgeLength - leftOffset;
            }
            else
            {
                left = cellBounds.Left + leftOffset;
            }

            // Set the dropDownButtonBoundsValue value using the calculated 
            // values, and adjust the cell padding accordingly.  
            dropDownButtonBoundsValue = new Rectangle(left, top,
                buttonEdgeLength, buttonEdgeLength);
            AdjustPadding(buttonEdgeLength + leftOffset);
        }

        /// <summary>
        /// Adjusts the cell padding to widen the header by the drop-down button width.
        /// </summary>
        /// <param name="newDropDownButtonPaddingOffset">The new drop-down button width.</param>
        private void AdjustPadding(Int32 newDropDownButtonPaddingOffset)
        {
            // Determine the difference between the new and current 
            // padding adjustment.
            Int32 widthChange = newDropDownButtonPaddingOffset -
                currentDropDownButtonPaddingOffset;

            // If the padding needs to change, store the new value and 
            // make the change.
            if (widthChange != 0)
            {
                // Store the offset for the drop-down button separately from 
                // the padding in case the client needs additional padding.
                currentDropDownButtonPaddingOffset =
                    newDropDownButtonPaddingOffset;

                // Create a new Padding using the adjustment amount, then add it
                // to the cell's existing Style.Padding property value. 
                Padding dropDownPadding = new Padding(0, 0, widthChange, 0);
                this.Style.Padding = Padding.Add(
                    this.InheritedStyle.Padding, dropDownPadding);
            }
        }

        /// <summary>
        /// The current width of the drop-down button. This field is used to adjust the cell padding.  
        /// </summary>
        private Int32 currentDropDownButtonPaddingOffset;

        #endregion button bounds

        #region public property: DropDownListBoxMaxLines

        /// <summary>
        /// The maximum number of lines in the drop-down list. 
        /// </summary>
        private Int32 dropDownListBoxMaxLinesValue = 20;

        /// <summary>
        /// Gets or sets the maximum number of lines to display in the drop-down filter list. 
        /// The actual height of the drop-down list is constrained by the DataGridView height. 
        /// </summary>
        [DefaultValue(20)]
        public Int32 DropDownListBoxMaxLines
        {
            get { return dropDownListBoxMaxLinesValue; }
            set { dropDownListBoxMaxLinesValue = value; }
        }

        #endregion public properties

        /// <summary>
        /// Represents a ListBox control used as a drop-down filter list
        /// in a DataGridView control.
        /// </summary>
        private class DropDownListBox : ListBox
        {
            /// <summary>
            /// Initializes a new instance of the FilterListBox class.
            /// </summary>
            public DropDownListBox()
            {
                Visible = false;
                IntegralHeight = true;
                BorderStyle = BorderStyle.FixedSingle;
                TabStop = false;
            }

            /// <summary>
            /// Indicates that the FilterListBox will handle (or ignore) all 
            /// keystrokes that are not handled by the operating system. 
            /// </summary>
            /// <param name="keyData">A Keys value that represents the keyboard input.</param>
            /// <returns>true in all cases.</returns>
            protected override bool IsInputKey(Keys keyData)
            {
                return true;
            }

            /// <summary>
            /// Processes a keyboard message directly, preventing it from being
            /// intercepted by the parent DataGridView control.
            /// </summary>
            /// <param name="m">A Message, passed by reference, that 
            /// represents the window message to process.</param>
            /// <returns>true if the message was processed by the control;
            /// otherwise, false.</returns>
            protected override bool ProcessKeyMessage(ref Message m)
            {
                return ProcessKeyEventArgs(ref m);
            }

        }

    }

}

