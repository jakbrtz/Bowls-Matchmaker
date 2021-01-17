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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace CustomControls
{
    /// <summary>
    /// Represents a DataGridViewTextBoxColumn with a drop-down filter list accessible from the header cell.  
    /// </summary>
    public class DataGridViewDropDownHeaderColumn : DataGridViewTextBoxColumn
    {
        /// <summary>
        /// Initializes a new instance of the DataGridViewAutoFilterTextBoxColumn class.
        /// </summary>
        public DataGridViewDropDownHeaderColumn() : base()
        {
            base.DefaultHeaderCellType = typeof(DataGridViewDropDownHeaderCell);
            base.SortMode = DataGridViewColumnSortMode.Programmatic;
            ((DataGridViewDropDownHeaderCell)HeaderCell).HeaderOptionClicked += DataGridViewDropDownHeaderColumn_HeaderOptionClicked; ;
        }

        /// <summary>
        /// Returns the AutoFilter header cell type. This property hides the 
        /// non-virtual DefaultHeaderCellType property inherited from the 
        /// DataGridViewBand class. The inherited property is set in the 
        /// DataGridViewAutoFilterTextBoxColumn constructor. 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Type DefaultHeaderCellType
        {
            get
            {
                return typeof(DataGridViewDropDownHeaderCell);
            }
        }

        /// <summary>
        /// Gets or sets the maximum height of the drop-down filter list for this column. 
        /// </summary>
        [DefaultValue(20)]
        public Int32 DropDownListBoxMaxLines
        {
            get
            {
                return ((DataGridViewDropDownHeaderCell)HeaderCell).DropDownListBoxMaxLines;
            }
            set
            {
                ((DataGridViewDropDownHeaderCell)HeaderCell).DropDownListBoxMaxLines = value;
            }
        }

        public string[] AllOptions
        {
            get
            {
                return ((DataGridViewDropDownHeaderCell)HeaderCell).AllOptions;
            }
            set
            {
                ((DataGridViewDropDownHeaderCell)HeaderCell).AllOptions = value;
            }
        }

        private void DataGridViewDropDownHeaderColumn_HeaderOptionClicked(int obj)
        {
            HeaderOptionClicked(this, obj);
        }

        public event Action<object, int> HeaderOptionClicked;
    }

}
